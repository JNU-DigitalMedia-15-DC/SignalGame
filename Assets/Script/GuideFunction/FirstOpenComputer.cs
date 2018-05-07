using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstOpenComputer : FirstEnter {

		public override void UpdateText()
	{
		if(textIndex == guideTexts.Length-1)
		{
			GameManager.Instance.isFirstOpenComputer = false;
			gameObject.SetActive(false);
		}
		else textIndex++;
		guideText.text = guideTexts[textIndex];
	}
}
