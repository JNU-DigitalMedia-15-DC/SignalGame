using UnityEngine;

public class WaveInputController : MonoBehaviour {

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

        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (camera.isOrthoGraphic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                camera.orthographicSize = Mathf.Max(camera.orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 0.1f, 179.9f);
            }
        }
#endif
    }
}