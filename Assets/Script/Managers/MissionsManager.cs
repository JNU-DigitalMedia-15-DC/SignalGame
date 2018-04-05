using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSystem;

public class MissionsManager : MonoBehaviour {


	public int currentMissionIndex;	//暂定 还不知道要不要用序数来计算关卡

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DebugNextMission()
	{
		currentMissionIndex++ ;
		Dictionary<string,int> dict = new Dictionary<string, int>();
		dict.Add("Index",currentMissionIndex);
		NotifyEvent nEvent = new NotifyEvent(NotifyType.UpdateComputerTextWithMissionIndex,dict,this.gameObject);
		NotificationCenter.getInstance ().postNotification (nEvent);
	}
	public void DebugPreMission()
	{
		if(currentMissionIndex!= 0)
		{
			currentMissionIndex-- ;
			Dictionary<string,int> dict = new Dictionary<string, int>();
			dict.Add("Index",currentMissionIndex);
			NotifyEvent nEvent = new NotifyEvent(NotifyType.UpdateComputerTextWithMissionIndex,dict,this.gameObject);
			NotificationCenter.getInstance ().postNotification (nEvent);
		}
		else Debug.Log("First Mission!");
	}

	//初始化关卡 
	public void InitializeMissions(BaseMission m)
	{
		m.Initialize();
	}
}
