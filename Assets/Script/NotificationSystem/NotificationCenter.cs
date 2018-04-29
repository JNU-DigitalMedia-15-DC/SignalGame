using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotificationSystem{

	//可能的事件类型
	public enum NotifyType{
		Hit,
		Main_Mission_Passed,//主关卡序数更新
		InitializeMission,//初始化关卡
		ClearMission,//清空关卡
	}
	// 事件监听委托类型
	public delegate void EventListenerDelegate(NotifyEvent evt);
	
	public class NotificationCenter{
		
	// 静态单例，不需要挂载 使用getInstance()取得  
	private static NotificationCenter instance;  
	private NotificationCenter() { }//构造函数  
	public static NotificationCenter getInstance(){  
		if (instance == null)  {   
			instance = new NotificationCenter();  
		}  
		return instance;  
	}  

	//这个字典用于存储由事件类型对应的委托回调函数
	public Dictionary<NotifyType, EventListenerDelegate> notifications = new Dictionary<NotifyType, EventListenerDelegate>() ;

	
	
	  
	//向字典中添加消息以及对应的处理回调（监听者回调）
	public void AddNotification(NotifyType type,EventListenerDelegate ed)
		{
			EventListenerDelegate d = null;
			if (notifications.TryGetValue (type, out d))
				return;
			else
				notifications.Add (type, ed);
		}
	


	// 注册监视，监视者一般是需要做某些变化或者计算的对象（监听）
	public void registerObserver(NotifyType type, EventListenerDelegate listener)  {  
		if (listener == null)  {  
			Debug.LogError("registerObserver: listener不能为空");  
			return;  
		}  
				
		//Debug.Log("NotificationCenter: New Observer" + type);  

		EventListenerDelegate myListener = null;  
		notifications.TryGetValue(type, out myListener);  
		notifications [type] += listener;
	}  

	// 移除监视  
	public void removeObserver(NotifyType type, EventListenerDelegate listener)  {  
		if (listener == null)  {  
			Debug.LogError("removeObserver: listener不能为空");  
			return;  
		}  

		// 与添加的思路相同，只是这里是移除操作  
		Debug.Log("NotificationCenter: 移除监视" + type);  
			notifications [type] -= listener;  
	}  

	public void removeAllObservers()  {  
		notifications.Clear();  
	}  

	// 消息触发（广播消息）  
	public void postNotification(NotifyEvent evt)  {  
		EventListenerDelegate listenerDelegate;  
		if(notifications.TryGetValue(evt.Type, out listenerDelegate))  {    
				// 执行调用所有的监听者  
			//	Debug.Log(notifications.TryGetValue(evt.Type, out listenerDelegate));
				//Debug.Log(listenerDelegate);
				//Debug.Log("Post notification " + evt.Type + " from " + evt.Sender);
				listenerDelegate(evt);  
		}  
	}  

	}  
}
