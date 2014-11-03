using UnityEngine;
using System.Collections;

namespace KeiserSDK
{
    [System.Serializable]
    public class KeiserBike
    {
        public class BikeData
        {
            public int rpm;
            public int heartRate;
            public int power;
            public int kcal;
            public int clock;
            public int rssi;
            
            public int majorVersion;
            public int minorVersion;
            public int bikeId;
            public int interval;
            public int tripDistance;
            public int bikeGear;
            
            public string bikeUUID;
        }
        public BikeData bikeData = new BikeData ();
        
        public class BikeDeltaData
        {
            public int rpm;
            public int heartRate;
            public int power;
            public int kcal;
            public int clock;
            public int rssi;
            public int tripDistance;
            public int bikeGear;
        }
        public BikeDeltaData bikeDeltas = new BikeDeltaData ();
        
        public KeiserBike (KeiserDLL.Bike bike)
        {
            bikeData.bikeUUID = bike.uuidToString ();
            UpdateInfo (bike);
        }
        
        public void UpdateInfo (KeiserDLL.Bike bike)
        {
            bikeData.rpm = bike.rpm;
            bikeData.heartRate = bike.hr;
            bikeData.power = bike.power;
            bikeData.kcal = bike.kcal;
            bikeData.clock = bike.clock;
            bikeData.rssi = bike.rssi;
            bikeData.majorVersion = bike.major;
            bikeData.minorVersion = bike.minor;
            bikeData.bikeId = bike.id;
            bikeData.interval = bike.interval;
            bikeData.tripDistance = bike.trip;
            
            bikeDeltas.rpm = bike.rpmDelta;
            bikeDeltas.heartRate = bike.hrDelta;
            bikeDeltas.power = bike.powerDelta;
            bikeDeltas.kcal = bike.kcalDelta;
            bikeDeltas.clock = bike.clockDelta;
            bikeDeltas.rssi = bike.rssiDelta;
            bikeDeltas.tripDistance = bike.tripDelta;
            bikeDeltas.bikeGear = bike.gearDelta;
        }
    }
}