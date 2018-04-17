using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSystem;

public class MissionsManager : MonoBehaviour {


	private int currentMainIndex = 1;//主关卡序数
	private int currentSubIndex = 1;//辅关卡序数

	private int[] subMissionNumber = {2,2,1};//第一二关有两个小关，第三关有一个小关

	//下一主关
	public void DebugNextMainMission()
	{
		currentMainIndex++ ;
		Dictionary<string,int> dict = new Dictionary<string, int>();
		dict.Add("MainIndex",currentMainIndex);
		dict.Add("SubIndex",currentSubIndex);
		NotifyEvent nEvent = new NotifyEvent(NotifyType.Main_Mission_Passed,dict,this.gameObject);
		NotificationCenter.getInstance ().postNotification (nEvent);
	}
	//上一主关
	public void DebugPreMainMission()
	{
		if(currentMainIndex!= 0)
		{
			currentMainIndex-- ;
			Dictionary<string,int> dict = new Dictionary<string, int>();
			dict.Add("MainIndex",currentMainIndex);
			dict.Add("SubIndex",currentSubIndex);
			NotifyEvent nEvent = new NotifyEvent(NotifyType.Main_Mission_Passed,dict,this.gameObject);//关卡更新的时候，需要调用各个管理器的接口，更新相关讯息
			NotificationCenter.getInstance ().postNotification (nEvent);
		}
		else Debug.Log("First Mission!");
	}

	//下一子关
	public void DebugNextSubMission()
	{
		int subNumber = subMissionNumber[currentMainIndex-1];//获取当前主关卡下子关卡数
		if(currentSubIndex == subNumber)
			{
				DebugNextMainMission();
				currentSubIndex = 1;
			}
		else currentSubIndex++;
		Debug.Log("Current mission is "+ currentMainIndex + " - " + currentSubIndex);
	}
	//上一子关
	public void DebugPreSubMission()
	{
		if(currentSubIndex == 1) return;
		else currentSubIndex--;
		Debug.Log("Current mission is "+ currentMainIndex + " - " + currentSubIndex);
	}


	public void UpdateManagersInfo()
	{
		
	}
	//初始化关卡 
	public void InitializeMissions()
	{
		NotifyEvent nEvent = new NotifyEvent(NotifyType.InitializeMission,this.gameObject);
		Debug.Log("I");
		NotificationCenter.getInstance().postNotification(nEvent);
	}

	//清空关卡 
	public void ClearMissions()
	{
		NotifyEvent nEvent = new NotifyEvent(NotifyType.InitializeMission,this.gameObject);
		Debug.Log("I");
		NotificationCenter.getInstance().postNotification(nEvent);
	}
}
