using UnityEngine;
using System.Collections;
using NotificationSystem;

public class DamegeDealer : MonoBehaviour
{
	void Awake()
	{
		

	}
	public void DealDamage()
	{
		Debug.Log ("Hit!");
		NotifyEvent nEvent = new NotifyEvent (NotifyType.Hit, this.gameObject);

		NotificationCenter.getInstance ().postNotification (nEvent);
	}
}

