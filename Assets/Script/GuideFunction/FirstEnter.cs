using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstEnter : MonoBehaviour {


	protected int textIndex = 0;
	protected Text guideText;
	public string[] guideTexts;
	
	void Awake()
	{
		guideText = GetComponentInChildren<Text>();
	
	}

	void Start () {
		guideText.text = guideTexts[0];

	}
	
	// Update is called once per frame
	public virtual void UpdateText()
	{
		

	}
}
