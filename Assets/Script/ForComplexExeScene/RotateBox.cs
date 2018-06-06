using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBox : MonoBehaviour {
	private float ry,rz;
	void Start () {
		ry=0f;
		rz=0f;
	}
	
	void Update () {
		
	}

	public void changeRY(float y){
		//transform.Rotate(new Vector3(1,0,0),ry);
		/*transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
			transform.localEulerAngles.y + y,
			transform.localEulerAngles.z
		);*/
		transform.rotation =Quaternion.Euler(transform.eulerAngles.x,-y,transform.eulerAngles.z);
	}

	public void changeRX(float x){
		//transform.Rotate(Vector3.up,z);
		/*transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
			transform.localEulerAngles.y,
			transform.localEulerAngles.z + z
		);*/
		//transform.Rotate(Vector3.up * Time.deltaTime *z);
		transform.rotation =Quaternion.Euler(x,transform.eulerAngles.y,transform.eulerAngles.z);
	}

}
