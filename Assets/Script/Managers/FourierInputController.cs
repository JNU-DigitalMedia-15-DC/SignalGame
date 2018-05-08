using UnityEngine;

/// <summary> 傅里叶变换与滤波关卡的用户输入处理组件 </summary>
public class FourierInputController : MonoBehaviour {

    /// <summary> 完成“时域到正中”、“正中到频域”旋转操作需要的横向滑动总量 </summary>
    private const float rotateSwipe = 200f;
    /// <summary> 完成“波形分离”操作需要的横向滑动总量 </summary>
    private const float subtractSwipe = 200f;
    /// <summary> 完成“波形推进”操作需要的横向滑动总量 </summary>
    private const float pushSwipe = 200f;
    private const float EQSpeed = .001f;
    private const float maxEQ = 1.25f;

    /// <summary> deadZone半径（设备对微小操作的“不响应区域”的大小）） </summary>
    private const float deadZoneSize = 50f;
    /// <summary> 频域图的频率最大值 </summary>
    private const float maxOmega = 10f;
    /// <summary> z轴推进量的最大值 </summary>
    private const float maxZ = 10f;

    /// <summary> 上一帧结束时的用户输入点 </summary>
    private Vector2 prevInputPos = -Vector2.one;
    /// <summary> 纸片组Holder 的 Transform </summary>
    Transform papersParentTransform;
    private float rightLast;
    /// <summary> 上一帧结束时，用户的横向滑动量累和 所处的 操作区间 的 Index </summary>
    private int prevIntervalId = 0;
    /// <summary> 是否正在划动（划动是否已经开始） </summary>
    private bool isSwiping = false;
    /// <summary> 划动是否仍在 deadZone 范畴内 </summary>
    private bool inDeadZone = true;
    private bool isChangingEQ = false;
    /// <summary> 上一帧结束时，用户的横向滑动量累和 </summary>
    private float totalSwipeX = 0f;
    /// <summary> 这一帧开始时，用户的横向滑动量累和 </summary>
    private float newSwipeX;
    /// <summary> 波的参数组的总数 </summary>
    private int waveAttributesCount;
    /// <summary> totalSwipeX 和 newSwipeX 中左边的（数值较小的） </summary>
    private float leftSwipeX;
    /// <summary> totalSwipeX 和 newSwipeX 中右边的（数值较大的） </summary>
    private float rightSwipeX;
    /// <summary> 一个 WaveModification 的临时变量，只许在Subtract()里用 </summary>
    /// <remarks> 用于防止频繁 new WaveModification() </remarks>
    private WaveModification tempWaveMod = new WaveModification();
    /// <summary> 总纸片的所有波参数组 </summary>
    private WaveAttribute[] waveAttributes;
    /// <summary> 每个波参数组对应分纸片推进的终点的 z坐标 </summary>
    private float[] destinationZ;
    /// <summary> 纸片们的 WaveController们 </summary>
    private WaveController[] waveControllers;
    /// <summary> 纸片们 </summary>
    private GameObject[] papers;
    /// <summary> 纸片们的 Transform们 </summary>
    private Transform[] paperTransforms;
    /// <summary> 纸片们的 WaveData们 </summary>
    private WaveData[] waveDatas;
    private float[] originWaveA;

    /// <summary> 操作区间的左右边界 </summary>
    private float[] left, right;
    /// <summary> 操作区间总数 </summary>
    int intervalsCount;
    /// <summary> 摄像机是否已经正对频域（是否已经可以进行滤波操作） </summary>
    private bool facingFrequencyDomain = false;
    private float[] EQ = new float[] { 1, 1, 1 };
    private Vector2 startInputPos;

    /// <summary> 某个设备输入的输入阶段（刚刚开始，刚刚结束） </summary>
    enum InputPhase { Unassigned, Began, Ended }

