using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSystem;
using UnityEngine.UI;

public class BookShelfManager : MonoBehaviour {

	//主要内容：操作教程，关卡回顾，反馈，知识点 。除了操作教程以外，都会跟随关卡进度而更新

	//书架里有知识点页面，感谢信/照片（反馈）页面以及关卡回顾页面的对象
	//在这里获得引用
	public GameObject lettersButtonPrefab;
	public Transform lettersButtonGroups; 
	public GameObject letterPanel;
	
	//暂时使用代码来加载文字资源，后转为XML
	private string[] feedbackText = {
										"尊敬的**博士：您好，现在德尔小镇的奶牛不再深受噪声影响，产奶量还稳持上升趋势。德尔小镇居民托我们公司向您表示诚挚的谢意和祝福！",
										"尊敬的**博士：您好，现在我市农村鼠灾面积得到有效的控制并且在农作物逐渐恢复，农民与市长皆大欢喜，市长特此向您表示真切地问候和诚挚的感谢，并授予您“最佳设计奖”！",
										"尊敬的**博士：您好，经过长达三年的工作时间，您辛苦了。在这三年里，您为我们公司做出了杰出的贡献，为此本公司为你准备了一趟度假旅行，目的地是光闪闪岛屿，其岛屿风光旖旎，柳暗花明，芳草萋萋，绿茵如毡，是放松身心的绝佳选择。希望您假期愉快！"

									};
	 void Awake() {
		NotificationCenter.getInstance ().AddNotification (NotifyType.Main_Mission_Passed, null);
		NotificationCenter.getInstance ().registerObserver (NotifyType.Main_Mission_Passed, AddLettersButton);
	}
	//开始关卡回顾，需要设置游戏状态
	public void SetMissionReviewing()
	{}
	//知识点页面操作
	//添加知识点页面，根据关卡属性。可能需要用到load XML文字加载
	public void AddKnowledgePointPage()
	{}

	//反馈页面操作，要根据关卡属性
	//添加感谢信按钮
	public void AddLettersButton(NotifyEvent nE)
	{
		//在panel添加按钮 按钮调用ui移动并显示指定feedback文字
		GameObject nLB = Instantiate(lettersButtonPrefab);
		nLB.transform.SetParent(lettersButtonGroups);
		nLB.GetComponentInChildren<Text>().text = "new Feedback";
		nLB.GetComponent<Button>().onClick.AddListener(delegate() {letterPanel.GetComponentInChildren<Text>().text = feedbackText[nE.Params["MainIndex"]-1];});
	}
	//添加

}
