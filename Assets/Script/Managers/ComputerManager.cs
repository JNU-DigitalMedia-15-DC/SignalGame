using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NotificationSystem;

public class ComputerManager : MonoBehaviour {

	//目前功能：接受关卡管理出来的消息广播，更新文字
	//待完善：1.窗口完全弹出后用dotween更新文字2.窗口退出后清空文字
	public Text description;
	

	public string[] MissionDescription = new string[3];//公变存储所有要用的文字
	private string currentDescription;
	private string[] currentEmailDescriptions = new string[3];//当前可用的文本数组

	private int emailPageIndex = 0;
	// Use this for initialization

	void Awake()
	{
		NotificationCenter.getInstance ().AddNotification (NotifyType.Main_Mission_Passed, null);
		NotificationCenter.getInstance ().registerObserver (NotifyType.Main_Mission_Passed, UpdateDescription);
	}
	void Start () {
		currentEmailDescriptions[0] = MissionDescription[0];
	}


	/// <summary>
	/// 打开电脑界面时自动切到当前主管卡序数对应的文本
	/// </summary>
	public void InitiateEmail()
	{
		description.text = currentEmailDescriptions[GameManager.Instance.MainIndex-1];
		emailPageIndex = GameManager.Instance.MainIndex-1;
	}
	
	public void NextEmail()
	{
		if(emailPageIndex == currentEmailDescriptions.Length || emailPageIndex>=GameManager.Instance.MainIndex-1)
		return;
		else emailPageIndex++;
		ShowDescription();
	}
	public void LastEmail()
	{
		if(emailPageIndex == 0)
			return;
		else emailPageIndex--;
		ShowDescription();
	}
	/// <summary>
	/// 将预置文本数组中的特定内容赋给当前可使用的文本数组内容
	/// </summary>
	/// <param name="nE"> 消息事件</param>
	public void UpdateDescription(NotifyEvent nE)
	{
		currentEmailDescriptions[ nE.Params["MainIndex"]-1 ] = MissionDescription[ nE.Params["MainIndex"]-1 ];
	}

	public void ShowDescription()
	{
		description.text = currentEmailDescriptions[emailPageIndex]; 
		Debug.Log(currentEmailDescriptions[0]);
		Debug.Log(currentEmailDescriptions[1]);
		Debug.Log(currentEmailDescriptions[2]);
	}

	public void ClearDescription()
	{
		description.text = null;
	}

}
