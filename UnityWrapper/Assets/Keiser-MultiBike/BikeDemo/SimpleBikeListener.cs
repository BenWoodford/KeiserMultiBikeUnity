using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KeiserSDK;

public class SimpleBikeListener : MonoBehaviour,BikeListenerInterface
{
    public GameObject bikePrefab;
    public KeiserManager keiserManager;
    
    List<BikeCube> bikeCubes = new List<BikeCube> ();
    
    public void gotNewBike (KeiserSDK.KeiserBike bike)
    {
        GameObject newCube = Instantiate (bikePrefab) as GameObject;
        newCube.transform.position = new Vector3 (0, 0, bikeCubes.Count * -2);
        BikeCube bikeScript = newCube.GetComponent<BikeCube> ();
        bikeScript.bike = bike;
        bikeCubes.Add (bikeScript);
    }
    
    public void lostBike (KeiserSDK.KeiserBike bike)
    {
        BikeCube cube = bikeCubes.Find (y => y.bike == bike);
        Destroy (cube.gameObject);
        bikeCubes.Remove (cube);
    }
    
    public void updatedBike (KeiserSDK.KeiserBike bike)
    {
        bikeCubes.Find (y => y.bike == bike).BikeDidUpdate ();
    }
}
