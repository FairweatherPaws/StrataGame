using UnityEngine;
using System.Collections;

public class RotCam : MonoBehaviour {
	
	private float speed = 4f;
	public int multiplier;
	public Transform cameraFolder;
	public GameObject steve;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3(steve.GetComponent<GCScript>().sizeX * multiplier + steve.GetComponent<GCScript>().sizeX / 2,0,20);
		transform.parent = cameraFolder;
	}
	
	// Update is called once per frame
	void Update () {

		float Xtranslation = Input.GetAxis ("Horizontal") * speed * 2;
		float Ztranslation = Input.GetAxis ("Vertical") * speed * 2;
		float Ytranslation = Input.GetAxis ("Mouse ScrollWheel") * speed * 2;
		Xtranslation *= Time.deltaTime;
		Ytranslation *= Time.deltaTime;
		Ztranslation *= Time.deltaTime;
		if (transform.position.z > 10 && Ytranslation < 0) {
			Ytranslation = 0;
			transform.position = new Vector3(transform.position.x, transform.position.y, 10);
		}
		if (transform.position.z < 5 && Ytranslation > 0) {
			Ytranslation = 0;
			transform.position = new Vector3(transform.position.x, transform.position.y, 5);
		}
		if (transform.position.x > steve.GetComponent<GCScript>().sizeX * (multiplier + 1)) {
			transform.position = new Vector3(transform.position.x - steve.GetComponent<GCScript>().sizeX, transform.position.y, transform.position.z);
		}
		if (transform.position.x < steve.GetComponent<GCScript>().sizeX * multiplier) {
			transform.position = new Vector3(transform.position.x + steve.GetComponent<GCScript>().sizeX, transform.position.y, transform.position.z);
		}

		transform.Translate(-Xtranslation, Ztranslation, 0, Space.World);
		transform.Translate(0,0,Ytranslation);

	}
}
