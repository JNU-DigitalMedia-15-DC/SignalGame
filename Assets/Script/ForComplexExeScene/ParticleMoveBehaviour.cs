using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMoveBehaviour : MonoBehaviour {

	void Start () {
	}
	
	void Update () {
		GetComponent<Transform>().Translate(new Vector3(1,0,0)*Time.deltaTime*3f,Space.Self);
	}
}
