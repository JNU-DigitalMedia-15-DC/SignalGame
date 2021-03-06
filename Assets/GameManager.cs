﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	

	private static GameManager instance;  
	public static GameManager getInstance(){  
		if (instance == null)  {   
			instance = new GameManager();  
		}  
		return instance;  
	}  

	void Start() 
	{
 		SceneManager.LoadScene(1);	
	}

	private int currentMainIndex = 1;//主关卡序数
	private int currentSubIndex = 1;//辅关卡序数

	public int[] subMissionNumber = {2,2,1};//第一二关有两个小关，第三关有一个小关
	public int[] prefix = {0,2,4};//前缀和

	public int MainIndex
	{
		get{ return currentMainIndex; }
		set{ currentMainIndex = value; } 
	}
	public int SubIndex
	{
		get{ return currentSubIndex; }
		set{ currentSubIndex = value; } 
	}
}
