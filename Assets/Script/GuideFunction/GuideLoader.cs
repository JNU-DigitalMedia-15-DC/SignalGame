using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideLoader : MonoBehaviour {

	public FirstEnter[] fe;
	// Use this for initialization
	void Start() {
		LoadFirstEnter();
	}
	
	// Update is called once per frame
	public void LoadFirstEnter()
	{
		if(GameManager.Instance.isFirstEnter)
			fe[0].gameObject.SetActive(true);
		else return;
	}
	public void LoadFirstOpenBookshelf()
	{
		if(GameManager.Instance.isFirstOpenBookshelf)
			fe[1].gameObject.SetActive(true);
		else return;
	}
	public void LoadFirstOpenFeedback()
	{
		if(GameManager.Instance.isFirstOpenFeedback)
			fe[2].gameObject.SetActive(true);
		else return;
	}
	public void LoadFirstOpenComputer()
	{
		if(GameManager.Instance.isFirstOpenComputer)
			fe[3].gameObject.SetActive(true);
		else return;
	}
}
