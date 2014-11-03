using UnityEngine;
using System.Collections;
using KeiserDLL;
using System.Collections.Generic;
using System.Linq;

namespace KeiserSDK
{
    public class KeiserManager : MonoBehaviour
    {
        List<BikeListenerInterface> bikeListenerInterfaces;
        public List<MonoBehaviour> bikeListeners;
        public bool startOnAwake = false;
        public string listeningIP = "239.10.10.10";
        public int port = 35680;
        public int bikeTimeout = 5000;
        
        List<KeiserBike> keiserBikes = new List<KeiserBike> ();
        
        // Due to Unity's non-threadsafe nature, we're gonna make a queue!
        List<Bike> addQueue = new List<Bike> ();
        List<Bike> updateQueue = new List<Bike> ();
        List<Bike> deleteQueue = new List<Bike> ();

        void Start ()
        {
            KeiserWrapper.InitListener ();
            RebuildBikeListeners ();
            BindEvents ();
            
            KeiserWrapper.listener.bikeTimeout = bikeTimeout;

            if (startOnAwake)
                StartListener ();
        }
        
        void RebuildBikeListeners ()
        {
            bikeListenerInterfaces = new List<BikeListenerInterface> ();
            
            foreach (MonoBehaviour script in bikeListeners) {
                if (script && script is BikeListenerInterface) {
                    bikeListenerInterfaces.Add ((BikeListenerInterface)script);
                }
            }
        }
        
        public void StartListener ()
        {
            KeiserWrapper.StartListener (listeningIP, port);
            Debug.Log ("Started Keiser Listener on " + listeningIP + ":" + port);
        }
        
        public void StopListener ()
        {
            KeiserWrapper.StopListener ();
        }
        
        public void BindEvents ()
        {
            KeiserWrapper.listener.onBikeGainedEvent += AddNewBike;
            KeiserWrapper.listener.onBikeLostEvent += DeleteBike;
            KeiserWrapper.listener.onBikeUpdateEvent += UpdateBike;
        }
        
        void Update ()
        {
            Bike[] tmpAdd = addQueue.ToArray ();
            Bike[] tmpUpdate = updateQueue.ToArray ();
            Bike[] tmpDelete = deleteQueue.ToArray ();
            
            addQueue.Clear ();
            updateQueue.Clear ();
            deleteQueue.Clear ();
            
            foreach (Bike bike in tmpAdd) {
                KeiserBike newBike = new KeiserBike (bike);
                keiserBikes.Add (newBike);
                
                Debug.Log ("Got new Bike: " + newBike.bikeData.bikeUUID);
                
                foreach (BikeListenerInterface b in bikeListenerInterfaces) {
                    b.gotNewBike (newBike);
                }
            }
            
            foreach (Bike bike in tmpUpdate) {
                KeiserBike updatedBike;
                if (bike.uuid [0] != 0 && bike.uuid [1] != 0)
                    updatedBike = keiserBikes.Find (y => y.bikeData.bikeUUID == bike.uuidToString ());
                else
                    updatedBike = keiserBikes.Find (y => y.bikeData.bikeId == bike.id);
                
                if (updatedBike == null) {
                    Debug.LogError ("We just tried to get a dodgy bike!");
                    return;
                }
                
                updatedBike.UpdateInfo (bike);
                
                foreach (BikeListenerInterface b in bikeListenerInterfaces) {
                    b.updatedBike (updatedBike);
                }
            }
            
            foreach (Bike bike in tmpDelete) {
                KeiserBike toDelete;
                
                if (bike.uuid [0] != 0 && bike.uuid [1] != 0)
                    toDelete = keiserBikes.Find (y => y.bikeData.bikeUUID == bike.uuidToString ());
                else
                    toDelete = keiserBikes.Find (y => y.bikeData.bikeId == bike.id);
                
                keiserBikes.Remove (toDelete);
                
                foreach (BikeListenerInterface b in bikeListenerInterfaces) {
                    b.lostBike (toDelete);
                }
            }
        }
        
        public void ClearBikes ()
        {
            keiserBikes.Clear ();
        }
        
        void AddNewBike (Bike bike)
        {
            addQueue.Add (bike);
        }
        
        void DeleteBike (Bike bike)
        {
            deleteQueue.Add (bike);
        }
        
        void UpdateBike (Bike bike)
        {
            updateQueue.Add (bike);
        }
    }
}