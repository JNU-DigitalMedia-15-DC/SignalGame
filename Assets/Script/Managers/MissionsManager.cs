  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSystem;

public class MissionsManager : MonoBehaviour {



	//下一主关
	public void DebugNextMainMission()
	{
		GameManager.Instance.currentMainIndex++ ;
		Dictionary<string,int> dict = new Dictionary<string, int>();
		dict.Add("MainIndex",GameManager.Instance.currentMainIndex);
		dict.Add("SubIndex",GameManager.Instance.currentSubIndex);
		NotifyEvent nEvent = new NotifyEvent(NotifyType.Main_Mission_Passed,dict,this.gameObject);
		NotificationCenter.getInstance ().postNotification (nEvent);
	}
	//上一主关
	public void DebugPreMainMission()
	{
		if(GameManager.Instance.currentMainIndex!= 0)
		{
			GameManager.Instance.currentMainIndex-- ;
			Dictionary<string,int> dict = new Dictionary<string, int>();
			dict.Add("MainIndex",GameManager.Instance.currentMainIndex);
			dict.Add("SubIndex",GameManager.Instance.currentSubIndex);
			NotifyEvent nEvent = new NotifyEvent(NotifyType.Main_Mission_Passed,dict,this.gameObject);//关卡更新的时候，需要调用各个管理器的接口，更新相关讯息
			NotificationCenter.getInstance ().postNotification (nEvent);
		}
		else Debug.Log("First Mission!");
	}

	//下一子关
	public void DebugNextSubMission()
	{
		int subNumber = GameManager.Instance.subMissionNumber[GameManager.Instance.currentMainIndex-1];//获取当前主关卡下子关卡数
		if(GameManager.Instance.currentSubIndex == subNumber)
			{
				DebugNextMainMission();
				GameManager.Instance.currentSubIndex = 1;
			}
		else GameManager.Instance.currentSubIndex++;
		Debug.Log("Current mission is "+ GameManager.Instance.currentMainIndex + " - " + GameManager.Instance.currentSubIndex);
		World.instance.BGSwaper.Swap();
	}
	//上一子关
	public void DebugPreSubMission()
	{
		if(GameManager.Instance.currentSubIndex == 1) return;
		else GameManager.Instance.currentSubIndex--;
		Debug.Log("Current mission is "+ GameManager.Instance.currentMainIndex + " - " + GameManager.Instance.currentSubIndex);
	}


	public void UpdateManagersInfo()
	{
		
	}
	//初始化关卡 
	public void InitializeMissions()
	{
		
		Dictionary<string,int> dict = new Dictionary<string, int>(); 
		dict.Add("LevelIndex",GameManager.Instance.currentSubIndex + GameManager.Instance.prefix[GameManager.Instance.currentMainIndex-1]);
		NotifyEvent nEvent = new NotifyEvent(NotifyType.InitializeMission,dict,this.gameObject);
		//Debug.Log("I");
		NotificationCenter.getInstance().postNotification(nEvent);
	}

	//清空关卡 
	public void ClearMissions()
	{
		NotifyEvent nEvent = new NotifyEvent(NotifyType.ClearMission,this.gameObject);
		Debug.Log("I");
		NotificationCenter.getInstance().postNotification(nEvent);
	}
}
