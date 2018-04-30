using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpriteFade : MonoBehaviour {

	Tween tween,rTween;
	SpriteRenderer spriteRenderer;
	float originalAlpha;
	public float targetAlpha; 

	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		originalAlpha = spriteRenderer.color.a;
		tween = spriteRenderer.DOFade (targetAlpha, 0.2f).SetEase (Ease.Linear).SetAutoKill(false).Pause().OnComplete(ResetRT);
		rTween = spriteRenderer.DOFade (originalAlpha, 0.2f).SetEase (Ease.Linear).SetAutoKill(false).Pause().OnComplete(ResetT);
	}
		
	public virtual void ResetT () {
		tween.Rewind ();
	}
	public virtual void ResetRT () {
		rTween.Rewind ();
	}

	public void Move () {
		if (spriteRenderer.color.a == targetAlpha)
			return;
		tween.Play ();
	}
	public void ReverseMove () {
		if (spriteRenderer.color.a == originalAlpha)
			return;
		rTween.Play ();
	}
}
