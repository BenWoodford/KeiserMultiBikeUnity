using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace KeiserDLL
{
    public class KeiserMultiBikeParser
    {
        private Thread networkThread;
        private volatile Boolean keepThreadWorking;
        public bool running = false;

        public string ipAdress = "";
        public int ipPort;
        public string apiVersionStr = "";
        public int apiVersionInt;
        public static List<Bike> bikes = new List<Bike> ();

        // Timeout (if no updates from bike are given) in milliseconds
        public long bikeTimeout = 20000;

        public static KeiserMultiBikeParser instance;

        /* Events */
        public delegate void onBikeUpdate (Bike bike);

        public event onBikeUpdate onBikeUpdateEvent;

        public delegate void onBikeGained (Bike bike);

        public event onBikeGained onBikeGainedEvent;

        public delegate void onBikeLost (Bike bike);

        public event onBikeLost onBikeLostEvent;


        public KeiserMultiBikeParser ()
        {
            instance = this;
        }

        public void Start (string ipAdress, int ipPort, string apiVersionStr = "1.0+")
        {
            this.ipAdress = ipAdress;
            this.ipPort = ipPort;
            this.apiVersionStr = apiVersionStr;
            bikes.Clear ();
            networkThread = new Thread (Listen);
            keepThreadWorking = running = true;
            networkThread.Start ();
        }

        public void Stop ()
        {
            keepThreadWorking = false;
        }

        private void Listen ()
        {
            Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipEndPoint = new IPEndPoint (IPAddress.Any, ipPort);
            socket.Bind (ipEndPoint);
            socket.SetSocketOption (SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption (IPAddress.Parse (ipAdress)));
            byte[] receivedData = new byte[1024];
            Console.WriteLine ("Bound port");

            int cycleBikes = 0;

            while (keepThreadWorking) {
                if (socket.Poll (200000, SelectMode.SelectRead)) {
                    socket.Receive (receivedData);
                    switch (apiVersionStr) {
                        case "1.0+":
                            API.v1.Parser.Process (receivedData);
                            break;
                    }
                    receivedData = new byte[1024];
                }

                if (cycleBikes < bikes.Count && bikes [cycleBikes] != null) {
                    if (bikes [cycleBikes].timeFromUpdate.ElapsedMilliseconds > bikeTimeout) {
                        Bike lost = bikes [cycleBikes];
                        bikes.RemoveAt (cycleBikes);
                        LostBike (lost);
                    }
                } else if (cycleBikes >= bikes.Count) {
                    cycleBikes = -1; // It'll be 0 in a moment.
                } else {
                    bikes.RemoveAt (cycleBikes); // It's null, get rid of it and don't fire an event.
                }

                cycleBikes++;
            }
            socket.Close ();
        }

        public static Bike getBike (int id)
        {
            return bikes.Find (y => y.idEquals (id));
        }

        public static Bike getBike (byte[] uuid)
        {
            return bikes.Find (y => y.uuidEquals (uuid));
        }

        public static Bike getBike (string uuid)
        {
            return bikes.Find (y => y.uuid.ToString () == uuid);
        }

        public static void UpdatedBike (Bike bike)
        {
            if (instance.onBikeUpdateEvent != null)
                instance.onBikeUpdateEvent (bike);
        }

        public static void NewBike (Bike bike)
        {
            bikes.Add (bike);
            if (instance.onBikeGainedEvent != null)
                instance.onBikeGainedEvent (bike);
        }

        public static void LostBike (Bike bike)
        {
            if (instance.onBikeLostEvent != null)
                instance.onBikeLostEvent (bike);
        }

    }
}
