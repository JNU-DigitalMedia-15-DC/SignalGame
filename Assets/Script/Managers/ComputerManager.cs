using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NotificationSystem;

public class ComputerManager : MonoBehaviour {

	//目前功能：接受关卡管理出来的消息广播，更新文字
	//待完善：1.窗口完全弹出后用dotween更新文字2.窗口退出后清空文字
	public Text description;
	private string currentDescription;

	public string[] MissionDescription;
	// Use this for initialization

	void Awake()
	{
		NotificationCenter.getInstance ().AddNotification (NotifyType.UpdateComputerTextWithMissionIndex, null);
		NotificationCenter.getInstance ().registerObserver (NotifyType.UpdateComputerTextWithMissionIndex, UpdateDescription);
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateDescription(NotifyEvent nE)
	{
		currentDescription = MissionDescription[ nE.Params["Index"]-1 ];
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
