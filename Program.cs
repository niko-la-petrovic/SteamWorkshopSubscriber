using System;

namespace SteamWorkshopSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            SteamWorkshopSubscriber steamWorkshopSubscriber = new SteamWorkshopSubscriber("workshop_list.json");
            steamWorkshopSubscriber.Start();
        }
    }
}
