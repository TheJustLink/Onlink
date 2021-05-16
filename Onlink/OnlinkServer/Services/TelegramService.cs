namespace OnlinkServer.Services
{
    class TelegramService : Service
    {
        protected override void Handle()
        {
            Logger.Log("Hi from " + Name);
            Wait(3000);
        }

        protected override void OnStart()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnStop()
        {
            throw new System.NotImplementedException();
        }
    }
}