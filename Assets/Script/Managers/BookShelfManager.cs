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

	static int pagesIndex = 0;
	static string[] currentUseContents = null;
	string[] temp = null;

	static int limitedIndex = 1;//根据关卡号来限定当前文字内容数组的访问范围 1->只能访问[0] 2->[0][1] 以此类推
	
	//暂时使用代码来加载文字资源，后转为XML
	static string[] feedbackText = {
										"尊敬的**博士：您好，现在德尔小镇的奶牛不再深受噪声影响，产奶量还稳持上升趋势。德尔小镇居民托我们公司向您表示诚挚的谢意和祝福！",
										"尊敬的**博士：您好，现在我市农村鼠灾面积得到有效的控制并且在农作物逐渐恢复，农民与市长皆大欢喜，市长特此向您表示真切地问候和诚挚的感谢，并授予您“最佳设计奖”！",
										"尊敬的**博士：您好，经过长达三年的工作时间，您辛苦了。在这三年里，您为我们公司做出了杰出的贡献，为此本公司为你准备了一趟度假旅行，目的地是光闪闪岛屿，其岛屿风光旖旎，柳暗花明，芳草萋萋，绿茵如毡，是放松身心的绝佳选择。希望您假期愉快！"

									};

	static string[] knowledgePointext = {
										 "简谐运动：如果做机械振动的质点，其位移与时间的关系遵从正弦（或余弦）函数规律，这样的振动叫做简谐运动，又名简谐振动。基本知识：函数y=Acos(ωx+φ)中，A为振幅，(ωx+φ)为相位，φ为初相位，ω为角频率波的叠加原理：1、独立传播原理：波所传播的振动不因波相遇而发生相互影响，每个波列都保持单独传播时的振动特性而继续传播。2、当两个或更多波在同一个空间中传播，在每一点的合成振幅是各个波的振幅的矢量和。",
										 "主动降噪是一种降噪技术，就是通过降噪系统产生与外界噪音相等的反向声波，将噪音中和，从而实现降噪的效果。它的原理是所有的声音都由一定的频谱组成，如果可以找到一种声音，其频谱与所要消除的噪声完全一样，只是相位刚好相反就可以将这噪声完全抵消掉。",
										 "老鼠这类动物都是以超声波进行沟通的，如果能利用一种强大的高频率超声波脉冲对鼠类听觉系统进行有效的干扰和刺激，使其无法忍受，并感到恐慌及不安，表现出食欲不振、逃离、甚至抽搐等症状，从而能达到将该鼠类驱除出他们活动范围的目的。"
										};
	static string[] basicActionText = {
										"blablablablablablablablablablablablabalbla",
										"abcabcabcabacbacbacbacbacbacbacbacbacbac",
										"I'm bb's father he is my son oh yeah"
										};
	static string[] stageReviewText = {};


	 void Awake() {
		NotificationCenter.getInstance ().AddNotification (NotifyType.Main_Mission_Passed, null);
		NotificationCenter.getInstance ().registerObserver (NotifyType.Main_Mission_Passed, AddLettersButton);
		NotificationCenter.getInstance ().registerObserver (NotifyType.Main_Mission_Passed, LimitedIndexUpdate);
	}


	void LimitedIndexUpdate(NotifyEvent nE)
	{
		limitedIndex = nE.Params["MainIndex"];
		Debug.Log("Limited = " + limitedIndex);
	}
	//基础操作


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
		nLB.GetComponent<Button>().onClick.AddListener( ()=> {letterPanel.GetComponentInChildren<Text>().text = feedbackText[nE.Params["MainIndex"]-1];});
	}
	//添加
	//要更新的文字数组，按序数更新
	//从页面左侧开始计数 1/2 3/4 5/6
	
	public void SetCurrentUse(int classIndex)
	{
		switch(classIndex)
		{
			case 1: currentUseContents = basicActionText;break;
			case 2: currentUseContents = knowledgePointext;break;
			case 3: currentUseContents = stageReviewText;break;
		}
		
	} 

	public void UpdateContents(GameObject pages)
	{
		Text f,s;
		f = pages.transform.GetChild(0).GetComponent<Text>();
		s = pages.transform.GetChild(1).GetComponent<Text>();
		temp = new string[limitedIndex+1];
		for(int i = 0;i<limitedIndex;i++)
			{
				temp[i] = currentUseContents[i];
				temp[i+1] = null;
			}
		if(pagesIndex+2 >= temp.Length)
			{
				f.text = temp[pagesIndex];
				s.text = null;
				return;
			}
		f.text = temp[pagesIndex];
		s.text = temp[pagesIndex+1];
		
	}
	public void ClearContents(GameObject pages)
	{
		//获取到左右页面的文字组件，清除文字
		pages.transform.GetChild(0).GetComponent<Text>().text = null;
		pages.transform.GetChild(1).GetComponent<Text>().text = null;
		pagesIndex = 0;//序号清零
	}

	public void PageIndexPlus()
	{
		if(pagesIndex+2 >= temp.Length)
		return;
		else if ( temp[pagesIndex+2] != null)pagesIndex+=2;
	}
	public void PageIndexSub()
	{
		if(pagesIndex == 0)
		return;
		else pagesIndex-=2;
	}



}
