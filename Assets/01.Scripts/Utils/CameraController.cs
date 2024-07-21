using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float Speed = 1f;
    private Vector2 nowPos, prePos;
    private Vector3 movePos;
    private bool isDragging = false;
    [SerializeField] private LayerMask fieldLayer;
    [SerializeField] private string fieldTag = "Ground";
    private bool isOutOfBounds = false;
    public Camera mainCamera;
    private Vector3 cameraOriginPos;
    private float minCameraSize = 5f;
    private float maxCameraSize = 20f;
    private float zoomSpeed = 2f;

    private void Start()
    {
        cameraOriginPos = mainCamera.transform.position;
    }

    void Update()
    {
        HandleTouchInput();
        HandleMouseInput();
    }

    void HandleTouchInput()
    {
        // 터치 입력 처리
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag(fieldTag))
                    {
                        prePos = touch.position - touch.deltaPosition;
                        isDragging = true;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                nowPos = touch.position - touch.deltaPosition;
                movePos = new Vector3((prePos - nowPos).x * Time.deltaTime * Speed, 0, (prePos - nowPos).y * Time.deltaTime * Speed);
                mainCamera.transform.Translate(movePos, Space.World);
                prePos = touch.position - touch.deltaPosition;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                StartCoroutine(ConstrainCameraToTag());
            }
        }
        else if (Input.touchCount == 2)
        {
            // 두 손가락 터치로 화면 확대축소 처리
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            AdjustCameraSize(deltaMagnitudeDiff * zoomSpeed * Time.deltaTime);
        }
    }

    void HandleMouseInput()
    {
        // 마우스 입력 처리 (에디터에서 터치 입력을 시뮬레이트)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag(fieldTag))
                {
                    prePos = Input.mousePosition;
                    isDragging = true;
                }
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            nowPos = (Vector2)Input.mousePosition;
            movePos = new Vector3((prePos - nowPos).x * Time.deltaTime * Speed, 0, (prePos - nowPos).y * Time.deltaTime * Speed);

            mainCamera.transform.Translate(movePos, Space.World);
            prePos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            StartCoroutine(ConstrainCameraToTag());
        }

        // 마우스 휠로 화면 확대축소 처리
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            AdjustCameraSize(-scroll * zoomSpeed);
        }
    }

    void AdjustCameraSize(float delta)
    {
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize + delta, minCameraSize, maxCameraSize);
    }

    private IEnumerator ConstrainCameraToTag()
    {
        while (true)
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, fieldLayer))
            {
                yield break;
            }
            else
            {
                if (Vector3.Distance(mainCamera.transform.position, cameraOriginPos) > 1f)
                {
                    isOutOfBounds = true;
                    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraOriginPos, Time.deltaTime * Speed);
                }
                else
                {
                    isOutOfBounds = false;
                    yield break;
                }
            }
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isOutOfBounds ? Color.red : Color.green;
        Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.forward * 100);
    }
}
