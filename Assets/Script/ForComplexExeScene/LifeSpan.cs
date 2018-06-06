using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSpan : MonoBehaviour {

	void Start () {
		Destroy(gameObject,4.0f);
	}

}
