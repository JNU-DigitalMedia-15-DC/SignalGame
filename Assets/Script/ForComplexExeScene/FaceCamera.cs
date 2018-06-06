using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

	private Transform cameraTransform;
	void Start () {
		cameraTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		//transform.LookAt(cameraTransform);
	}
}
