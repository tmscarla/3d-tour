using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class colliderTrigger : MonoBehaviour {

	public Transform placeCube;
	private bool isOpen = false;
	private bool isFirstOpening = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) && isOpen) {
			placeCube.Find ("Popup").gameObject.SetActive (false);
			isOpen = false;
		}
	}

	IEnumerator OnTriggerStay(Collider other)
	{	
		if (Input.GetKeyDown (KeyCode.Escape) && isOpen == false) {
			if (isFirstOpening) {
				WWW wwwPicture = new WWW (placeCube.Find ("Popup/Panel/PictureUrl").gameObject.GetComponent<Text> ().text);
				yield return wwwPicture;
				placeCube.Find ("Popup/Panel/Image").gameObject.GetComponent<Image>().sprite =
					Sprite.Create(wwwPicture.texture, new Rect(0, 0, wwwPicture.texture.width, wwwPicture.texture.height), new Vector2(0.5f, 0.5f));
				isFirstOpening = false;
			}

			placeCube.Find ("Popup").gameObject.SetActive (true);
			isOpen = true;
		}

		else if (Input.GetKeyDown("q") && isOpen == true) {
			placeCube.Find ("Popup").gameObject.SetActive (false);
			isOpen = false;
		}
	}

	void OnTriggerExit(Collider other) {
		placeCube.Find ("Popup").gameObject.SetActive (false);
		isOpen = false;
	}

	IEnumerator OnMouseDown(){
		if (!isOpen) {
			if (isFirstOpening) {
				WWW wwwPicture = new WWW (placeCube.Find ("Popup/Panel/PictureUrl").gameObject.GetComponent<Text> ().text);
				yield return wwwPicture;
				placeCube.Find ("Popup/Panel/Image").gameObject.GetComponent<Image>().sprite =
					Sprite.Create(wwwPicture.texture, new Rect(0, 0, wwwPicture.texture.width, wwwPicture.texture.height), new Vector2(0.5f, 0.5f));
				isFirstOpening = false;
			}

			placeCube.Find ("Popup").gameObject.SetActive (true);
			isOpen = true;
		}

		else if (isOpen) {
			placeCube.Find ("Popup").gameObject.SetActive (false);
			isOpen = false;
		}
	}

}
