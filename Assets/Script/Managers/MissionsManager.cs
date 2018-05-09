  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSystem;

public class MissionsManager : MonoBehaviour {

	public GameObject missionsPassUi;

	//下一主关
	public void DebugNextMainMission()
	{
		GameManager.Instance.MainIndex++ ;
		Dictionary<string,int> dict = new Dictionary<string, int>();
		dict.Add("MainIndex",GameManager.Instance.MainIndex);
		dict.Add("SubIndex",GameManager.Instance.SubIndex);
		NotifyEvent nEvent = new NotifyEvent(NotifyType.Main_Mission_Passed,dict,this.gameObject);
		NotificationCenter.getInstance ().postNotification (nEvent);
	}
	//上一主关
	public void DebugPreMainMission()
	{
		if(GameManager.Instance.MainIndex!= 0)
		{
			GameManager.Instance.MainIndex-- ;
			Dictionary<string,int> dict = new Dictionary<string, int>();
			dict.Add("MainIndex",GameManager.Instance.MainIndex);
			dict.Add("SubIndex",GameManager.Instance.SubIndex);
			NotifyEvent nEvent = new NotifyEvent(NotifyType.Main_Mission_Passed,dict,this.gameObject);//关卡更新的时候，需要调用各个管理器的接口，更新相关讯息
			NotificationCenter.getInstance ().postNotification (nEvent);
		}
		else Debug.Log("First Mission!");
	}

	//下一子关
	public void DebugNextSubMission()
	{
		int subNumber = GameManager.Instance.subMissionNumber[GameManager.Instance.MainIndex-1];//获取当前主关卡下子关卡数
		Debug.Log("当前主关： " + GameManager.Instance.MainIndex + " 当前子关 " + GameManager.Instance.SubIndex);
		if(GameManager.Instance.SubIndex == subNumber)
			{
				DebugNextMainMission();
				GameManager.Instance.SubIndex = 1;
			}
		else GameManager.Instance.SubIndex++;
		Debug.Log("Current mission is "+ GameManager.Instance.MainIndex + " - " + GameManager.Instance.SubIndex);
		World.instance.BGSwaper.Swap();
	}
	//上一子关
	public void DebugPreSubMission()
	{
		if(GameManager.Instance.SubIndex == 1) return;
		else GameManager.Instance.SubIndex--;
		Debug.Log("Current mission is "+ GameManager.Instance.MainIndex + " - " + GameManager.Instance.SubIndex);
	}


	public void PassMission()
	{
		missionsPassUi.SetActive(true);
		World.instance.AM.setWinTrue();
	}
	//初始化关卡 
	public void InitializeMissions()
	{
		
		Dictionary<string,int> dict = new Dictionary<string, int>(); 
		dict.Add("LevelIndex",GameManager.Instance.SubIndex + GameManager.Instance.prefix[GameManager.Instance.MainIndex-1]);
		NotifyEvent nEvent = new NotifyEvent(NotifyType.InitializeMission,dict,this.gameObject);
		//Debug.Log("I");
		NotificationCenter.getInstance().postNotification(nEvent);
	}

	/// <summary>
	/// 清空关卡，用tag搜索场景物体消除
	/// </summary>
	public void ClearMissions()
	{
		GameObject[] papers = GameObject.FindGameObjectsWithTag("paper");
		foreach(GameObject go in papers)
		Destroy(go);
	}
}
