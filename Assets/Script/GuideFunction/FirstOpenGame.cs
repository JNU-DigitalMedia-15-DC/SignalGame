using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstOpenGame : FirstEnter {

	public override void UpdateText()
	{
		if(textIndex == guideTexts.Length-1)
		{
			GameManager.Instance.isFirstEnter = false;
			gameObject.SetActive(false);
		}
		else textIndex++;
		guideText.text = guideTexts[textIndex];
	}
}
