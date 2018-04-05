using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotificationSystem{

	//在这添加可能的消息类型
	public enum NotifyType{
		Hit, UpdateComputerTextWithMissionIndex
	}
	// 事件监听委托类型
	public delegate void EventListenerDelegate(NotifyEvent evt);

	//


}
