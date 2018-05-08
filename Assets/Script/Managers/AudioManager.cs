using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSystem;

public class AudioManager : MonoBehaviour {
	// 
	FMODUnity.StudioEventEmitter emitter;

	// 各种乐器的音量
	// 较低音正弦波的音量
	public float lowWaveAmp = 1f;
	// 较高音正弦波的音量
	public float highWaveAmp = 1f;
	// 八音盒的音量
	public float musicBoxAmp = 0f;
	// 贝斯的音量
	public float bassAmp = 0f;
	// 弦乐的音量
	public float stringsAmp = 0f;

	// 低中高通滤波器
	public float lowPass = 0.8f;
	public float bandPass = 0.8f;
	public float highPass = 0.8f;

	// 是否通关：通过关卡时设置为1，下一个关卡开始时再重置为0
	public float isWin = 0f;
	public float isTransforming = 0f;// 暂时没用，不要改这个参数

	void OnEnable(){
		var target = GameObject.Find("MusicObject");
    	emitter = target.GetComponent<FMODUnity.StudioEventEmitter>();
	}
	void Start () {
		
	}
	
	void Update () {
		
		UpdateParameters();
	}

	///<summary> 将所有声音相关参数更新至FMOD中 </summary>
	private void UpdateParameters(){
	// 更新音频相关所有参数
		// 调整各乐器音量
		emitter.SetParameter("lowWaveAmp", lowWaveAmp);
		emitter.SetParameter("highWaveAmp", highWaveAmp);
		emitter.SetParameter("musicBoxAmp", musicBoxAmp);
		emitter.SetParameter("bassAmp", bassAmp);
		emitter.SetParameter("stringsAmp", stringsAmp);

		// 调整滤波器参数
		emitter.SetParameter("lowPass", lowPass);
		emitter.SetParameter("bandPass", bandPass);
		emitter.SetParameter("highPass", highPass);
		
		emitter.SetParameter("isTransforming", isTransforming);// 这行没用，暂时不动
		// 是否播放过关音效
		emitter.SetParameter("isWin", isWin);
	}

	///<summary> 暂存放一下正式写到代码中的语句，该函数不会被调用 </summary>
	private void BackupFunction(){
		// 开始背景音乐事件（进入工作台时）


	}
}
