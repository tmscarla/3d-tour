using UnityEngine;
using System.Collections;

public class PlayerLocationService : MonoBehaviour {

	public GeoPoint loc = new GeoPoint();
	[HideInInspector]
	public float trueHeading;
	public bool locServiceIsRunning = false;
	public int maxWait = 30; // seconds
	private float locationUpdateInterval = 0.2f; // seconds
	private double lastLocUpdate = 0.0; //seconds



	public void StartLocationService() {
		Debug.Log ("Player Loc started.");
		StartCoroutine (_StartLocationService ());
	}

	public IEnumerator _StartLocationService()
	{
		
		// First, check if user has location service enabled
		if (!Input.location.isEnabledByUser) {
			Debug.Log ("Locations is not enabled.");

			//NOTE: If location is not enabled, we initialize the postion of the player to somewhere in Florence
			loc.setLatLon_deg (43.77142629999999f, 11.254018999999971f); 

			GameManager.Instance.playerStatus = GameManager.PlayerStatus.FreeFromDevice;
			// To get the game run on Editor without location services
			locServiceIsRunning = true;
			yield break;
		}

		// Start service before querying location
		Input.location.Start();
		// Wait until service initializes
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		// Service didn't initialize in maxWait seconds
		if (maxWait < 1)
		{
			print("Locations services timed out");
			yield break;
		}

		// Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed)
		{
			print("Location services failed");
			yield break;
		} else if (Input.location.status == LocationServiceStatus.Running){
			GameManager.Instance.playerStatus = GameManager.PlayerStatus.TiedToDevice;
			loc.setLatLon_deg (Input.location.lastData.latitude, Input.location.lastData.longitude);
			Debug.Log ("Location: " + Input.location.lastData.latitude.ToString ("R") + " " + Input.location.lastData.longitude.ToString ("R") + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
			locServiceIsRunning = true;
			lastLocUpdate = Input.location.lastData.timestamp;
		} else {
			print ("Unknown Error!");
		}
		Debug.Log (loc.ToString());
	}

	public IEnumerator RunLocationService()
	{
		double lastLocUpdate = 0.0;
		while (true) {
			if (lastLocUpdate != Input.location.lastData.timestamp) {
				loc.setLatLon_deg (Input.location.lastData.latitude, Input.location.lastData.longitude);
				trueHeading = Input.compass.trueHeading;
				Debug.Log ("Location: " + Input.location.lastData.latitude.ToString ("R") + " " + Input.location.lastData.longitude.ToString ("R") + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
				//locServiceIsRunning = true;
				lastLocUpdate = Input.location.lastData.timestamp;
			}
			yield return new WaitForSeconds(locationUpdateInterval);
		}
	}
}