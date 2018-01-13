using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class createLocations : MonoBehaviour {

	public Transform place;
	public Transform playerArrowContainer;
	public Transform player;
	public Transform ui;

	public static Places[] categorizedPlaces = new Places[8];
	public static PlacesObjects[] categorizedPlacesObjects = new PlacesObjects[8];

	public static Places places;
	public static string[] categoriesNames = {"ARTS_ENTERTAINMENT", "EDUCATION", "FITNESS_RECREATION",
		"FOOD_BEVERAGE", "HOTEL_LODGING", "MEDICAL_HEALTH", 
		"SHOPPING_RETAIL", "TRAVEL_TRANSPORTATION"};
	public static bool[] isToggleEnabled = {false, false, false, false, false, false, false, false};

	public static float initialLat;
	public static float initialLon;
	private static bool isFirstCall = true;

	[System.Serializable]
	public class PlacesObjects {
		public List<Transform> data = new List<Transform>();
	}

	[System.Serializable]
	public class Picture {
		public string is_silhouette;
		public string url;
		public int id;
	}

	[System.Serializable]
	public class aPicture {
		public Picture data;
	}

	[System.Serializable]
	public class Location { 
		public string city;
		public string country;
		public float latitude;
		public float longitude;
		public string state;
		public string street;
		public int zip;
	}

	[System.Serializable]
	public class Place { 
		public string name;
		public int checkins;
		public aPicture picture;
		public string description;
		public Location location;
		public float overall_star_rating;
		public string link;
	}

	[System.Serializable]
	public class Places { 
		public List<Place> data = new List<Place>();
	}

	// Use this for initialization
	void Start () {
		StartCoroutine (createArrow ());

		// Enabled categories have more priority
		for(int j = 0; j < isToggleEnabled.Length; j++) {
			if(isToggleEnabled[j]) {
				StartCoroutine (createPlaces (j, true)); 
			}
		}


		// Disabled categories can load after
		for(int j = 0; j < isToggleEnabled.Length; j++) {
			if(!isToggleEnabled[j]) {
				StartCoroutine (createPlaces (j, false)); 
			}
		}

	}


	void selectColor(MeshRenderer mr, int c) {
		switch(c) {
		case 0:
			foreach (Material m in mr.materials) {
				m.color = new Color32(29, 131, 59, 255);
			}
			break;
		case 1:
			foreach (Material m in mr.materials) {
				m.color = new Color32 (181, 8, 8, 255);
			}
			break;
		case 2:
			foreach (Material m in mr.materials) {
				m.color = new Color32 (232, 199, 63, 255);
			}
			break;
		case 3:
			foreach (Material m in mr.materials) {
				m.color = new Color32 (50, 85, 216, 255);
			}
			break;
		case 4:
			foreach (Material m in mr.materials) {
				m.color = new Color32 (160, 43, 211, 255);
			}
			break;
		case 5:
			foreach (Material m in mr.materials) {
				m.color = new Color32(255, 255, 255, 255);
			}
			break;
		case 6:
			foreach (Material m in mr.materials) {
				m.color = new Color32(84, 84, 84, 255);
			}
			break;
		case 7:
			foreach (Material m in mr.materials) {
				m.color = new Color32(254, 138, 50, 255);
			}
			break;
		}
	}

	float placeFactor(Place p) {
		float c = 0f;
		float r = p.overall_star_rating;
		float a = 0.7f;

		if (p.checkins < 10)
			c = 0;
		else if (p.checkins >= 10 && p.checkins < 100)
			c = 1;
		else if (p.checkins >= 100 && p.checkins < 1000)
			c = 2;
		else if (p.checkins >= 1000 && p.checkins < 10000)
			c = 3;
		else if (p.checkins >= 10000 && p.checkins < 100000)
			c = 4;
		else
			c = 5;

		return (a*c + (1-a)*r) * 0.8f;
	}


	// Update is called once per frame
	void Update () {
		
	}

	public void createPlacesInterface() {

		// Enabled categories have more priority
		for(int j = 0; j < isToggleEnabled.Length; j++) {
			if(isToggleEnabled[j]) {
				StartCoroutine (createPlaces (j, true)); 
			}
		}


		// Disabled categories can load after
		for(int j = 0; j < isToggleEnabled.Length; j++) {
			if(!isToggleEnabled[j]) {
				StartCoroutine (createPlaces (j, false)); 
			}
		}

	}

	IEnumerator createPlaces(int j, bool isVisible) {
		WWW www;
		string url;

		// Disable controllers until it loads all the resources
		player.gameObject.GetComponent<YControlCs> ().enabled = false;
		ui.Find ("Panel/Arts").GetComponent<Toggle> ().enabled = false;
		ui.Find ("Panel/Education").GetComponent<Toggle> ().enabled = false;
		ui.Find ("Panel/Fitness").GetComponent<Toggle> ().enabled = false;
		ui.Find ("Panel/Food").GetComponent<Toggle> ().enabled = false;
		ui.Find ("Panel/Hotel").GetComponent<Toggle> ().enabled = false;
		ui.Find ("Panel/Medical").GetComponent<Toggle> ().enabled = false;
		ui.Find ("Panel/Shopping").GetComponent<Toggle> ().enabled = false;
		ui.Find ("Panel/Transportation").GetComponent<Toggle> ().enabled = false;

		yield return new WaitForSeconds (5);

		float lat_d = GameManager.Instance.playerGeoPosition.lat_d;
		float lon_d = GameManager.Instance.playerGeoPosition.lon_d;

		// Assign global variables for future retrieving
		if (isFirstCall) {
			initialLat = lat_d;
			initialLon = lon_d;
			isFirstCall = false;
		}

		// Load places from Facebook
		url = "https://graph.facebook.com/v2.9/search?type=place&center="+lat_d+","+lon_d+"&distance=1000&fields=name,checkins,picture,description,location,overall_star_rating,link&categories=[\""+categoriesNames[j]+"\"]&access_token=109326626282214|4f7b0c5393305ecd3cfe0a3504ded604";
		www = new WWW(url);
		yield return www;
		places = new Places ();
		JsonUtility.FromJsonOverwrite (www.text, places);
		categorizedPlaces[j] = places;

		PlacesObjects po = new PlacesObjects ();
		foreach(Place p in places.data) {

			//Instantiate place
			Transform t = Instantiate (place);
			t.GetComponentInChildren<TextMesh> ().text = p.name;
			po.data.Add (t);
			categorizedPlacesObjects [j] = po;

			//Scale place
			float scaleFactor = placeFactor(p);
			t.localScale += new Vector3(scaleFactor, scaleFactor, scaleFactor);
			t.transform.position += new Vector3( 0f, scaleFactor * 0.5f, 0f );

			//Select color
			MeshRenderer mr = t.Find ("PlaceCube/Model/Mesh").GetComponent<MeshRenderer> ();
			selectColor (mr, j);

			//Set popup properties
			t.Find ("PlaceCube/Popup/Panel/Name").gameObject.GetComponent<Text> ().text = p.name;
			t.Find ("PlaceCube/Popup/Panel/PictureUrl").gameObject.GetComponent<Text> ().text = p.picture.data.url;
			t.Find ("PlaceCube/Popup/Panel/PageLink").gameObject.GetComponent<Text> ().text = p.link;
			t.Find ("PlaceCube/Popup/Panel/Name/Info/Checkins").gameObject.GetComponent<Text> ().text = "Checkins: " + p.checkins;
			t.Find ("PlaceCube/Popup/Panel/Name/Info/Ratings").gameObject.GetComponent<Text> ().text = "Ratings: " + p.overall_star_rating;
			t.Find ("PlaceCube/Popup/Panel/Lat").gameObject.GetComponent<Text> ().text = p.location.latitude.ToString();
			t.Find ("PlaceCube/Popup/Panel/Lon").gameObject.GetComponent<Text> ().text = p.location.longitude.ToString();
			t.Find ("PlaceCube/Popup/Panel/PlayerLat").gameObject.GetComponent<Text> ().text = lat_d.ToString ();
			t.Find ("PlaceCube/Popup/Panel/PlayerLon").gameObject.GetComponent<Text> ().text = lon_d.ToString();



			t.gameObject.SetActive (true);

			//Select position
			ObjectPosition op = t.gameObject.GetComponent<ObjectPosition>();
//			GeoPoint gp = new GeoPoint(p.location.latitude, p.location.longitude);
//			op.setPositionOnMap (gp);
			op.lat_d = p.location.latitude;
			op.lon_d = p.location.longitude;
			op.createPos ();

//			if (!isVisible)		
//				t.gameObject.SetActive (false);
//			else
//				t.gameObject.SetActive (true);
		}

		// Re-enable components 
		player.gameObject.GetComponent<YControlCs> ().enabled = true;
		ui.Find ("Panel/Arts").GetComponent<Toggle> ().enabled = true;
		ui.Find ("Panel/Education").GetComponent<Toggle> ().enabled = true;
		ui.Find ("Panel/Fitness").GetComponent<Toggle> ().enabled = true;
		ui.Find ("Panel/Food").GetComponent<Toggle> ().enabled = true;
		ui.Find ("Panel/Hotel").GetComponent<Toggle> ().enabled = true;
		ui.Find ("Panel/Medical").GetComponent<Toggle> ().enabled = true;
		ui.Find ("Panel/Shopping").GetComponent<Toggle> ().enabled = true;
		ui.Find ("Panel/Transportation").GetComponent<Toggle> ().enabled = true;

		StopCoroutine (createPlaces(j, isVisible));
	}

	public void refresh() {
		for(int i=0; i<8; i++) {
			foreach (Transform t in createLocations.categorizedPlacesObjects[i].data) {
				Destroy (t.gameObject);
			}
			createLocations.categorizedPlacesObjects [i].data.Clear ();
		}

		createPlacesInterface ();
	}

	public IEnumerator createArrow() {
		yield return new WaitForSeconds (3);

		float lat_d = GameManager.Instance.playerGeoPosition.lat_d;
		float lon_d = GameManager.Instance.playerGeoPosition.lon_d;

		Transform pa = Instantiate (playerArrowContainer);
		pa.gameObject.SetActive (true);

		ObjectPosition opPlayer = pa.gameObject.AddComponent<ObjectPosition>();
		GeoPoint gpPlayer = new GeoPoint(lat_d, lon_d);
		opPlayer.setPositionOnMap (gpPlayer);

		MeshRenderer mrPlayer = pa.Find("PlayerArrow").GetComponent<MeshRenderer> ();
		foreach (Material m in mrPlayer.materials) {
			m.color = new Color32(159, 239, 72, 200);
		}

		MeshRenderer mrPlayerInternal = pa.Find("PlayerArrow/Mesh").GetComponent<MeshRenderer> ();
		foreach (Material m in mrPlayerInternal.materials) {
			m.color = new Color32(159, 239, 72, 200);
		}


		StopCoroutine (createArrow ());
	}

	public void movePlayerToInitialPosition() {
		ObjectPosition opPlayer = player.gameObject.GetComponent<ObjectPosition>();
		GeoPoint gpPlayer = new GeoPoint(initialLat, initialLon);
		opPlayer.setPositionOnMap (gpPlayer);
	}

}
