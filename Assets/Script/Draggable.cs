using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour , IBeginDragHandler,IDragHandler,IEndDragHandler {


	public void OnBeginDrag(PointerEventData eventdata)
	{

	}
	public void OnDrag(PointerEventData eventdata)
	{
		float newX = transform.position.x;
		float newY = transform.position.y;
		newX = Mathf.Lerp(transform.position.x,eventdata.position.x,0.45f);
		newY = Mathf.Lerp(transform.position.y,eventdata.position.y,0.45f);
		transform.position = new Vector3(newX,newY,transform.position.z);
	}
	public void OnEndDrag(PointerEventData eventdata)
	{
		float newX = transform.position.x;
		float newY = transform.position.y;
		newX = Mathf.Lerp(transform.position.x,eventdata.position.x,0.2f);
		newY = Mathf.Lerp(transform.position.y,eventdata.position.y,0.2f);
		transform.position = new Vector3(newX,newY,transform.position.z);
	}
}
