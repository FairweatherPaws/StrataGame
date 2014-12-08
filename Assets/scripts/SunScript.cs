using UnityEngine;
using System.Collections;

public class SunScript : MonoBehaviour {

	public GameObject steve;
	public bool narm;

	// Use this for initialization
	void Start () {
		if (narm) {transform.Translate(steve.GetComponent<GCScript>().sizeX,0,0);}
		transform.Translate(0,steve.GetComponent<GCScript>().sizeY/2,-steve.GetComponent<GCScript>().sizeX/2);
		light.intensity = steve.GetComponent<GCScript>().sizeX/60f;
	}

	
	// Update is called once per frame
	void Update () {
		// sizeX + 16
		// -16
		transform.Translate (1f*Time.deltaTime,0,0);
		if (transform.position.x > steve.GetComponent<GCScript>().sizeX * 3/2) {
			transform.position = new Vector3(-steve.GetComponent<GCScript>().sizeX/2, transform.position.y, transform.position.z);
		}
	}
}
