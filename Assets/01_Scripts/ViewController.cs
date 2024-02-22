using UnityEngine;

public class ViewController : MonoBehaviour
{
    [SerializeField] private GameObject anchor;
    [SerializeField] private GameObject defaultAnchor;
    [SerializeField] private Transform cameraTransform;

    [Range(1, 5)]
    [SerializeField] private float mouseSens;

    [Tooltip("Multiplicative"), Range(1, 6)]
    [SerializeField] private float zoomSpeed = 0.5f;

    private float xRotation;
    private float yRotation;

    [SerializeField] private int anchorLayerMask = 6;

    private void Update()
    {
        HandleCameraRotation();
        ZoomHandling();
    }

    private void ZoomHandling()
    {
        var direction = anchor.transform.position - cameraTransform.position;
        cameraTransform.Translate(direction.normalized * zoomSpeed * Input.mouseScrollDelta.y * 10.0f * Time.deltaTime);
    }

    private void HandleCameraRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (!Input.GetMouseButton(1)) { return; }

        float mouseX = Input.GetAxis("Mouse X") * mouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;

        xRotation -= mouseY;
        yRotation += mouseX;

        //xRotation = Mathf.Clamp(xRotation, -90, 90);
        //yRotation = Mathf.Clamp(yRotation, -90, 90);

        transform.SetPositionAndRotation(anchor.transform.position, Quaternion.Euler(xRotation, yRotation, 0));
    }
}