    /// <summary>
    /// 设置 FourierInputController 需要用到的数据
    /// </summary>
    /// <param name="waveAttributes"> 总纸片的所有波参数组 </param>
    /// <param name="waveDatas"> 纸片们的 WaveData 们，用于插值调整（分离）波形 </param>
    /// <param name="waveControllers"> 纸片们的 WaveController 们，用于调用 WaveController.Refresh() </param>
    /// <param name="papersParentTransform"> 纸片组Holder 的 Transform </param>
    internal void SetDatas(
        WaveAttribute[] waveAttributes,
        WaveData[] waveDatas,
        WaveController[] waveControllers,
        Transform papersParentTransform
    ) {
        this.waveAttributes = waveAttributes;
        originWaveA = new float[waveAttributes.Length];
        int i = 0;
        foreach (var wa in waveAttributes) {
            originWaveA[i++] = wa.A;
        }
        destinationZ = new float[waveAttributes.Length];
        i = 0;
        foreach (WaveAttribute waveAttribute in waveAttributes) {
            destinationZ[i++] = Mathf.Lerp(0, maxZ,
                InverseLerpClamped(0, maxOmega,
                    waveAttribute.Omega)
            );
        }
        this.waveDatas = waveDatas;
        this.waveControllers = waveControllers;
        papers = new GameObject[waveControllers.Length];
        paperTransforms = new Transform[waveControllers.Length];
        i = 0;
        foreach (WaveController waveController in waveControllers) {
            papers[i] = waveController.gameObject;
            paperTransforms[i++] = waveController.GetComponent<Transform>();
        }

        this.papersParentTransform = papersParentTransform;
        this.waveAttributesCount = waveAttributes.Length;

        // 预处理操作区间边界
        // 计算操作区间总数，每个波参数组对应两个操作区间，外加开始和最后两个旋转操作区间
        intervalsCount = waveAttributesCount * 2 + 2;

        // 分配内存空间
        left = new float[intervalsCount];
        right = new float[intervalsCount];

        // 设置第一个旋转操作区间的边界
        left[0] = 0;
        right[0] = rotateSwipe;

        // 设置每一个波参数组对应操作区间，i从1开始，总共waveAttributesCount次
        for (i = 1; i < waveAttributesCount + 1; ++i) {
            // 由 1*2-1 == 1 知，第一个操作区间的index为 i*2-1，第二个为 i*2
            int first = i * 2 - 1, second = i * 2;
            left[first] = right[first - 1];
            right[first] = left[second] = left[first] + subtractSwipe;
            right[second] = left[second] + pushSwipe;
        }

        /// <summary> 最后一个操作区间的index </summary>
        int last = intervalsCount - 1;
        // 设置最后一个旋转操作区间的边界
        left[last] = right[last - 1];
        right[last] = left[last] + rotateSwipe;
        rightLast = right[last];
    }

