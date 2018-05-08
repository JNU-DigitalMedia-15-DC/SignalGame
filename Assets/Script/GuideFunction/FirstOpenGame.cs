﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstOpenGame : FirstEnter {

	public Image b;
	public Sprite sprite;
	public override void UpdateText()
	{
		if(textIndex == guideTexts.Length-1)
		{
			GameManager.Instance.isFirstEnter = false;
			gameObject.SetActive(false);
			b.sprite = sprite;
		}
		else textIndex++;
		guideText.text = guideTexts[textIndex];
	}
}
