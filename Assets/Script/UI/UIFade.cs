using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIFade : MonoBehaviour {

	Tween tween,rTween;
	Image I;
	float originalAlpha;
	public float targetAlpha; 

	// Use this for initialization
	void Start () {
		I = gameObject.GetComponent<Image>();
		originalAlpha = I.color.a;
		tween = I.DOFade (targetAlpha, 0.2f).SetEase (Ease.Linear).SetAutoKill(false).Pause().OnComplete(ResetRT);
		rTween = I.DOFade (originalAlpha, 0.2f).SetEase (Ease.Linear).SetAutoKill(false).Pause().OnComplete(ResetT);
	}
		
	public virtual void ResetT () {
		tween.Rewind ();
	}
	public virtual void ResetRT () {
		rTween.Rewind ();
	}

	public void Move () {
		if (I.color.a == targetAlpha)
			return;
		tween.Play ();
	}
	public void ReverseMove () {
		if (I.color.a == originalAlpha)
			return;
		rTween.Play ();
	}
}
