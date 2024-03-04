using UnityEngine;

public class ViewController : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private Transform cameraTransform;

    [Range(1, 5)]
    [SerializeField] private float mouseSens;

    [Tooltip("Multiplicative"), Range(1, 6)]
    [SerializeField] private float zoomSpeed = 0.5f;

    [SerializeField] private int rotateMouseButton = 2;

    [SerializeField] private bool pan = false;

    private float xRotation;
    private float yRotation;

    private void Start ()
    {
        transform.position = anchor.position;
    }

    private void Update ()
    {
        if(pan)
            HandlePanning();
        else
            HandleCameraRotation();
        ZoomHandling();
    }

    private void ZoomHandling ()
    {
        var direction = anchor.position - cameraTransform.localPosition;

        var translation = 10.0f * Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed * direction.normalized;

        if ( Vector3.Distance(anchor.position, cameraTransform.position + translation) < 10.0f )
        { return; }

        cameraTransform.Translate(10.0f * Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed * direction.normalized, Space.Self);
    }

    private void HandleCameraRotation ()
    {
        if ( Input.GetMouseButtonDown(rotateMouseButton) )
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if ( Input.GetMouseButtonUp(rotateMouseButton) )
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if ( !Input.GetMouseButton(rotateMouseButton) )
        { return; }

        float mouseX = Input.GetAxis("Mouse X") * mouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;

        xRotation -= mouseY;
        yRotation += mouseX;

        transform.SetPositionAndRotation(anchor.position, Quaternion.Euler(xRotation, yRotation, 0));
    }

    private void HandlePanning ()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;

        Vector3 moveDirection = transform.forward * mouseY + transform.right * mouseX;

        transform.Translate(moveDirection * Time.deltaTime);
    }
}