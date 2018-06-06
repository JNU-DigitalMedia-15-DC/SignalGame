using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {

	public Rigidbody2D rb2d;
	public float maxSpeed;
	public float maxForce;
	[Range(0f,1.5f)]
	public float seekWeight;
	[Range(0f,1.5f)]
	public float awayWeight;
	[Range(0f,1.5f)]
	public float alignWeight;
	public float maxToleranceDistance;
	[Header("Seperate")]
	public float boundryDistance = 2f;
	[Range(0f,1.5f)]
	public float seperateWeight;
	[Header("Consolidate")]
	public float attractDistance = 1.5f;
	[Range(0f,1.5f)]
	public float attractWeight;
	private Quaternion originRotation;
	// Use this for initialization
	void Awake () {
		//rb2d = GetComponent<Rigidbody2D>();
		rb2d.velocity = new Vector3(0,1f,0);
		originRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if(10 - Mathf.Abs(transform.position.x)<boundryDistance)
		{
			rb2d.velocity = new Vector2(-rb2d.velocity.x,rb2d.velocity.y);
		}
		if(7 - Mathf.Abs(transform.position.y)<boundryDistance)
		{
			rb2d.velocity = new Vector2(rb2d.velocity.x,-rb2d.velocity.y);
		}
		//Seek(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		
	}

	Vector3 Seek(Vector3 target)
	{
		Vector3 desired = target - transform.position;
		desired = desired.normalized * maxSpeed;
		Vector3 steer = desired - (Vector3)rb2d.velocity;
		steer = Vector2.ClampMagnitude(steer,maxForce);
		return steer;
		//float angle = Vector3.Angle(rb2d.velocity,desired);
		//transform.rotation = Quaternion.Euler(0,0,angle);
		//transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,0,90+theta),0.6f);
		//transform.rotation = originRotation + Quaternion.Euler(0,0,theta);
	}

	public Vector3 Separate(List<Creature> creatures)
	{
		int count = 0;
		Vector3 sum = Vector3.zero;
		foreach(Creature c in creatures)
		{
			float d = Vector3.Distance(transform.position,c.transform.position);
			if( d>0 && d<maxToleranceDistance)
			{
				Vector3 diff = transform.position - c.transform.position;
				diff.Normalize();
				sum += diff;
				count++;
			}
		}
		if(count>0)
		{
			sum /= count;
			sum *= maxSpeed;
			Vector3 steer = sum - (Vector3)rb2d.velocity;
			Vector3.ClampMagnitude(steer,maxForce);
			return steer;
			//rb2d.AddForce(steer);
		}
		return Vector3.zero;
	}
	public Vector3 Consolidate(List<Creature> creatures)
	{
		int count = 0;
		Vector3 sum = Vector3.zero;
		foreach(Creature c in creatures)
		{
			float d = Vector3.Distance(transform.position,c.transform.position);
			if( d>0 && d<attractDistance)
			{
				Vector3 diff = c.transform.position - transform.position;
				diff.Normalize();
				sum += diff;
				count++;
			}
		}
		if(count>0)
		{
			sum /= count;
			sum *= maxSpeed;
			Vector3 steer = sum - (Vector3)rb2d.velocity;
			Vector3.ClampMagnitude(steer,maxForce);
			return steer;
		}
		else return Vector3.zero;
	}
	public Vector3 Align(List<Creature> creatures)
	{
		int count = 0;
		Vector3 sum = Vector3.zero;
		foreach(Creature c in creatures)
		{
			float d = Vector3.Distance(transform.position,c.transform.position);
			if(d>0 && d<0.5f)
			{
				sum += (Vector3)c.rb2d.velocity;
				count++;
			}
		}
		if(count>0)
		{
		sum /= creatures.Count;
		sum = sum.normalized * maxSpeed;
		
		Vector3 steer = sum - (Vector3)rb2d.velocity;
		Vector3.ClampMagnitude(steer,maxForce);

		return steer;
		}
		else return Vector3.zero;
	}
	public Vector3 Away(Vector3 target,float radius)
	{
		
		Vector3 desired = target - transform.position;
		float distance = desired.magnitude;
		desired = desired.normalized * maxSpeed;
		Vector3 steer = desired - (Vector3)rb2d.velocity;
		steer = Vector2.ClampMagnitude(steer,maxForce);
		if(distance>radius + 0.2f)
		return steer;
		else return -steer * 1.5f;
	}
	public void ApplyBehaviour(List<Creature> c,Vector3 source,float radius)
	{
		Vector3 seperate = Separate(c);
		Vector3 align = Align(c);
		Vector3 cohesion = Consolidate(c);
		//Vector3 seek = Seek(Vector3.zero/*Camera.main.ScreenToWorldPoint(Input.mousePosition)*/);
		Vector3 away = Away(source,radius);
		rb2d.AddForce(seperate * seperateWeight + away * awayWeight + cohesion * attractWeight + align * alignWeight);
	}
	
}