    // 处理输入
    private void Update() {
        /// <summary> 设备上第一个输入点的位置 </summary>
        Vector2 firstInputPos = -Vector2.one;
        /// <summary> 设备上第一个输入点的输入阶段 </summary>
        InputPhase firstInputPhase = InputPhase.Unassigned;

        // #if UNITY_EDITOR || UNITY_STANDALONE
        // Unity Editor 或电脑端使用鼠标输入

        // 如果鼠标左键……
        firstInputPos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0)) {
            firstInputPhase = InputPhase.Began;
        } else if (Input.GetMouseButtonUp(0)) {
            firstInputPhase = InputPhase.Ended;
        }

        // #else
        // // 移动端使用 touch 输入

        // // 单点触控
        // if (isTouching = (Input.touchCount >= 1)) {
        //     onePointPos = Input.GetTouch(0).position;
        // }
        // if (Input.GetTouch(0).phase == TouchPhase.Began) {
        //     onePointPhase = OnePointPhase.Began;
        // } else if (Input.GetTouch(0).phase == TouchPhase.Ended) {
        //     onePointPhase = OnePointPhase.Ended;
        // }

        // #endif

        // 仍在划动
        if (isSwiping) {
            Swipe(startInputPos, prevInputPos, firstInputPos);
            prevInputPos = firstInputPos;
        }

        // 终止划动
        // 安排在处理划动操作之后
        // 这样可以处理 TouchPhase.Began 之后紧接着 Ended Phase 的情况
        // （否则，isSwiping 会被设置为 false，于是这组 began-Ended 的处理便不会进行）
        if (firstInputPhase == InputPhase.Ended) {
            isSwiping = false;
        }

        // 开始划动
        if (firstInputPhase == InputPhase.Began) {
            startInputPos = firstInputPos;
            prevInputPos = firstInputPos;
            isSwiping = true;
            inDeadZone = true;
        }
    }

    /// <summary> 处理划动操作 </summary>
    /// <param name="startInputPos"> 划动开始时的输入点位置 </param>
    /// <param name="prevInputPos"> 上一帧结束时输入点的位置 </param>
    /// <param name="inputPos"> 这一帧开始时输入点的位置 </param>
    private void Swipe(Vector2 startInputPos, Vector2 prevInputPos, Vector2 inputPos) {
        // 计算划动总位移矢量
        Vector2 diff = inputPos - startInputPos;
        // 如果已经正对频域……
        if (facingFrequencyDomain) {
            // 如果上一帧仍在 deadZone 内而这一帧移出了
            if (inDeadZone && diff.magnitude > deadZoneSize / 2) {
                // 判断是调的是否为EQ
                isChangingEQ = Mathf.Abs(diff.y) > Mathf.Abs(diff.x);

                // 标记此次 touch 已经脱离 deadZone
                inDeadZone = false;
            }

            // 如果已经不在 deadZone 内
            if (!inDeadZone) {
                // 是否在修改EQ
                if (isChangingEQ) {
                    int EQId = System.Convert.ToInt32(Mathf.Floor(startInputPos.x * 3 / Screen.width));
                    float deltaY = inputPos.y - prevInputPos.y;
                    EQ[EQId] = Mathf.Clamp(EQ[EQId] + deltaY * EQSpeed, 0, maxEQ);
                    Debug.Log(EQ[EQId]);

                    int right = System.Convert.ToInt32(
                        (EQId + 1) * waveAttributesCount / 3f
                    );
                    for (int i = System.Convert.ToInt32(EQId * waveAttributesCount / 3f); i < right; ++i) {
                        waveAttributes[i].A = originWaveA[i] * EQ[EQId];
                        waveControllers[i + 1].Refresh();
                    }
                }
            }
        }

        // if (!facingFrequencyDomain || (!inDeadZone && !isChangingEQ)) {
        if (!(facingFrequencyDomain && (inDeadZone || isChangingEQ))) {
            /// <summary> 两帧间划动位移的横坐标 </summary>
            float deltaX = -(inputPos.x - prevInputPos.x);

            // 更新各个**SwipeX
            newSwipeX = Mathf.Clamp(totalSwipeX + deltaX, -1f, rightLast + 1f);
            leftSwipeX = Mathf.Min(totalSwipeX, newSwipeX);
            rightSwipeX = Mathf.Max(totalSwipeX, newSwipeX);

            /// <summary> 是否在向左划动 </summary>
            bool isSwipingLeft = deltaX < 0;

            // 处理划动此次划动涉及的所有操作区间
            do {
                // 利用反插值函数处理prevIntervalId所示操作区间
                LerpInterval(prevIntervalId, newSwipeX);
                // 尝试步进区间index，如果成功进入新操作区间，继续循环
            } while (AdvanceInterval(ref prevIntervalId, isSwipingLeft));

            // 划动处理结束，更新 totalSwipeX
            totalSwipeX = newSwipeX;
        }
    }

    /// <summary>
    /// 尝试步进区间Id，并返回是否应该继续步进
    /// </summary>
    /// <param name="intervalId"> 要步进的Id </param>
    /// <param name="isGoingLeft"> 是否向左步进（减小） </param>
    /// <returns> 返回是否应该继续步进 </returns>
    private bool AdvanceInterval(ref int intervalId, bool isGoingLeft) {
        /// <summary> 下一个区间的Id </summary>
        int nextIntervalId = intervalId + (isGoingLeft ? -1 : 1);

        // 如果步进Id无效，返回 false
        if (nextIntervalId < 0 || nextIntervalId >= intervalsCount) {
            return false;
        }

        // 如果步进后操作区间与划动区间无交集，返回 false
        if (right[nextIntervalId] < leftSwipeX ||
            rightSwipeX < left[nextIntervalId]) {
            return false;
        }

        // 测试成功，步进区间Id，返回true
        intervalId = nextIntervalId;
        return true;
    }

    /// <summary> 先钳制版的反插值函数 </summary>
    private float InverseLerpClamped(float a, float b, float value) {
        value = Mathf.Clamp(value, a, b);
        return Mathf.InverseLerp(a, b, value);
    }

    /// <summary>
    /// 利用反插值函数处理区间
    /// </summary>
    /// <param name="intervalId"> 待处理的区间Id </param>
    /// <param name="SwipeX"> 划动操作的横坐标 </param>
    private void LerpInterval(int intervalId, float SwipeX) {
        // 求出划动操作在操作区间的反插值
        float inverseLerp = InverseLerpClamped(left[intervalId], right[intervalId], SwipeX);

        // 如果是第一个划动操作区间……
        if (intervalId == 0) {
            // ……进行时域到正中的旋转
            firstRotate(inverseLerp);
            return;
        }
        // 如果是最后一个划动操作区间……
        if (intervalId == waveAttributesCount * 2 + 1) {
            // ……进行正中到频域的旋转
            lastRotate(inverseLerp);
            return;
        }

        // 否则是分离或推动操作区间
        /// <summary> 操作区间对应的波参数组 index </summary>
        /// <remarks> intervalId 减去第一个旋转操作区间再除以 2 </remarks>
        int waveAttributeId = (intervalId - 1) / 2;
        // 如果是分离操作区间……
        if (intervalId % 2 == 1) {
            // ……进行分离操作
            Subtract(waveAttributeId, inverseLerp);
        } else { // ……否则是推动操作区间……
            // ……进行推动操作
            Push(waveAttributeId, inverseLerp);
        }
    }

    /// <summary> 利用插值函数处理时域与正中之间的旋转 </summary>
    private void firstRotate(float inverseLerp) { // TODO
        papersParentTransform.rotation = Quaternion.identity;
        papersParentTransform.Rotate(Vector3.right * Mathf.Lerp(0f, -30f, inverseLerp));
        papersParentTransform.Rotate(Vector3.up * Mathf.Lerp(0f, 45f, inverseLerp));
    }

    /// <summary> 利用插值函数处理正中与频域之间的旋转 </summary>
    private void lastRotate(float inverseLerp) { // TODO
        // 如果从正中视角切换到正对频域视角……
        if (facingFrequencyDomain == false && inverseLerp >.99f) {
            inverseLerp = 1f;
            // 标记为已经正对频域
            facingFrequencyDomain = true;
            // 将总纸片设置为未激活
            papers[0].SetActive(false);
            isSwiping = false;
        }
        // 如果从正对频域视角切换到正中视角
        if (facingFrequencyDomain == true && inverseLerp < .9f) {
            // 标记为不正对频域
            facingFrequencyDomain = false;
            // 将总纸片设置为激活
            papers[0].SetActive(true);
            waveControllers[0].Refresh();
        }
        papersParentTransform.rotation = Quaternion.Euler(Vector3.up * 90);
        papersParentTransform.Rotate(Vector3.forward * Mathf.Lerp(0f, -30f, 1 - inverseLerp));
        papersParentTransform.Rotate(Vector3.up * Mathf.Lerp(0f, -45f, 1 - inverseLerp));
    }

    /// <summary> 将单个正弦波从总波分离出来 </summary>
    private void Subtract(int waveAttributeId, float inverseLerp) {
        int paperId = waveAttributeId + 1;

        // // 如果分离刚刚开始一丢丢，设置纸片为未激活
        // if (inverseLerp < .01f) {
        //     papers[paperId].SetActive(false);
        //     return;
        // }

        // // 激活纸片
        // papers[paperId].SetActive(true);

        tempWaveMod.A = 1 - inverseLerp;
        // 处理总波
        // waveDatas[0].SetWaveModification(waveAttributeId, tempWaveMod);
        // waveControllers[0].Refresh();

        // 处理分波
        waveDatas[paperId].SetWaveModification(0, tempWaveMod);
        waveControllers[paperId].Refresh();
    }

    /// <summary> 将单个正弦波纸片向频率更高的位置推进 </summary>
    private void Push(int waveAttributeId, float inverseLerp) {
        float z = Mathf.Lerp(0, destinationZ[waveAttributeId], inverseLerp);
        int paperId = waveAttributeId + 1;
        paperTransforms[paperId].localPosition = Vector3.forward * z;
    }
}