using UnityEngine;

[ExcludeFromObjectFactory]
public class CameraIsometricPivot : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _target;
    public float targetAngle = 45f;
    public float currentAngle = 0f;
    public float mouseSensitivity = 2f;
    public float rotationSpeed = 5f;
    public float distanceFromTarget = 10f;

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Debug.LogError("No camera found. Please assign a camera.");
                return;
            }
        }

        if (_target == null)
        {
            Debug.LogError("No target assigned. Please assign a target object.");
            return;
        }

        _camera.transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        HandleMouseInput();
        UpdateRotation();
    }

    private void HandleMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");

        if (Input.GetMouseButton(0))
        {
            targetAngle += mouseX * mouseSensitivity;
        }
        else
        {
            targetAngle = Mathf.Round(targetAngle / 45f) * 45f;
        }

        targetAngle = Mathf.Repeat(targetAngle, 360f);
    }

    private void UpdateRotation()
    {
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        Vector3 newPosition = _target.position + Quaternion.Euler(30, currentAngle, 0) * Vector3.back * distanceFromTarget;

        transform.position = newPosition;
        transform.LookAt(_target);
    }
}