using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGestureScript : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private Vector2 touchStartPosition;

    void OnTouchDown(Touch touch)
    {
        if (touch.fingerId == 0) // Check for first finger touch
        {
            touchStartPosition = touch.position;
        }
    }

    void OnTouchDrag(Touch touch)
    {
        if (touch.fingerId == 0) // Check for first finger drag
        {
            Vector2 delta = touch.position - touchStartPosition;
            // Implement logic to move the map based on the delta (e.g., translate the map camera)

            mainCamera.transform.position += (Vector3)delta;

            touchStartPosition = touch.position;
        }
    }

    //private Vector3 touchStart;
    //private float initialPinchDistance;
    //private float initialRotationAngle;
    //private bool isRotating = false;
    //private bool isZooming = false;

    //public float zoomSpeed = 0.1f;
    //public float rotateSpeed = 0.1f;
    //public Camera mainCamera;

    //void Update()
    //{
    //    if (Input.touchCount == 1)
    //    {
    //        Touch touch = Input.GetTouch(0);

    //        if (touch.phase == TouchPhase.Began)
    //        {
    //            touchStart = mainCamera.ScreenToWorldPoint(touch.position);
    //            isRotating = false;
    //            isZooming = false;
    //        }
    //        else if (touch.phase == TouchPhase.Moved && !isRotating && !isZooming)
    //        {
    //            Vector3 direction = touchStart - mainCamera.ScreenToWorldPoint(touch.position); 
    //            mainCamera.transform.position += direction;
    //        }
    //    }
    //    else if (Input.touchCount == 2)
    //    {
    //        Touch touch1 = Input.GetTouch(0);
    //        Touch touch2 = Input.GetTouch(1);

    //        if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
    //        {
    //            initialPinchDistance = Vector2.Distance(touch1.position, touch2.position);
    //            initialRotationAngle = Vector2.Angle(touch1.position, touch2.position);
    //            isRotating = false;
    //            isZooming = false;
    //        }
    //        else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
    //        {
    //            // Check for pinch gesture (zoom)
    //            float currentPinchDistance = Vector2.Distance(touch1.position, touch2.position);
    //            if (Mathf.Abs(currentPinchDistance - initialPinchDistance) > 0.01f)
    //            {
    //                float pinchAmount = (currentPinchDistance - initialPinchDistance) * zoomSpeed;
    //                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - pinchAmount, 2f, 10f);
    //                initialPinchDistance = currentPinchDistance;
    //                isZooming = true;
    //            }

    //            // Check for rotation gesture
    //            float currentRotationAngle = Vector2.Angle(touch1.position, touch2.position);
    //            if (Mathf.Abs(currentRotationAngle - initialRotationAngle) > 0.01f)
    //            {
    //                float rotationAmount = (initialRotationAngle - currentRotationAngle) * rotateSpeed;
    //                mainCamera.transform.Rotate(Vector3.forward, rotationAmount);
    //                initialRotationAngle = currentRotationAngle;
    //                isRotating = true;
    //            }
    //        }
    //    }
    //}
}
