using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeiserDLL;

namespace KeiserDLLTest
{
    class Program
    {
        static void Main (string[] args)
        {
            Console.WriteLine ("Started");
            KeiserMultiBikeParser rec = new KeiserMultiBikeParser ();

            rec.onBikeGainedEvent += BikeGained;
            rec.onBikeLostEvent += BikeLost;
            rec.onBikeUpdateEvent += BikeUpdated;

            rec.Start ("239.10.10.10", 35680);
        }

        public static void BikeGained (Bike bike)
        {
            Console.WriteLine ("New Bike with ID: " + bike.id + " and UUID: " + bike.uuidToString ());
        }

        public static void BikeLost (Bike bike)
        {
            Console.WriteLine ("Bike " + bike.id + " hasn't sent anything in a while, so we're dropping them.");
        }

        public static void BikeUpdated (Bike bike)
        {

        }
    }
}
