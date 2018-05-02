using UnityEngine;

public class WaveInputController : MonoBehaviour {

    enum OnePointPhase { Unassigned, Began, Ended }

    // 设备对微小操作的“不响应区域”的大小（半径）
    private const float deadZoneSize = .01f;
    // 对 A 的修改的乘数
    private const float aZoomSpeed = 1f;
    // 对 Omega 的修改的乘数
    private const float mouseScrollSpeed = 1f;
    // 对 Phi 的修改的乘数
    private const float phiTransSpeed = 1f;

    // 主相机
    private Camera mainCamera;
    // 纸片们的边界们的世界坐标，顺序依次为：左右下上
    private float[, ] papersBounds = new float[2, 4];
    // 所有可以被修改的纸片的 WaveModification
    private WaveModification[] waveModifications = new WaveModification[2];
    // 划动开始的位置
    private Vector2 startPos;
    // 是否正在划动（划动是否已经开始）
    private bool isSwiping = false;
    // 划动仍在 deadZone 范畴内
    private bool inDeadZone = true;
    // 要更改的 WaveModification
    private WaveModification waveModification;
    // 手势操作开始前 WaveModification 的初态
    private WaveModification originWaveModification;
    // 划动是在修改 A 还是 Phi
    private bool changingANotPhi;
    // 是否正在捏合（捏合是否已经开始）
    private bool isPinching = false;
    // 捏合手势开始时，俩touch之间的距离（缺省值应该和 deadZoneSize 一致）
    private float originTouchDeltaMag = .01f;

    /// <summary>
    /// 帮助 WaveInputController 设置 raycast 所用信息
    /// </summary>
    /// <param name="papersData"> 纸片们的源数据 </param>
    /// <param name="waveDatas"> 实例化后纸片们的 waveData </param>
    internal void SetDatas(PaperData[] papersData, WaveData[] waveDatas) {
        // 计算每个纸片的边界，顺序：左右下上
        for (int i = 0; i < 2; ++i) {
            papersBounds[i, 0] = papersData[i].position.x;
            papersBounds[i, 1] = papersData[i].position.x + papersData[i].paperWeight;
            papersBounds[i, 2] = papersData[i].position.y - papersData[i].paperHeight;
            papersBounds[i, 3] = papersData[i].position.y;

            // 记录 纸片对应WaveModification
            waveModifications[i] = waveDatas[i].GetWaveModificationPrototype();
        }
    }

    private void Awake() {
        // 获取主相机
        mainCamera = Camera.main;
    }

