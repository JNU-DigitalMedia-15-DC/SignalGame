using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextsFade : MonoBehaviour {

	private Text[] texts;
	private Tweener[] textTweens;
	private Tweener[] textRTweens;
	// Use this for initialization
	void Awake () {
		texts = GetComponentsInChildren<Text>();
		textTweens = new Tweener[texts.Length];
		textRTweens = new Tweener[texts.Length];
		Debug.Log(texts.Length);
	}
	void Start(){
		for (int i = 0; i < texts.Length; i++)
		{
			textTweens[i] = texts[i].DOFade(1,0.6f).Pause().SetAutoKill(true);
			//textTweens[i].OnComplete( ()=> {textTweens[i].Rewind();});
			textRTweens[i] = texts[i].DOFade(0,0.2f).Pause().SetAutoKill(true);
			//textRTweens[i].OnComplete( ()=> {textRTweens[i].Rewind();});
			Debug.Log("complete " + i);
		}
	}
	
	// Update is called once per frame
	public void ShowTexts()
	{
		foreach (Tweener tw in textTweens)
		{
			tw.Play();
		}
		Invoke("HideTexts",7f);
	} 
	public void HideTexts()
	{
		foreach (Tweener tw in textRTweens)
		{
			tw.Play();
		}
	} 
}
