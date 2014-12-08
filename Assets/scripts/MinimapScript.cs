using UnityEngine;
using System.Collections;

public class MinimapScript : MonoBehaviour {
	
	public GameObject steve;

	// Use this for initialization
	void Start () {
		camera.orthographicSize = steve.GetComponent<GCScript>().sizeX/3;
		transform.position = new Vector3(steve.GetComponent<GCScript>().sizeX, -100 + steve.GetComponent<GCScript>().sizeY / 2, 32);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
