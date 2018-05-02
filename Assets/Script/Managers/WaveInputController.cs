using UnityEngine;

public class WaveInputController : MonoBehaviour {

    // 要更改的 WaveModification
    WaveModification waveModification;
    // WaveModification 的初态
    WaveModification originWaveModification;

    // 设备对微小操作的“不响应区域”的大小（半径）
    private const float deadZoneSize = .01f;
    // 对 A 的修改的乘数
    private const float aZoomSpeed = 1f;
    // 对 Phi 的修改的乘数
    private const float phiTransSpeed = 1f;

    // 划动开始的位置
    private Vector2 startPos;
    // 是否正在划动（划动是否已经开始）
    private bool isSwiping = false;
    // 划动仍在 deadZone 范畴内
    private bool inDeadZone = true;
    // 划动是在修改 A 还是 Phi
    private bool changingANotPhi;

    // #if !UNITY_STANDALONE
    // #endif

    private void Update() {
        // #if UNITY_EDITOR || UNITY_STANDALONE
        // // Use key input in editor or standalone
        // if (Input.GetKeyDown(KeyCode.LeftArrow)) {
        //     ChangeLane(-1);
        // } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
        //     ChangeLane(1);
        // } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
        //     Jump();
        // } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
        //     if (!m_Sliding)
        //         Slide();
        // }
        // #else

        // 移动端使用 touch 输入
        // 如果设备收到 一个touch ……
        if (Input.touchCount == 1) {
            // 如果已经开始滑动……
            if (isSwiping) {
                // 计算触点总位移矢量
                Vector2 diff = Input.GetTouch(0).position - startPos;

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

            // phase的检查 安排在处理划动操作之后
            // 这样可以处理 TouchPhase.Began 之后紧接着 Ended Phase 的情况
            // （否则，isSwiping 会被设置为 false，于是这组 began-Ended 的处理便不会进行）
            if (Input.GetTouch(0).phase == TouchPhase.Began) {
                startPos = Input.GetTouch(0).position;
                isSwiping = true;
                inDeadZone = true;
            } else if (Input.GetTouch(0).phase == TouchPhase.Ended) {
                isSwiping = false;
            }
        }

        // 如果设备收到 两个touch ……
        if (Input.touchCount == 2) {
            // 记录 两个touch
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // 计算 每个 touch 前一帧的 position
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // 计算 每个touch 两帧间的向量 的 模长（距离）
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 确保模长不小于 deadZoneSize，以防止过大幅度的变化和不必要的错误
            prevTouchDeltaMag = Mathf.Max(prevTouchDeltaMag, deadZoneSize);
            touchDeltaMag = Mathf.Max(touchDeltaMag, deadZoneSize);

            // 计算两帧间 Omega（应有）的变化量
            float deltaOmegaDiff = prevTouchDeltaMag / touchDeltaMag;

            // 套用 Omega的变化量
            waveModification.Omega *= deltaOmegaDiff;
        }
        // #endif
    }
}