using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace KeiserDLL
{
    public class Bike
    {
        //API Independent
        public int updates;
        public Stopwatch timeFromStart, timeFromUpdate;
        public TimeSpan elapsedAtLastUpdate;

        // API Versions: 1.0, 0.l8
        public int rpm = 0;
        public int rpmDelta = 0;

        public int hr = 0;
        public int hrDelta = 0;

        public int power = 0;
        public int powerDelta = 0;

        public int kcal = 0;
        public int kcalDelta = 0;

        public int clock = 0;
        public int clockDelta = 0;

        public int rssi = 0;
        public int rssiDelta = 0;

        public byte[] uuid = new byte[6];

        //API Versions: 1.0
        public int major = 0;
        public int minor = 0;
        public int id = 0;
        public int interval = 0;
        public int trip = 0;
        public int tripDelta = 0;
        
        //API Versions 1.1
        public int gear = 0;
        public int gearDelta = 0;

        public Bike (byte[] uuid)
        {
            this.uuid = uuid;
            updates = 0;
            timeFromStart = Stopwatch.StartNew ();
            timeFromUpdate = Stopwatch.StartNew ();
        }

        public Bike (int id)
        {
            this.id = id;
            updates = 0;
            timeFromStart = Stopwatch.StartNew ();
            timeFromUpdate = Stopwatch.StartNew ();
        }

        public bool uuidEquals (byte[] otherUuid)
        {
            return(
                otherUuid [0] == uuid [0] &&
                otherUuid [1] == uuid [1] &&
                otherUuid [2] == uuid [2] &&
                otherUuid [3] == uuid [3] &&
                otherUuid [4] == uuid [4] &&
                otherUuid [5] == uuid [5]
            );
        }

        public bool idEquals (int otherId)
        {
            return otherId == id;
        }

        public void stop ()
        {
            timeFromStart.Stop ();
            timeFromUpdate.Stop ();
        }

        // API v1.0 Update (based on method-signature)
        public void Update (byte[] uuid, int major, int minor, int rpm, int hr, int power, int interval, int kcal, int clock, int trip, int rssi, int gear)
        {
            this.major = major;
            this.minor = minor;

            this.rpmDelta = rpm - this.rpm;
            this.rpm = rpm;

            this.hrDelta = hr - this.hr;
            this.hr = hr;

            this.powerDelta = power - this.power;
            this.power = power;

            this.interval = interval;

            this.kcalDelta = kcal - this.kcal;
            this.kcal = kcal;

            this.clockDelta = clock - this.clock;
            this.clock = clock;

            this.tripDelta = trip - this.trip;
            this.trip = trip;

            this.rssiDelta = rssi - this.rssi;
            this.rssi = rssi;

            this.gearDelta = gear - this.gear;
            this.gear = gear;

            updates++;
            elapsedAtLastUpdate = timeFromStart.Elapsed;
            timeFromUpdate.Reset ();
            timeFromUpdate.Start ();

            KeiserMultiBikeParser.UpdatedBike (this);
        }

        public override string ToString ()
        {
            return ("Bike: " + id + " Version: " + major + "." + minor + " RPM: " + rpm + " hr: " + hr + " Power: " + power + " Interval: " + interval + " Kcal: " + kcal + " Clock: " + clock + " Trip: " + trip + " rssi " + rssi + " Gear: " + gear);
        }

        public string uuidToString ()
        {
            string uuidString = "";
            int count = 0;
            foreach (Byte segment in uuid) {
                uuidString += string.Format ("{0:X2}", segment);
                if (count++ < 5)
                    uuidString += ":";
            }
            return uuidString;
        }
    }
}
