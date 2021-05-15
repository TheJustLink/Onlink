namespace OnlinkServer.Services
{
    class RawService : Service
    {
        protected override void Handle()
        {
            Logger.Log("Hi from " + Name);
            Wait(3000);
        }
    }
}