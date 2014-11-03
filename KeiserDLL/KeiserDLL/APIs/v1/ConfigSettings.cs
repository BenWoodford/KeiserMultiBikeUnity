using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeiserDLL.API.v1
{
    public struct ConfigSettings
    {
        public bool imperialUnits;
        public bool rssiSend;
        public bool intervalSend;
        public bool versionSend;
        public bool uuidSend;
        public bool gearSend;

        public int idOffset ()
        {
            return 0;
        }

        public int uuidOffset ()
        {
            return 1;
        }

        public int majorOffset ()
        {
            return uuidOffset () + ((uuidSend) ? 6 : 0);
        }

        public int minorOffset ()
        {
            return majorOffset () + ((versionSend) ? 1 : 0);
        }

        public int rpmOffset ()
        {
            return minorOffset () + ((versionSend) ? 1 : 0);
        }

        public int hrOffset ()
        {
            return rpmOffset () + 1;
        }

        public int powerOffset ()
        {
            return hrOffset () + 1;
        }

        public int intervalOffset ()
        {
            return powerOffset () + 2;
        }

        public int kcalOffset ()
        {
            return intervalOffset () + ((intervalSend) ? 1 : 0);
        }

        public int clockOffset ()
        {
            return kcalOffset () + ((intervalSend) ? 2 : 0);
        }

        public int tripOffset ()
        {
            return clockOffset () + ((intervalSend) ? 2 : 0);
        }

        public int rssiOffset ()
        {
            return tripOffset () + ((intervalSend) ? 2 : 0);
        }

        public int gearOffset ()
        {
            return rssiOffset () + ((rssiSend) ? 1 : 0);
        }
    }

    public static class SettingUtils
    {
        public static ConfigSettings getConfigSettings (byte configFlags)
        {
            return new ConfigSettings {
                uuidSend = (configFlags & 1) != 0,
                versionSend = (configFlags & 2) != 0,
                intervalSend = (configFlags & 4) != 0,
                rssiSend = (configFlags & 8) != 0,
                gearSend = (configFlags & 16) != 0,
                imperialUnits = (configFlags & 128) != 0
            };
        }

        public static int sizeOfData (ConfigSettings configSet)
        {
            int size = 5;
            size += (configSet.uuidSend) ? 6 : 0;
            size += (configSet.versionSend) ? 2 : 0;
            size += (configSet.intervalSend) ? 7 : 0;
            size += (configSet.rssiSend) ? 1 : 0;
            size += (configSet.gearSend) ? 1 : 0;

            return size;
        }

        public static byte[] getUUID (byte[] receivedData, int offset)
        {

            byte[] uuid = new byte[6];
            int size = 6;
            for (int x = 0; x < size; x++) {
                uuid [x] = receivedData [offset + x];
            }
            return uuid;
        }

        public static int twoByteConcat (byte lower, byte higher)
        {
            return ((higher << 8 | lower));
        }
    }
    
}
