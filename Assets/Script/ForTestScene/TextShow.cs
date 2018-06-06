using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextShow : MonoBehaviour {


	public Text instruct;
	// Use this for initialization
	void Start () {
		Tween t = instruct.DOFade(1,0.4f).SetAutoKill(true).OnComplete(Disapear);
	}
	
	// Update is called once per frame
	void Disapear () {
		Invoke("Reverse",5f);
	}
	void Reverse()
	{
		Tween t = instruct.DOFade(0,0.4f).SetAutoKill(true);
	}
}
