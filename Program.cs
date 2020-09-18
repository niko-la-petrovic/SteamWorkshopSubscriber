using System;
using System.Linq;

namespace SteamWorkshopSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            SteamWorkshopSubscriber steamWorkshopSubscriber = null;
            if (args.Length == 0)
            {
                steamWorkshopSubscriber = new SteamWorkshopSubscriber(new System.Collections.Generic.List<string> { "workshop_list.json" });
            }
            else
            {
                steamWorkshopSubscriber = new SteamWorkshopSubscriber(args.ToList());
            }
            steamWorkshopSubscriber.Start();
        }
    }
}
