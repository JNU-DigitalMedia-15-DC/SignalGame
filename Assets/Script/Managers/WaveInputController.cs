using UnityEngine;

public class WaveInputController : MonoBehaviour {

    WaveModification waveModification;

#if !UNITY_STANDALONE
    protected Vector2 m_StartingTouch;
    protected bool m_IsSwiping = false;
#endif
    private void Update() {
#if UNITY_EDITOR || UNITY_STANDALONE
        // Use key input in editor or standalone
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            ChangeLane(-1);
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ChangeLane(1);
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Jump();
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (!m_Sliding)
                Slide();
        }
#else
        // Use touch input on mobile
        if (Input.touchCount == 1) {
            if (m_IsSwiping) {
                Vector2 diff = Input.GetTouch(0).position - m_StartingTouch;

                // Put difference in Screen ratio, but using only width, so the ratio is the same on both
                // axes (otherwise we would have to swipe more vertically...)
                diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);

                if (diff.magnitude > 0.01f) //we set the swip distance to trigger movement to 1% of the screen width
                {
                    if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x)) {
                        if (diff.y < 0) {
                            Slide();
                        } else {
                            Jump();
                        }
                    } else {
                        if (diff.x < 0) {
                            ChangeLane(-1);
                        } else {
                            ChangeLane(1);
                        }
                    }

                    m_IsSwiping = false;
                }
            }

            // Input check is AFTER the swip test, that way if TouchPhase.Ended happen a single frame after the Began Phase
            // a swipe can still be registered (otherwise, m_IsSwiping will be set to false and the test wouldn't happen for that began-Ended pair)
            if (Input.GetTouch(0).phase == TouchPhase.Began) {
                m_StartingTouch = Input.GetTouch(0).position;
                m_IsSwiping = true;
            } else if (Input.GetTouch(0).phase == TouchPhase.Ended) {
                m_IsSwiping = false;
            }
        }

        // 如果设备上有 两个touch ……
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
#endif
    }
}