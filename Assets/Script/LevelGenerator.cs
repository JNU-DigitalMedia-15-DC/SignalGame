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
        // 对目标纸片的第一个蒙版做目标修改
        // 注：因为用户只操作一个纸片，故只需要对一个 Mask 做修改，约定为第一个
        goal.ModifyByMask(0, levelData.modification);
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