using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTextGuideManager : MonoBehaviour {

	public TextsFade[] textsFades;
	// Use this for initialization
	public void ShowTextsByLevelIndex()
	{
		Debug.Log(GameManager.Instance.GetTotalMissionIndex());
		textsFades[GameManager.Instance.GetTotalMissionIndex()-1].ShowTexts();
	}
}
