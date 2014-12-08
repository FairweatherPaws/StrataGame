using UnityEngine;
using System.Collections;

public class RiverData : MonoBehaviour {

	public int id, elev, prev, next, terminate, oldDir, newDir, incoming;
	public string type;
	// terminate values: 0 = doesn't
	//					 1 = another river
	//					 2 = sea/lake
	//					 3 = self

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
