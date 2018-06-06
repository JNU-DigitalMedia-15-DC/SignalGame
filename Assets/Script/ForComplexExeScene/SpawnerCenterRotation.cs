using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerCenterRotation : MonoBehaviour {
	//private float rz;
	void Start () {
		//rz = GetComponent<Transform>().Rotate(Vector3())
		//rz=transform.rotation.z;
	}
	
	void Update () {
		//rz += Time.time*2f;
		transform.Rotate(new Vector3(3.5f,0,0));
	}
}
