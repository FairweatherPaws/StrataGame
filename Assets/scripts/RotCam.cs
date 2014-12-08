using UnityEngine;
using System.Collections;

public class RotCam : MonoBehaviour {
	
	private float speed = 4f;
	private float rspeed = 4f;
	private int toggle = 0;
	private bool switcher = false;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			
			if (toggle == 0) {speed = 10f; toggle = 1; goto endloop;}
			if (toggle == 1) {speed = 20f; toggle = 2; goto endloop;}
			if (toggle == 2) {speed = 40f; toggle = 3; goto endloop;}
			if (toggle == 3) {speed = 4f; toggle = 0; goto endloop;}
			
		}
	endloop:

		float Yrotation = Input.GetAxis("Mouse Y") * rspeed * 15;
		float Xrotation = Input.GetAxis("Mouse X") * rspeed * 15;
		float Zrotation = Input.GetAxis("Rotate Sideways") * rspeed * 3;
		float Xtranslation = Input.GetAxis ("Horizontal") * speed * 2;
		float Ztranslation = Input.GetAxis ("Vertical") * speed * 2;
		float Ytranslation = Input.GetAxis ("Up Shift") * speed * 2;
		Xtranslation *= Time.deltaTime;
		Ytranslation *= Time.deltaTime;
		Ztranslation *= Time.deltaTime;
		Yrotation *= Time.deltaTime;
		Xrotation *= Time.deltaTime;
		Zrotation *= Time.deltaTime;
		
		transform.Rotate(-Yrotation, Xrotation, -Zrotation);
		transform.Translate(Xtranslation, Ytranslation, Ztranslation);

	}
}
