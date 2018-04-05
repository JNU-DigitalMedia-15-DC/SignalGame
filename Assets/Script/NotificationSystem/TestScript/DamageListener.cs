using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSystem;

public class DamageListener : MonoBehaviour {

	EventListenerDelegate DoHit;
	//float hp = 2f;

	void Awake()
	{
		NotificationCenter.getInstance ().AddNotification (NotifyType.Hit, null);

	}
	// Use this for initialization
	void Start () {
		//NotifyType type = NotifyType.Hit;
		DoHit = Hit;

		NotificationCenter.getInstance ().registerObserver(NotifyType.Hit,DoHit);//监听打击消息

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Death()
	{
		
	}
	void Hit(NotifyEvent nE)
	{
	//	hp--;
		Debug.Log ("Was hitted!");
		Debug.Log ((nE.Sender as GameObject).name);
	}
}
