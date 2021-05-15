using System.Collections.Generic;

using OnlinkServer.Logs;

namespace OnlinkServer.Services
{
    class ServicePool
    {
        public bool IsAllServicesRunned
        {
            get
            {
                foreach (var service in Services)
                {
                    if (service.IsRunning == false)
                        return false;
                }

                return true;
            }
        }
        public bool IsAllServicesStopped
        {
            get
            {
                foreach (var service in Services)
                {
                    if (service.IsRunning)
                        return false;
                }

                return true;
            }
        }

        public ILogger Logger = EmptyLogger.Instance;

        private readonly List<Service> Services = new List<Service>();

        public void AddServices(params Service[] services)
        {
            foreach (var service in services)
                AddService(service);
        }
        public void AddService(Service service)
        {
            service.Logger = Logger;
            Services.Add(service);
        }

        public void RemoveServices()
        {
            for (var i = 0; i < Services.Count;)
                RemoveService(i);
        }
        private void RemoveService(int index)
        {
            Services[index].Stop();
            Services.RemoveAt(index);
        }

        public void StartServices()
        {
            Logger.Log("Starting services...");
            foreach (var service in Services)
                StartService(service);

            WaitServicesStarting();
            Logger.Log("Services started");
        }
        private void StartService(Service service)
        {
            service.Start();
        }
        private void WaitServicesStarting()
        {
            while (IsAllServicesRunned == false) { }
        }

        public void StopServices()
        {
            Logger.Log("Stopping services...");
            foreach (var service in Services)
                StopService(service);

            WaitServicesStopping();
            Logger.Log("Services stopped");
        }
        private void StopService(Service service)
        {
            service.Stop();
        }
        private void WaitServicesStopping()
        {
            while (IsAllServicesStopped == false) { }
        }
    }
}