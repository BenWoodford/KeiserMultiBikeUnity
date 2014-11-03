using System;

namespace KeiserDLL.API.v1
{
    public class Parser
    {
        public static void Process (byte[] receivedData)
        {
            int apiVersionInt = receivedData [0];
            switch (apiVersionInt) {
                case 11:
                case 10:
                    Parse (receivedData);
                    break;
            }
        }

        public static void Parse (byte[] receivedData)
        {
            byte configFlags = receivedData [1];
            ConfigSettings configSettings = SettingUtils.getConfigSettings (configFlags);
            int dataSize = SettingUtils.sizeOfData (configSettings);
            for (int x = 0; x < (receivedData.Length - 2) / dataSize; x++) {
                int bikeOffset = 2 + (x * dataSize);

                Bike bike;

                if (configSettings.uuidSend) {
                    byte[] uuid = SettingUtils.getUUID (receivedData, bikeOffset + configSettings.uuidOffset ());

                    if (uuid [1] == 0 && uuid [2] == 0 && uuid [3] == 0 && uuid [4] == 0 && uuid [5] == 0)
                        continue;

                    if (!KeiserMultiBikeParser.bikes.Exists (y => y.uuidEquals (uuid))) {
                        Bike newBike = new Bike (uuid);
                        newBike.id = Convert.ToInt32 (receivedData [bikeOffset]);
                        KeiserMultiBikeParser.NewBike (newBike);
                    }
                    bike = KeiserMultiBikeParser.bikes.Find (y => y.uuidEquals (uuid));
                } else {
                    // Only do this if we don't get a UUID, we should never use Bike ID as the Unique Key for a Bike (it's a user thing).
                    int id = receivedData [bikeOffset];
                    if (id == 0)
                        continue;
                    if (!KeiserMultiBikeParser.bikes.Exists (y => y.idEquals (id))) {
                        KeiserMultiBikeParser.NewBike (new Bike (id));
                    }
                    bike = KeiserMultiBikeParser.bikes.Find (y => y.idEquals (id));
                }
                UpdateBike (configSettings, receivedData, bikeOffset, bike);
            }
        }

        public static void UpdateBike (ConfigSettings configSettings, byte[] receivedData, int offset, Bike bike)
        {
            byte[] uuid = configSettings.uuidSend ? SettingUtils.getUUID (receivedData, configSettings.uuidOffset ()) : new byte[6];
            int major = configSettings.versionSend ? Convert.ToInt32 (receivedData [offset + configSettings.majorOffset ()]) : 0;
            int minor = configSettings.versionSend ? Convert.ToInt32 (receivedData [offset + configSettings.minorOffset ()]) : 0;
            int rpm = Convert.ToInt32 (receivedData [offset + configSettings.rpmOffset ()]);
            int hr = Convert.ToInt32 (receivedData [offset + configSettings.hrOffset ()]);
            int power = Convert.ToInt32 (SettingUtils.twoByteConcat (receivedData [offset + configSettings.powerOffset ()], receivedData [offset + configSettings.powerOffset () + 1]));
            int interval = configSettings.intervalSend ? Convert.ToInt32 (receivedData [offset + configSettings.intervalOffset ()]) : 0;
            int kcal = configSettings.intervalSend ? Convert.ToInt32 (SettingUtils.twoByteConcat (receivedData [offset + configSettings.kcalOffset ()], receivedData [offset + configSettings.kcalOffset () + 1])) : 0;
            int clock = configSettings.intervalSend ? Convert.ToInt32 (SettingUtils.twoByteConcat (receivedData [offset + configSettings.clockOffset ()], receivedData [offset + configSettings.clockOffset () + 1])) : 0;
            int trip = configSettings.intervalSend ? Convert.ToInt32 (SettingUtils.twoByteConcat (receivedData [offset + configSettings.tripOffset ()], receivedData [offset + configSettings.tripOffset () + 1])) : 0;
            trip = Convert.ToInt32 (trip & 32767);
            int rssi = configSettings.rssiSend ? Convert.ToInt32 (receivedData [offset + configSettings.rssiOffset ()]) : 0;
            int gear = configSettings.gearSend ? Convert.ToInt32 (receivedData [offset + configSettings.gearOffset ()]) : 0;
            bike.Update (uuid, major, minor, rpm, hr, power, interval, kcal, clock, trip, rssi, gear);
        }
    }
}

