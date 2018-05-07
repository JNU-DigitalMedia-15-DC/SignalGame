using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSwaper : MonoBehaviour {

	private bool isNight = false;
	public SpriteRenderer nightWithLight;
	
	public void Swap()
	{
		if(isNight)
			SetNightOff();
		else SetNightOn();
	}
	private void SetNightOn()
	{
		nightWithLight.gameObject.SetActive(true);
		isNight = true;
	}
	private void SetNightOff()
	{
		nightWithLight.gameObject.SetActive(false);
		isNight = false;
	}
}
