using System.Collections.Generic;
using UnityEngine;

/// <summary> 关卡生成器 </summary>
internal class LevelGenerator : MonoBehaviour {
    /// <summary> 纸片预置体 </summary>
    public GameObject PaperPrefab;

    /// <summary> 纸片们的Holder </summary>
    private Transform papersParentTransform;

    private void Awake() {
        // 初始化Holder
        papersParentTransform = new GameObject("Papers").transform;
    }

    /// <summary> 初始化用户修改纸片的关卡 </summary>
    public void InitializeModifyLevel() {
        DataController dc = DataController.Instance;
        Debug.Log("datacontroler instance is " + dc);
        // 关卡数据
        LevelData levelData = dc.GetCurrentLevelData();
        // 纸片们数据
        PaperData[] papersData = levelData.papersData;
        // 用户可操作纸片的数据们
        WaveData[] waveDatas = new WaveData[2];
        // 所有纸片的 WaveController们
        WaveController[] waveControllers =
            new WaveController[levelData.papersData.Length];

        // 设置纸片组 Holder 的位置（纸片组的左上角）
        papersParentTransform.position = levelData.HolderPosition;

        // 生成纸片们，尚未给予 WaveData
        {
            int i = 0;
            foreach (PaperData paperData in papersData)
                waveControllers[i++] = GetPaper(paperData);
        }

        // 配置两张基础纸片的 WaveData
        for (int i = 0; i < 2; ++i)
            waveControllers[i].WaveData = waveDatas[i] =
            new WaveData(papersData[i].waveAttributes);

        // 配置 和视图纸片 的 WaveData
        // 注: 和视图纸片 不能被用户直接修改
        //     但会自动立即反应其下 加数纸片 的修改
        //     即创建自己的 waveDataMasks，记录别人的 WaveDataMask们
        WaveData sum = waveControllers[2].WaveData = new WaveData(waveDatas);

        // 配置目标纸片的 WaveData
        // 注：目标纸片 初始化的结果为最原始的 和视图纸片 叠加一个 目标修改
        //             且初始化后不能被修改
        //     即：如果关卡只修改 用户可操作纸片们的WaveModification
        //            则至少对和纸片的拷贝级别应该达到 拷贝每个WaveModification
        WaveData goal = new WaveData(sum);
        // 对目标纸片的每个蒙版（每个用户可操作纸片）做目标修改
        for (int i = 0; i < 2; ++i) {
            goal.ModifyByMask(i, levelData.modifications[i]);
        }
        waveControllers[3].WaveData = goal;

        // 关卡初始化完成，将数据引用传送给 WaveInputController
        WaveInputController waveInputController = GetComponent<WaveInputController>();
        waveInputController.SetDatas(
            papersData,
            waveDatas,
            waveControllers
        );
        // 激活 WaveInputController
        waveInputController.enabled = true;
    }

    /// <summary> 初始化傅里叶变换与滤波关卡 </summary>
    public void InitializeFourierLevel() {
        DataController dc = DataController.Instance;
        /// <summary> 关卡数据 </summary>
        /// <remarks> 外部数据 </remarks>
        LevelData levelData = dc.GetCurrentLevelData();
        /// <summary> 纸片数据 </summary>
        /// <remarks> 外部数据 </remarks>
        PaperData paperData = levelData.papersData[0];
        /// <summary> 波参数组总数量 </summary>
        int waveAttributesCount = paperData.waveAttributes.Length;
        /// <summary> 纸片总数量 </summary>
        int papersCount = waveAttributesCount + 1;
        /// <summary> 纸片们的 WaveData们 </summary>
        WaveData[] waveDatas = new WaveData[papersCount];
        /// <summary> 纸片们的 WaveController们 </summary>
        WaveController[] waveControllers = new WaveController[papersCount];

        // 设置纸片组 Holder 的位置（纸片组的左上角）
        papersParentTransform.position = levelData.HolderPosition;

        // 生成纸片，尚未给予 WaveData
        for (int i = 0; i < papersCount; ++i) {
            waveControllers[i] = GetPaper(paperData);
        }

        // 将波参数组外部数据导入到链表中
        var waveAttributes = new LinkedList<WaveAttribute>(paperData.waveAttributes);
        // 配置总纸片的 WaveData
        // 新建一个空的 WaveData
        waveControllers[0].WaveData = waveDatas[0] = new WaveData();
        // 取出链表中第一个节点
        var WALLNode = waveAttributes.First;
        // 对于每一个波参数组……
        for (int i = 0; i < waveAttributesCount; ++i) {
            // 在此 WaveData 的尾部加一个新的 WaveDataMask，传入该波参数
            waveDatas[0].AddMaskLast();
            waveDatas[0].AddAttributeLast(i, WALLNode.Value);
            // 步进链表节点指针
            WALLNode = WALLNode.Next;
        }

        // 配置分纸片的 WaveData
        // 取出链表中第一个和第二个节点
        WALLNode = waveAttributes.First;
        var WALLNodeNext = WALLNode.Next;
        // 对于每一个分纸片……
        for (int i = 1; i < papersCount; ++i) {
            // 先从总波参数组链表中临时移出此波参数组
            waveAttributes.Remove(WALLNode);

            // 用剩下的波参数组初始化一个新 WaveData
            waveControllers[i].WaveData = waveDatas[i] =
                new WaveData(waveAttributes);

            // 在此 WaveData 的尾部加一个新的 WaveDataMask，传入临时移出的波参数
            waveDatas[i].AddMaskLast();
            waveDatas[i].AddAttributeLast(1, WALLNode.Value);

            // 如果还不是是最后一个波参数组……
            if (WALLNodeNext != null) {
                // 将临时移出的波参数组添加回链表
                waveAttributes.AddBefore(WALLNodeNext, WALLNode);
                // 步进两个链表节点指针
                WALLNode = WALLNode.Next;
                WALLNodeNext = WALLNodeNext.Next;
            } else { // 如果已经是最后一个波参数组……
                // 将临时移出的波参数组添加回链表末尾
                waveAttributes.AddLast(WALLNode);
            }
        }

        // 关卡初始化完成，将数据引用传送给 FourierInputController
        FourierInputController fourierInputController = GetComponent<FourierInputController>();
        fourierInputController.SetDatas(
            paperData.waveAttributes, // 用于滤波、根据频率计算推进终点
            waveDatas, // 用于插值调整（分离）波形
            waveControllers, // 用于调用 WaveController.Refresh()
            papersParentTransform // 用于插值调整纸片组位置
        );

        // 激活 FourierInputController
        fourierInputController.enabled = true;
    }

    /// <summary>
    /// 根据 PaperData 的位置和高宽信息构造一个纸片
    /// </summary>
    /// <param name="paperData"> 至少包含纸片位置和高宽信息的纸片数据 </param>
    /// <returns> 返回新纸片对应的 WaveController脚本 </returns>
    /// <remarks> 纸片的 waveData 请之后单独设置 </remarks>
    private WaveController GetPaper(PaperData paperData) {
        // 实例化纸片
        Transform paperTransform = Instantiate(PaperPrefab, papersParentTransform).transform;
        paperTransform.localPosition = paperData.localPosition;
        //保存 WaveController
        WaveController waveController = paperTransform.GetComponent<WaveController>();

        // 设置纸片宽和高
        waveController.PaperWeight = paperData.paperWeight;
        waveController.PaperHeight = paperData.paperHeight;

        // 返回生成纸片的 WaveController脚本
        return waveController;
    }
}

/**

1、Unity EventSystem
3、处理输入并修改纸片
4、傅里叶变换部分

 */