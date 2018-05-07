using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSystem;

public class WaveInputController : MonoBehaviour {

    enum OnePointPhase { Unassigned, Began, Ended }

    // 设备对微小操作的“不响应区域”的大小（半径）
    private const float deadZoneSize = 10f;
    // 对 A 的修改的乘数
    private const float aZoomSpeed = .025f;
    // 对 Omega 的修改的乘数
    private const float mouseScrollSpeed = .025f;
    // 对 Phi 的修改的乘数
    private const float phiTransSpeed = .025f;

    // 主相机
    private Camera mainCamera;
    // 纸片们的边界们的世界坐标，顺序依次为：左右下上
    private float[][] papersBounds = new float[2][];
    // 用户对纸片们的修改们（WaveModification们）（初始化为（1，1，0））
    private WaveModification[] waveModifications = new WaveModification[2] {
        new WaveModification(), new WaveModification()
    };
    // 所有可以被修改的纸片的 WaveController
    private WaveController[] waveControllers = new WaveController[4];
    // 所有可以被修改的纸片的 WaveData
    private WaveData[] waveDatas = new WaveData[2];
    // 划动开始的位置
    private Vector2 startPos;
    // 是否正在划动（划动是否已经开始）
    private bool isSwiping = false;
    private bool isCheckContinue = true;
    // 划动仍在 deadZone 范畴内
    private bool inDeadZone = true;
    // 要更改的 WaveModification
    private WaveModification waveModification;
    // 要应用更改的 WaveData
    private WaveData waveData;
    // 手势操作开始前 WaveModification 的初态
    private WaveModification waveModificationOrigin = new WaveModification();
    // 要更改的 WaveController
    private WaveController waveController;
    // 划动是在修改 A 还是 Phi
    private bool isChangingANotPhi;
    // 是否正在捏合（捏合是否已经开始）
    private bool isPinching = false;
    // 捏合手势开始时，俩touch之间的距离（缺省值应该和 deadZoneSize 一致）
    private float touchDeltaMagOrigin = .01f;

