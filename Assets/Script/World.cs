using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

	public static World instance;

	public ComputerManager CM;
	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	// Update is called once per frame
	void Start () {
		
	}
}
