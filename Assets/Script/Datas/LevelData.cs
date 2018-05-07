/// <summary> 关卡的数据对象 </summary>
[System.Serializable]
public class LevelData {
    /// <summary> 纸片组 Holder 的位置（纸片组的左上角） </summary>
    public UnityEngine.Vector3 HolderPosition;
    /// <summary> 关卡内的多个纸片数据对象 </summary>
    public PaperData[] papersData;
    /// <summary> 用户通关需要做的修改 </summary>
    public WaveModification[] modifications;
}