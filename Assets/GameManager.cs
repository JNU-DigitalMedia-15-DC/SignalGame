using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	
	//用于处理导引的flag变量
	public bool isFirstEnter = true;//第一次进入游戏
	public bool isFirstOpenBookshelf = true;//第一次进入书架
	public bool isFirstOpenFeedback = true;//第一次打开回馈界面
	public bool isFirstOpenComputer = true;//第一次打开电脑界面

	private static GameManager instance;  
	public static GameManager Instance{
		get{return instance;}
	}  

	void Start() 
	{
		instance = this;
 		SceneManager.LoadScene(1);	
	}

	public int currentMainIndex = 1;//主关卡序数
	public int currentSubIndex = 1;//辅关卡序数

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

	public int GetTotalMissionIndex()
	{
		return prefix[currentMainIndex-1]+currentSubIndex;
	}
}
