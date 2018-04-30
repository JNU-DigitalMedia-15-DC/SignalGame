using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

	//这个脚本用于储存关卡变量以及 各个小管理器的引用
	public static World instance;

	public ComputerManager CM;
	public MissionsManager MM;
	public BookShelfManager BM;




	//
	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	public void SetMissionData()
	{
		//获取当前任务的id/index/name 属性
		//将当前任务的属性 用playerprefs存储

		
	}
}
