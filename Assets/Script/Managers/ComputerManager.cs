using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NotificationSystem;

public class ComputerManager : MonoBehaviour {

	//目前功能：接受关卡管理出来的消息广播，更新文字
	//待完善：1.窗口完全弹出后用dotween更新文字2.窗口退出后清空文字
	public Text description;
	

	public string[] MissionDescription;
	private string currentDescription;
	// Use this for initialization

	void Awake()
	{
		currentDescription = MissionDescription[0];
		NotificationCenter.getInstance ().AddNotification (NotifyType.Main_Mission_Passed, null);
		NotificationCenter.getInstance ().registerObserver (NotifyType.Main_Mission_Passed, UpdateDescription);
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateDescription(NotifyEvent nE)
	{
		currentDescription = MissionDescription[ nE.Params["MainIndex"]-1 ];
	}

	public void ShowDescription()
	{
		description.text = currentDescription; 
	}

	public void ClearDescription()
	{
		description.text = null;
	}

}
