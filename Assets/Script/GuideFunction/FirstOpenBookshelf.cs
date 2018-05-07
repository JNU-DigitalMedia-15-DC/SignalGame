using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstOpenBookshelf : FirstEnter {

		public override void UpdateText()
	{
		if(textIndex == guideTexts.Length-1)
		{
			GameManager.Instance.isFirstOpenBookshelf = false;
			gameObject.SetActive(false);
		}
		else textIndex++;
		guideText.text = guideTexts[textIndex];
	}
}
