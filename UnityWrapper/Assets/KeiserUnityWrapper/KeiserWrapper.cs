using KeiserDLL;
using System.Collections.Generic;

namespace KeiserSDK
{
    public class KeiserWrapper
    {
        public static KeiserMultiBikeParser listener;
        
        public static void InitListener ()
        {
            listener = new KeiserMultiBikeParser ();
        }

        // Use this for initialization
        public static void StartListener (string ip, int port)
        {
            listener.Start (ip, port);
        }

        public static void StopListener ()
        {
            listener.Stop ();
        }
    }
}