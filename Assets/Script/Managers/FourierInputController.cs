using System;
using UnityEngine;

public class FourierInputController : MonoBehaviour {

    private const float rotateDelta = 10f;
    private const float subtractDelta = 1.5f;
    private const float pushDelta = 1f;

    private float[] marks;

    // 设备对微小操作的“不响应区域”的大小（半径）
    private const float deadZoneSize = 10f;

    // 划动开始的位置
    private Vector2 prevPos;
    // 是否正在划动（划动是否已经开始）
    private bool isSwiping = false;
    // 划动仍在 deadZone 范畴内
    private bool inDeadZone = true;
    private float totalDeltaX = 0f;
    private int waveCount = 3;

    enum OnePointPhase { Unassigned, Began, Ended }

    internal void SetDatas(
        PaperData[] papersData,
        WaveData waveData,
        WaveController waveController
    ) {
        waveCount = papersData[0].waveAttributes.Length;
    }

    private void Awake() {

        marks = new float[waveCount * 2 + 3];
        marks[0] = 0;
        marks[1] = rotateDelta;

        for (int i = 1; i < 4; ++i) {
            marks[i * 2] = marks[i * 2 - 1] + subtractDelta;
            marks[i * 2 + 1] = marks[i * 2] + pushDelta;
        }

        marks[8] = marks[7] + rotateDelta;
    }

    private void Update() {
        bool isTouching = false;
        Vector2 onePointPos = -Vector2.one;
        OnePointPhase onePointPhase = OnePointPhase.Unassigned;

        // #if UNITY_EDITOR || UNITY_STANDALONE
        // Unity Editor 或电脑端使用鼠标输入

        // 如果鼠标被点击……
        if (Input.GetMouseButton(0)) {
            isTouching = true;
            onePointPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(0)) {
            onePointPhase = OnePointPhase.Began;
        } else if (Input.GetMouseButtonUp(0)) {
            onePointPhase = OnePointPhase.Ended;
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
        if (isSwiping && isTouching) {
            Swipe(prevPos, onePointPos);
            prevPos = onePointPos;
        }

        // 终止划动
        // 安排在处理划动操作之后
        // 这样可以处理 TouchPhase.Began 之后紧接着 Ended Phase 的情况
        // （否则，isSwiping 会被设置为 false，于是这组 began-Ended 的处理便不会进行）
        if (!isTouching || onePointPhase == OnePointPhase.Ended) {
            isSwiping = false;
        }

        // 开始划动
        if (isTouching && onePointPhase == OnePointPhase.Began) {
            prevPos = onePointPos;
            isSwiping = true;
            inDeadZone = true;
        }
    }

    // 处理划动操作
    private void Swipe(Vector2 startPos, Vector2 onePointPos) {
        // 计算划动总位移矢量
        Vector2 diff = onePointPos - startPos;

        // // 如果上一帧仍在 deadZone 内而这一帧移出了
        // if (inDeadZone && diff.magnitude > deadZoneSize / 2) {
        //     // 判断此次划动是在修改 A 还是 Phi
        //     isChangingANotPhi = Mathf.Abs(diff.y) > Mathf.Abs(diff.x);

        //     // 标记此次 touch 已经脱离 deadZone
        //     inDeadZone = false;
        // }

        // // 如果已经不在 deadZone 内
        // if (!inDeadZone) {
        //     // 要修改 A 还是 Phi
        //     if (isChangingANotPhi) {
        //         waveModification.A = waveModificationOrigin.A *
        //             (diff.y * aZoomSpeed + 1);
        //     } else {
        //         waveModification.Phi = waveModificationOrigin.Phi +
        //             diff.x * phiTransSpeed;
        //     }
        //     // 立刻刷新纸片上波形
        //     RefreshPapers();
        // }
    }
}