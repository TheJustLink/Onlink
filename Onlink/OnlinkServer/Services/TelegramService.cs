namespace OnlinkServer.Services
{
    class TelegramService : Service
    {
        protected override void Handle()
        {
            Logger.Log("Hi from " + Name);
            Wait(3000);
        }
    }
}