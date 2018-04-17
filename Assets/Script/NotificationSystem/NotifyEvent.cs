using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotificationSystem {

	//一个消息事件有需要传递的参数，事件类型，以及事件的发送者
	public class NotifyEvent {

		protected Dictionary<string, int> arguments;  //用于存储可能需要传递的参数字典  
		protected NotifyType type;  //事件类型  
		protected System.Object sender;    //发送消息事件的对象  在脚本里按情况初始化为对应对象

		//设置get&set接口
		public NotifyType Type  {  
			get { return type; }  
			set { type = value; }  
		}  

		public Dictionary<string, int> Params  {  
			get { return arguments; }  
			set { arguments = value; }  
		}  

		public System.Object Sender  {  
			get { return sender; }  
			set { sender = value; }  
		}  

		// 常用函数  
		public override string ToString()  {  
			return type + " [ " + ((sender == null) ? "null" : sender.ToString()) + " ] ";  
		}  

		public NotifyEvent Clone()  {  
			return new NotifyEvent(type, arguments, sender);  
		}  

		// 构造函数  
		public NotifyEvent(NotifyType type, System.Object sender)  {  
			Type = type;  
			Sender = sender;  
			if (arguments == null)  
			{  
				arguments = new Dictionary<string, int>();  
			}  
		}  

		public NotifyEvent(NotifyType type, Dictionary<string, int> args, System.Object sender)  {  
			Type = type;  
			arguments = args;  
			Sender = sender;  
			if (arguments == null)  {  
				arguments = new Dictionary<string, int>();  
			}  
		}  
	}
}
