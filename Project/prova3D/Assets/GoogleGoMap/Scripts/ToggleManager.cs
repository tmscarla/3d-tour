using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void artsChanged (bool b) {
		createLocations.isToggleEnabled [0] = b;
		if (b == false) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[0].data) {
				t.gameObject.SetActive (false);
			}
		}
		if (b == true) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[0].data) {
				t.gameObject.SetActive (true);
			}
		}
	}

	public void eduChanged (bool b) {
		createLocations.isToggleEnabled [1] = b;
		if (b == false) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[1].data) {
				t.gameObject.SetActive (false);
			}
		}
		if (b == true) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[1].data) {
				t.gameObject.SetActive (true);
			}
		}
	}

	public void fitnessChanged (bool b) {
		createLocations.isToggleEnabled [2] = b;
		if (b == false) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[2].data) {
				t.gameObject.SetActive (false);
			}
		}
		if (b == true) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[2].data) {
				t.gameObject.SetActive (true);
			}
		}
	}

	public void foodChanged (bool b) {
		createLocations.isToggleEnabled [3] = b;
		if (b == false) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[3].data) {
				t.gameObject.SetActive (false);
			}
		}
		if (b == true) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[3].data) {
				t.gameObject.SetActive (true);
			}
		}
	}

	public void hotelChanged (bool b) {
		createLocations.isToggleEnabled [4] = b;
		if (b == false) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[4].data) {
				t.gameObject.SetActive (false);
			}
		}
		if (b == true) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[4].data) {
				t.gameObject.SetActive (true);
			}
		}
	}

	public void medChanged (bool b) {
		createLocations.isToggleEnabled [5] = b;
		if (b == false) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[5].data) {
				t.gameObject.SetActive (false);
			}
		}
		if (b == true) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[5].data) {
				t.gameObject.SetActive (true);
			}
		}
	}

	public void shoppingChanged (bool b) {
		createLocations.isToggleEnabled [6] = b;
		if (b == false) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[6].data) {
				t.gameObject.SetActive (false);
			}
		}
		if (b == true) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[6].data) {
				t.gameObject.SetActive (true);
			}
		}
	}

	public void transportChanged (bool b) {
		createLocations.isToggleEnabled [7] = b;
		if (b == false) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[7].data) {
				t.gameObject.SetActive (false);
			}
		}
		if (b == true) {
			foreach(Transform t in createLocations.categorizedPlacesObjects[7].data) {
				t.gameObject.SetActive (true);
			}
		}
	}


}
