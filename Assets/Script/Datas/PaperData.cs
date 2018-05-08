using UnityEngine;

/// <summary> 单张纸片的数据对象 </summary>
[System.Serializable]
public class PaperData {
    /// <summary> 波形展示区的宽度 </summary>
    public float paperWeight;
    /// <summary> 波形展示区的高度 </summary>
    public float paperHeight;
    /// <summary> 波形展示区的左上角的本地坐标 </summary>
    public Vector3 localPosition;
    /// <summary> 波形初始数据表 </summary>
    public WaveAttribute[] waveAttributes;
}