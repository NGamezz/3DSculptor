using UnityEngine;

public class ViewController : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private Transform cameraTransform;

    [Range(1, 5)]
    [SerializeField] private float mouseSens;

    [Tooltip("Multiplicative"), Range(1, 6)]
    [SerializeField] private float zoomSpeed = 0.5f;

    private float xRotation;
    private float yRotation;

    private void Start ()
    {
        transform.position = anchor.position;
    }

    private void Update ()
    {
        HandleCameraRotation();
        ZoomHandling();
    }

    private void ZoomHandling ()
    {
        var direction = anchor.position - cameraTransform.localPosition;

        var translation = 10.0f * Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed * direction.normalized;

        if ( Vector3.Distance(anchor.position, cameraTransform.position + translation) < 10.0f)
        { return; }

        cameraTransform.Translate(10.0f * Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed * direction.normalized, Space.Self);
    }

    private void HandleCameraRotation ()
    {
        if ( Input.GetMouseButtonDown(1) )
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if ( Input.GetMouseButtonUp(1) )
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if ( !Input.GetMouseButton(1) )
        { return; }

        float mouseX = Input.GetAxis("Mouse X") * mouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;

        xRotation -= mouseY;
        yRotation += mouseX;

        transform.SetPositionAndRotation(anchor.position, Quaternion.Euler(xRotation, yRotation, 0));
    }
}