    /// <summary>
    /// 帮助 WaveInputController 设置 raycast 所用信息
    /// </summary>
    /// <param name="papersData"> 纸片们的源数据 </param>
    /// <param name="waveDatas"> 实例化后纸片们的 waveData </param>
    /// <param name="waveControllers"> 实例化后纸片们的 WaveController </param>
    internal void SetDatas(
        PaperData[] papersData,
        WaveData[] waveDatas,
        WaveController[] waveControllers
    ) {
        for (int i = 0; i < 2; ++i) {
            // 获取纸片的边界，顺序：左右下上
            papersBounds[i] = waveControllers[i].GetPaperBound();

            // 记录纸片对应的 WaveData 和 WaveController
            this.waveDatas[i] = waveDatas[i];
            this.waveControllers[i] = waveControllers[i];
        }
        for (int i = 2; i < 4; ++i) {
            // 继续记录 纸片对应WaveController
            this.waveControllers[i] = waveControllers[i];
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

#if UNITY_EDITOR || UNITY_STANDALONE
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

        // 获取鼠标滚轮纵向滚动量，计算Omega的变化量
        float mouseScrollY = Input.mouseScrollDelta.y * mouseScrollSpeed;
        // 如果鼠标滚轮被滚动
        if (Mathf.Abs(mouseScrollY) >.01f) {
            if (FindWaveRefByScreenPos(Input.mousePosition)) {
                // 如果鼠标滚轮向上滚动
                if (mouseScrollY >.01f) {
                    waveModification.Omega /= mouseScrollY + 1;
                }
                // 如果鼠标滚轮向下滚动
                if (mouseScrollY < -.01f) {
                    waveModification.Omega *= -mouseScrollY + 1;
                }
                RefreshPapers();//可以直接更新omega
            }
        }

#else
        // 移动端使用 touch 输入

        // 单点触控
        if ((touchCount = Input.touchCount) == 1) {
            onePointPos = Input.GetTouch(0).position;
        }
        if (Input.GetTouch(0).phase == TouchPhase.Began) {
            onePointPhase = OnePointPhase.Began;
        } else if (Input.GetTouch(0).phase == TouchPhase.Ended) {
            onePointPhase = OnePointPhase.Ended;
        }

        // 双点触控
        // 终止捏合
        if (isPinching && Input.touchCount != 2) {
            isPinching = false;
        }

        // 处理捏合
        if (Input.touchCount == 2) {
            // 记录 两个touch
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // 计算 两个touch 间的向量 的 模长（距离）
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 确保模长不小于 deadZoneSize，以防止过大幅度的变化和不必要的错误
            touchDeltaMag = Mathf.Max(touchDeltaMag, deadZoneSize);

            // 如果已经开始捏合
            if (isPinching) {
                // 计算两帧间 Omega（应有）的变化量
                float deltaOmegaDiff = originTouchDeltaMag / touchDeltaMag;

                // 套用 Omega的变化量
                waveModification.Omega = originWaveModification.Omega * deltaOmegaDiff;
            } else if (FindWaveRefByScreenPos(onePointPos, touchOne.position)) {
                // 双指都在同一个纸片上，初始化新捏合
                originTouchDeltaMag = touchDeltaMag;
                isPinching = true;
            }
        }

#endif

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
            if (FindWaveRefByScreenPos(onePointPos)) {
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
            isChangingANotPhi = Mathf.Abs(diff.y) > Mathf.Abs(diff.x);

            // 标记此次 touch 已经脱离 deadZone
            inDeadZone = false;
        }

        // 如果已经不在 deadZone 内
        if (!inDeadZone) {
            // 要修改 A 还是 Phi
            if (isChangingANotPhi) {
                waveModification.A = waveModificationOrigin.A *
                    (diff.y * aZoomSpeed + 1);
            } else {
                waveModification.Phi = waveModificationOrigin.Phi +
                    diff.x * phiTransSpeed;
            }
            // 立刻刷新纸片上波形
            RefreshPapers();
           // Debug.Log("A: " + waveModification.A + " O: " + waveModification.Omega + "P: " + waveModification.Phi);
        }
    }

   
    // 在改动后刷新被修改的纸片们
    private void RefreshPapers() {
        // 更新纸片的 WaveModification
        waveData.SetWaveModification(0, waveModification);

          if(CheckUserAnswer(waveControllers[2]))
        {
            //存在数据处理问题 
            //如果判断成功的一瞬间输入仍然在检测 则会不断加关导致数组越界
            //进入某ui后 点选确定下关 再初始化纸片
            isPinching = false;
            isSwiping = false;
            isChangingANotPhi = true;
            inDeadZone = true;
            World.instance.MM.ClearMissions();
            World.instance.MM.DebugNextSubMission();
            return;
        }
        
        waveController.Refresh();//被改动的波
        waveControllers[2].Refresh();//sum波
        waveControllers[3].Refresh();
      

        //Debug.Log(CheckUserAnswer(waveControllers[2]));
        //WaveData wd = waveControllers[2].WaveData;
        // Debug.Log("3rd's Modification attribute : "+"A: " + wd.GetWaveModificationPrototype().A + " O: " + wd.GetWaveModificationPrototype().Omega + "P: " + wd.GetWaveModificationPrototype().Phi);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sum">总和纸片的wavecontroller</param>
    /// <returns></returns>
     private bool CheckUserAnswer(WaveController sum) {
        WaveModification ans =
            DataController.Instance.GetCurrentLevelData().modification;
        //DataController.Instance.GetCurrentLevelData().papersData[2].
        WaveModification usr = sum.WaveData.GetSumWaveModification(); // TODO
        //Debug.Log(usr);
        float[] usrAttributes = { usr.A,usr.Omega,usr.Phi };
        float[] ansAttributes = { ans.A,ans.Omega,ans.Phi };
        float[] ratio = new float[3];

       
        //看前两个属性的比值是否都大于0.9，如果有一个不大于就返回false即不通过
        //后看第三个属性是否是2pi整数倍
        for(int i=0;i<3;i++)
        {
            ratio[i] = Mathf.Abs(usrAttributes[i])/Mathf.Abs(ansAttributes[i]);
        }
        float num = (usrAttributes[2] - ansAttributes[2]) / 2*Mathf.PI;
        ratio[2] = Mathf.Abs(num - (int)num);
        Debug.Log("Ratio A: " + ratio[0] + " Ratio O: " + ratio[1] + " Ratio Pi: " + ratio[2]);

        //广播消息，传出数值变动
        {
            Dictionary<string,System.Object> dict = new Dictionary<string, System.Object>();
            //TODO:传关卡总序数进去
            dict.Add("A",ratio[0]);
            dict.Add("Omega",ratio[1]);
            dict.Add("Phi",ratio[2]);
            NotifyEvent nE = new NotifyEvent(NotifyType.ModificationAlter,this);
            NotificationCenter.getInstance().postNotification(nE);

        }
        for(int i=0;i<2;i++)
        {
            if(ratio[i]<0.8f || ratio[i]>2f) return false;
        }
        if(ratio[2]>0.1f && ratio[2]<0.9f) return false;
        //if(ratio[2])
        return true;
        
    }

    // 根据屏幕坐标寻找要修改的纸片的 WaveModification 和 WaveController
    private bool FindWaveRefByScreenPos(
        Vector2 screenPointOne,
        Vector2? screenPointTwo = null
    ) {
        // 将 屏幕坐标 转换到 世界坐标
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(
            screenPointOne.x,
            screenPointOne.y
        ));

        // 如果坐标落在某个 可修改纸片的内部
        for (int i = 0; i < 2; ++i) {
            if (inBound(worldPoint, i)) {
                // 如果传入了第二个点
                if (screenPointTwo != null) {
                    // 将 屏幕坐标 转换到 世界坐标
                    worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(
                        screenPointOne.x,
                        screenPointOne.y
                    ));
                    // 如果第二个点不在第i张纸片上，返回 false，并且不更新引用们
                    if (!inBound(worldPoint, i))
                        return false;
                }

                // 记录用户要更改的 WaveModification
                waveModification = waveModifications[i];
                waveModificationOrigin.CopyFrom(waveModification);
                // 记录要应用更改的 WaveData 和 WaveController
                waveData = waveDatas[i];
                waveController = waveControllers[i];

                // 返回 true：已找到
                return true;
            }
        }

        // 否则返回 false：未找到
        return false;
    }

    // 点是否在第i张纸片上
    private bool inBound(Vector2 worldPoint, int i) {
        return (
            papersBounds[i][0] < worldPoint.x &&
            worldPoint.x < papersBounds[i][1] &&
            papersBounds[i][2] < worldPoint.y &&
            worldPoint.y < papersBounds[i][3]
        );
    }
}