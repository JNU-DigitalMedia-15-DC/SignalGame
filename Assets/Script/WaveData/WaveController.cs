using System;
using UnityEngine;

/// <summary>
/// WaveController 脚本用于控制其 GameObject 的 LineRenderer 中点的数据
/// </summary>
/// <remarks> 使用时请设置好 waveData </remarks>
public class WaveController : MonoBehaviour {
    /// <summary> 初始化时点的横坐标的间隔 </summary>
    private const float deltaX = .02f;

    /// <summary> 波形显示对象 Waveform 的 LineRenderer </summary>
    public LineRenderer lineRenderer;
    /// <summary> 波形所在的纸片的 transform </summary>
    public Transform paperTransform;
    /// <summary> 点坐标数据的缓冲区 </summary>
    private static Vector3[] positions;
    /// <summary> 初始化和刷新时点的总数 </summary>
    private int initialPositionCount;
    /// <summary> 波形展示区的高度 </summary>
    private float paperHeight = 1;
    /// <summary> 波形展示区的宽度 </summary>
    private float paperWeight = 2;
    /// <summary> 当前 WaveController 所采用的波形数据源 </summary>
    private WaveData waveData;

    /// <summary> 当前 WaveController 所采用的波形数据源 </summary>
    internal WaveData WaveData {
        get { return waveData;}
        set { waveData = value; }
    }

    /// <summary> 波形展示区的高度 </summary>
    internal float PaperHeight {
        get { return paperHeight; }
        set { paperHeight = value; }
    }

    /// <summary> 波形展示区的宽度 </summary>
    internal float PaperWeight {
        get { return paperWeight; }
        set { paperWeight = value; }
    }

    /// <summary> 返回波形展示区的边界坐标数组（左右下上） </summary>
    internal float[] GetPaperBound() {
        return new float[] {
            transform.position.x,
            transform.position.x + paperWeight,
            transform.position.y - paperHeight,
            transform.position.y,
        };
    }

    private void Start() {
        // 清空 LineRenderer 原有点集，并设置为使用本地空间
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = false;

        // 计算 Paper 对象的 Transform
        paperTransform.localScale =
            new Vector3(paperWeight, paperHeight, 0);
        paperTransform.localPosition =
            new Vector3(paperWeight / 2, -paperHeight / 2, 0);

        // 计算 Waveform 对象的 Transform
        lineRenderer.GetComponent<Transform>().localPosition =
            new Vector3(0, -paperHeight / 2, 0);

        // 计算初始化时点的总数
        // 注：Awake()时 paperWeight 仍然未被 LevelGenerator 设置，
        // 故必须等到 Start() 时计算点数
        initialPositionCount =
            Convert.ToInt32(Math.Ceiling(paperWeight / deltaX));

        // 初始化 LineRenderer
        InitializeLineRender();
    }

    private void Update() {
        // 取出原本在 LineRenderer 中的点
        // TODO：这里是不是有更好的办法？
        //       比如把 positions 的结构换成 LinkedList 然后映射 + 反映射？
        int positionCount = lineRenderer.positionCount;
        ExtendBufferSize(positionCount + 1);
        lineRenderer.GetPositions(positions);

        // 将之前 Line 中点的坐标更新，并且检测新坐标是否超出范围
        // 如果发现新坐标超出范围，则停止更新，并计算 positionCount
        // (如果均未超出范围（尽管不应如此），则保留原本的 positionCount)
        for (int i = 0; i < positionCount; ++i)
            if ((positions[i].x += Time.deltaTime) > paperWeight) {
                // 将多出的部分“截掉”
                positions[i].y =
                    positions[i - 1].y +
                    (paperWeight - positions[i - 1].x) *
                    (positions[i].y - positions[i - 1].y) /
                    (positions[i].x - positions[i - 1].x);
                positions[i].x = paperWeight;
                positionCount = i + 1; // 即：下标为i的点，是要（保留）的
                break;
            }

        // 将点顺移一个下标
        for (int i = positionCount; i > 0; --i)
            positions[i] = positions[i - 1];

        // 计算新点的坐标，加入存储数组
        float y = WaveFunction(Time.time);
        positions[0] = new Vector3(0, y, 0);
        ++positionCount;

        // 更新 LineRenderer
        lineRenderer.positionCount = positionCount;
        lineRenderer.SetPositions(positions);
    }

    /// <summary> 立即重新初始化 LineRenderer </summary>
    internal void Refresh() {
        // 清空原有点数据
        lineRenderer.positionCount = 0;
        // 重新计算并更新点
        CalcAndSetPoints(initialPositionCount);
    }

    /// <summary> 初始化 LineRenderer </summary>
    private void InitializeLineRender() {
        // 初始化点位置数组
        positions = new Vector3[initialPositionCount];
        // 重新计算并更新点
        CalcAndSetPoints(initialPositionCount);
    }

    /// <summary> 初始化或刷新时计算并设置点的位置 </summary>
    private void CalcAndSetPoints(int positionCount) {
        // 计算初始波形上各点的位置， t(x) = t0 - x
        for (int i = 0; i < positionCount; ++i)
            positions[i] =
            new Vector3(i * deltaX, WaveFunction(Time.time - i * deltaX), 0);

        // 更新到 LineRenderer
        lineRenderer.positionCount = positionCount;
        lineRenderer.SetPositions(positions);
    }

    /// <summary>调整 点坐标数据缓冲区positions 到可以容下 size 的大小 </summary>
    private static void ExtendBufferSize(int size) {
        // 注：需要 positions 已分配内存空间
        // 检查点坐标数据缓冲区大小是否小于 size
        if (positions.Length < size)
            // 并调整大小到能容纳的下一个 2 的幂
            Array.Resize(ref positions, Mathf.NextPowerOfTwo(size));
    }

    /// <summary> 波形函数：众正弦函数叠加 </summary>
    private float WaveFunction(float x) {
        return waveData.ReturnValueAt(x);
    }
}