    private void Update() {
        int touchCount = 0;
        Vector2 onePointPos = Vector2.zero;
        OnePointPhase onePointPhase = OnePointPhase.Unassigned;

        // #if UNITY_EDITOR || UNITY_STANDALONE
        // Unity Editor 或电脑端使用鼠标输入

        // 如果鼠标被点击……
        if (Input.GetMouseButton(0)) {
            touchCount = 1;
            onePointPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(0)) {
            onePointPhase = OnePointPhase.Began;
        } else if (Input.GetMouseButtonUp(0)) {
            onePointPhase = OnePointPhase.Ended;
        }

        // 如果鼠标滚轮被滚动，获取鼠标滚轮纵向滚动量
        float mouseScrollY = Input.mouseScrollDelta.y * mouseScrollSpeed;
        // 如果鼠标滚轮向上滚动
        if (mouseScrollY >.01f) {
            waveModification.Omega = originWaveModification.Omega / mouseScrollY;
        }
        // 如果鼠标滚轮向下滚动
        if (mouseScrollY < -.01f) {
            waveModification.Omega = originWaveModification.Omega * -mouseScrollY;
        }

        // #else
        // // 移动端使用 touch 输入

        // // 单点触控
        // if ((touchCount = Input.touchCount) == 1) {
        //     onePointPos = Input.GetTouch(0).position;
        // }
        // if (Input.GetTouch(0).phase == TouchPhase.Began) {
        //     onePointPhase = OnePointPhase.Began;
        // } else if (Input.GetTouch(0).phase == TouchPhase.Ended) {
        //     onePointPhase = OnePointPhase.Ended;
        // }

        // // 双点触控
        // // 终止捏合
        // if (isPinching && Input.touchCount != 2) {
        //     isPinching = false;
        // }

        // // 处理捏合
        // if (Input.touchCount == 2) {
        //     // 记录 两个touch
        //     Touch touchZero = Input.GetTouch(0);
        //     Touch touchOne = Input.GetTouch(1);

        //     // 计算 两个touch 间的向量 的 模长（距离）
        //     float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        //     // 确保模长不小于 deadZoneSize，以防止过大幅度的变化和不必要的错误
        //     touchDeltaMag = Mathf.Max(touchDeltaMag, deadZoneSize);

        //     // 如果已经开始捏合
        //     if (isPinching) {
        //         // 计算两帧间 Omega（应有）的变化量
        //         float deltaOmegaDiff = originTouchDeltaMag / touchDeltaMag;

        //         // 套用 Omega的变化量
        //         waveModification.Omega = originWaveModification.Omega * deltaOmegaDiff;
        //     } else if ( // 判断双指是否都在同一个纸片上
        //         (waveModification = FindWaveModByScreenPos(onePointPos)) != null &&
        //         waveModification == FindWaveModByScreenPos(Input.GetTouch(1).position)
        //     ) {
        //         // 初始化新捏合
        //         originWaveModification = new WaveModification(waveModification);
        //         originTouchDeltaMag = touchDeltaMag;
        //         isPinching = true;
        //     }
        // }

        // #endif

        // 仍在划动
        if (isSwiping && touchCount == 1) {
            Swipe(startPos, onePointPos);
        }

        // 终止划动
        // 安排在处理划动操作之后
        // 这样可以处理 TouchPhase.Began 之后紧接着 Ended Phase 的情况
        // （否则，isSwiping 会被设置为 false，于是这组 began-Ended 的处理便不会进行）
        if (touchCount != 1 || onePointPhase == OnePointPhase.Ended) {
            isSwiping = false;
        }

        // 开始划动
        if (touchCount == 1 && onePointPhase == OnePointPhase.Began) {
            waveModification = FindWaveModByScreenPos(onePointPos);
            if (waveModification != null) {
                originWaveModification = new WaveModification(waveModification);
                startPos = onePointPos;
                isSwiping = true;
                inDeadZone = true;
            }
        }
    }

    // 处理划动操作
    private void Swipe(Vector2 startPos, Vector2 onePointPos) {
        // 计算划动总位移矢量
        Vector2 diff = onePointPos - startPos;

        // 如果上一帧仍在 deadZone 内而这一帧移出了
        if (inDeadZone && diff.magnitude > deadZoneSize / 2) {
            // 判断此次划动是在修改 A 还是 Phi
            changingANotPhi = Mathf.Abs(diff.y) > Mathf.Abs(diff.x);

            // 标记此次 touch 已经脱离 deadZone
            inDeadZone = false;
        }

        // 如果已经不在 deadZone 内
        if (!inDeadZone) {
            // 要修改 A 还是 Phi
            if (changingANotPhi) {
                waveModification.A = originWaveModification.A *
                    (diff.y * aZoomSpeed + 1);
            } else {
                waveModification.Phi = originWaveModification.Phi +
                    diff.x * phiTransSpeed;
            }
        }
    }

    // 根据屏幕坐标寻找纸片的 WaveModification
    private WaveModification FindWaveModByScreenPos(Vector2 screenPoint) {
        // 将 屏幕坐标 转换到 世界坐标
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(
            screenPoint.x,
            screenPoint.y
        ));

        // 如果坐标落在某个 可修改纸片 的内部，则即刻返回 此纸片对应的WaveModification
        for (int i = 0; i < 2; ++i) {
            if (papersBounds[i, 0] < worldPoint.x && worldPoint.x < papersBounds[i, 1] &&
                papersBounds[i, 2] < worldPoint.y && worldPoint.y < papersBounds[i, 3]) {
                return waveModifications[i];
            }
        }

        // 否则返回 null 表示未找到
        return null;
    }
}