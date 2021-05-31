using System;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using OnlinkServer.Logging;
using OnlinkServer.Extensions;

namespace OnlinkServer
{
    class NetworkClient : IDisposable
    {
        public ILogger Logger = EmptyLogger.Instance;
        public event Action<NetworkClient> Disconnected;

        private readonly TcpClient Client;
        private readonly NetworkStream Stream;

        private byte[] _readBuffer;

        private Stack<IEnumerator> _logicEnumerators;

        public NetworkClient(TcpClient client)
        {
            Client = client;
            Stream = client.GetStream();

            _readBuffer = new byte[1024];
            _logicEnumerators = new Stack<IEnumerator>();
            _logicEnumerators.Push(SomeLogic());
        }
        ~NetworkClient() => Dispose();

        public void Disconnect()
        {
            Dispose();
            Disconnected?.Invoke(this);
        }
        public void Dispose()
        {
            Stream.Close();
            Client.Close();
        }
        
        public void Handle()
        {
            if (Client.GetConnectionState() == false)
            {
                Disconnect();
                return;
            }

            try
            {
                SafeHandle();
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                Disconnect();
            }
        }
        private void SafeHandle()
        {
            var logic = _logicEnumerators.Peek();

            if (logic.MoveNext() == false)
            {
                _logicEnumerators.Pop();
                SafeHandle();
            }
            else if (logic.Current is IEnumerator newLogic)
            {
                _logicEnumerators.Push(newLogic);
            }
        }

        private IEnumerator SomeLogic()
        {
            while (true)
            {
                yield return SendLine("Aboba");
                Logger.Log("*Sended aboba*");

                string msg = null;
                yield return Receive(msg);
                Logger.Log("Received msg : " + msg);

                yield return WaitSeconds(1000);
            }
        }

        public IEnumerator SendLine(string msg)
        {
            return Send(msg + Environment.NewLine);
        }
        public IEnumerator Send(string msg)
        {
            return Send(Encoding.UTF8.GetBytes(msg));
        }
        private IEnumerator Send(byte[] data)
        {
            var task = Stream.WriteAsync(data, 0, data.Length);
            var logic = SolveTask(task);
            logic.MoveNext();

            return logic;
        }

        public IEnumerator Receive(string msg)
        {
            var task = Stream.ReadAsync(_readBuffer, 0, _readBuffer.Length);
            yield return SolveTask(task);

            msg = Encoding.UTF8.GetString(_readBuffer);
            ClearReadBuffer();
        }

        private IEnumerator WaitSeconds(int milliseconds)
        {
            return SolveTask(Task.Delay(milliseconds));
        }
        private IEnumerator SolveTask(Task task)
        {
            if (task.IsCompleted) yield break;
            if (task.Status == TaskStatus.Created)
                task.Start();

            while (!task.IsCompleted)
                yield return null;
        }

        private void ClearReadBuffer()
        {
            for (var i = 0; i < _readBuffer.Length; i++)
                _readBuffer[i] = 0;
        }
    }
}