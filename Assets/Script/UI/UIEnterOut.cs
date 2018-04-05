
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


public class UIEnterOut : MonoBehaviour {

	Tween tween,rTween;
	RectTransform rT;
	Vector2 OriginalAP;
	public Vector2 TargetAP;
	// Use this for initialization
	void Start () {
		rT = GetComponent<RectTransform> ();
		OriginalAP = rT.anchoredPosition;
		tween = rT.DOAnchorPos(TargetAP, 0.5f).Pause ().SetAutoKill(false).SetEase(Ease.OutExpo).OnComplete(ResetRP);
		rTween = rT.DOAnchorPos(OriginalAP, 0.25f).Pause ().SetAutoKill(false).SetEase(Ease.InBack).OnComplete(ResetP);
		
	}

	public virtual void ResetP()
	{
		tween.Rewind ();
	}
	public virtual void ResetRP()
	{
		rTween.Rewind ();
	}
		

	public void Move () {
		tween.Play ();
	}
	public void ReverseMove () {
		rTween.Play ();
	}
}
