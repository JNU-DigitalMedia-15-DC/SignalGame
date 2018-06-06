using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwarmIntelligence : MonoBehaviour {

	[SerializeField]
	List<Creature> creatures = new List<Creature>();

	public Transform noiseSource;
	[Range(0f,3.5f)]
	public float sourceRadius; 

	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach(Creature c in creatures)
		{
			c.ApplyBehaviour(creatures,noiseSource.position,sourceRadius);
			//c.Consolidate(creatures);
		}
	}

	void ApplyBehaviour(List<Creature> c)
	{

	}

}
