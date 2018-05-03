using System;
using UnityEngine;

public class FourierInputController : MonoBehaviour {

    private const float rotateSwipe = 10f;
    private const float subtractSwipe = 1.5f;
    private const float pushSwipe = 1f;

    // 设备对微小操作的“不响应区域”的大小（半径）
    private const float deadZoneSize = 10f;

    // 划动开始的位置
    private float prevX = -10f;
    Transform papersParentTransform;
    private int prevIntervalId = -1;
    private WaveData.WaveDataNode waveDataNode;
    // 是否正在划动（划动是否已经开始）
    private bool isSwiping = false;
    // 划动仍在 deadZone 范畴内
    private bool inDeadZone = true;
    private float totalSwipeX = 0f;
    private float newSwipeX;
    private int waveAttributesCount = 3;
    private float leftSwipeX;
    private float rightSwipeX;
    private WaveController[] waveControllers;
    private WaveData[] waveDatas;

    // 区间的左右边界
    private float[] left, right;

    enum OnePointPhase { Unassigned, Began, Ended }

    internal void SetDatas(
        WaveData[] waveDatas,
        WaveController[] waveControllers,
        int wavesCount,
        Transform papersParentTransform
    ) {
        this.waveDatas = waveDatas;
        this.waveControllers = waveControllers;
        this.waveAttributesCount = wavesCount - 1;
        this.papersParentTransform = papersParentTransform;
        waveDataNode = waveDatas[0].GetWaveDataNodePrototype();
    }

    private void Awake() {
        left = new float[waveAttributesCount * 2 + 2];
        right = new float[waveAttributesCount * 2 + 2];

        left[0] = 0;
        right[0] = rotateSwipe;

        for (int i = 1; i < waveAttributesCount + 1; ++i) {
            int first = i * 2 - 1, second = i * 2;
            left[first] = right[first - 1];
            right[first] = left[second] = left[first] + subtractSwipe;
            right[second] = left[second] + pushSwipe;
        }

        int last = waveAttributesCount * 2 + 1;
        left[last] = right[last - 1];
        right[last] = left[last] + rotateSwipe;
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
            Swipe(onePointPos);
            prevX = onePointPos.x;
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
            prevX = onePointPos.x;
            isSwiping = true;
            inDeadZone = true;
        }
    }

    // 处理划动操作
    private void Swipe(Vector2 onePointPos) {
        // 计算划动总位移矢量
        float diff = onePointPos.x - prevX;

        newSwipeX = totalSwipeX + diff;

        leftSwipeX = Mathf.Min(totalSwipeX, newSwipeX);
        rightSwipeX = Mathf.Max(totalSwipeX, newSwipeX);

        int step = diff < 0 ? -1 : 1;

        while (CheckInterval(prevIntervalId + step)) {
            prevIntervalId += step;
        }

        totalSwipeX = newSwipeX;

        // if (newSwipeX == marks[nextMarkId])
        //     newSwipeX += 0.01f * (direction == 0 ? -1 : 1);

        // while (leftSwipeX < marks[nextMarkId] && marks[nextMarkId] < rightSwipeX) {
        //     if (nextMarkId == 0) {
        //         leftRotate(1);
        //     }
        //     if (nextMarkId == waveCount * 2 + 1) {
        //         rightRotate(1);
        //         newSwipeX += 1f;
        //     }
        // }
    }

    private bool CheckInterval(int intervalId) {
        if (right[intervalId] < leftSwipeX ||
            rightSwipeX < left[intervalId]) {
            return false;
        }

        LerpInterval(intervalId);

        return true;
    }

    private void LerpInterval(int intervalId) {
        float inverseLerp = Mathf.InverseLerp(left[intervalId], right[intervalId], newSwipeX);
        if (intervalId == 0) {
            firstRotate(inverseLerp);
            return;
        }
        if (intervalId == waveAttributesCount * 2 + 1) {
            lastRotate(inverseLerp);
            return;
        }

        int waveId = (intervalId - 1) / 2;
        if (intervalId % 1 == 1) {
            Subtract(waveId, inverseLerp);
        } else {
            Push(waveId, inverseLerp);
        }
    }

    private void firstRotate(float inverseLerp) {
        papersParentTransform.rotation = Quaternion.identity;
        papersParentTransform.Rotate(Vector3.right * Mathf.Lerp(0f, -30f, inverseLerp));
        papersParentTransform.Rotate(Vector3.up * Mathf.Lerp(0f, 45f, inverseLerp));
    }

    private void lastRotate(float inverseLerp) {
        papersParentTransform.rotation = Quaternion.Euler(Vector3.up * 90);
        papersParentTransform.Rotate(Vector3.right * Mathf.Lerp(0f, -30f, 1 - inverseLerp));
        papersParentTransform.Rotate(Vector3.up * Mathf.Lerp(0f, -45f, 1 - inverseLerp));
        if (inverseLerp >.99f) {
            newSwipeX += 1f;
        }
    }

    private void Subtract(int waveId, float inverseLerp) {
        waveDatas[0].GetWaveDataNodePrototype();
        waveDatas[waveId + 1].GetWaveDataNodePrototype();
    }

    private void Push(int waveId, float inverseLerp) {
        throw new NotImplementedException();
    }

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