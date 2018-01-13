using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

	public Transform placeCube;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClickFB() {
		string url = placeCube.Find ("Popup/Panel/PageLink").gameObject.GetComponent<Text> ().text;
		Application.OpenURL (url);
		placeCube.Find ("Popup").gameObject.SetActive (false);
	}

	public void onClickDirections() {
		string playerLat = placeCube.Find ("Popup/Panel/PlayerLat").gameObject.GetComponent<Text> ().text;
		string playerLon = placeCube.Find ("Popup/Panel/PlayerLon").gameObject.GetComponent<Text> ().text;
		string placeLat = placeCube.Find ("Popup/Panel/Lat").gameObject.GetComponent<Text> ().text;
		string placeLon = placeCube.Find ("Popup/Panel/Lon").gameObject.GetComponent<Text> ().text;

		string url = "https://www.google.com/maps/dir/" + playerLat + "," + playerLon + "/"
		             + placeLat + "," + placeLon + "/";
		Application.OpenURL (url);
	}
}

