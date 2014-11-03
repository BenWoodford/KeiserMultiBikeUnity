using UnityEngine;
using System.Collections;

public class BikeCube : MonoBehaviour
{
    public KeiserSDK.KeiserBike bike;
    public float rpmMultiplier = 1.0f;
    
    public void BikeDidUpdate ()
    {
        rigidbody.AddForce (new Vector3 (bike.bikeData.rpm * rpmMultiplier * Time.deltaTime, 0, 0), ForceMode.VelocityChange);
    }
}
