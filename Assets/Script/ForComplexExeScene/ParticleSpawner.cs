using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour {
	public GameObject[] particles;
	private float intervalTime = 0.025f;
	public Transform spawnerCenterTransform;

	private long temp = 1;
	void Start () {
		InvokeRepeating("Spawn",0.5f,intervalTime);
	}
	
	void Update () {

	}

	void Spawn(){
		if(temp%5 != 1){
			Instantiate(particles[0],transform.position,transform.rotation,spawnerCenterTransform);
		}else{
			GameObject go = Instantiate(particles[1],transform.position,transform.rotation,spawnerCenterTransform);
			go.GetComponent<SpriteRenderer>().color = Color.yellow;
		}
		temp++;
	}
}
