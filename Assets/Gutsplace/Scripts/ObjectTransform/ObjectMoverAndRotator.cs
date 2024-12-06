using UnityEngine;

public class ObjectMoverAndRotator : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveHeight = 2f; // Высота подъема/опускания
    public float moveSpeed = 1f; // Скорость движения вверх/вниз

    [Header("Rotation Settings")]
    public bool rotateX = false; // Вращение по оси X
    public bool rotateY = false; // Вращение по оси Y
    public bool rotateZ = false; // Вращение по оси Z
    public float rotationSpeed = 100f; // Скорость вращения

    private Vector3 startPosition;
    private float timeElapsed;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Движение вверх-вниз
        timeElapsed += Time.deltaTime * moveSpeed;
        float newY = startPosition.y + Mathf.Sin(timeElapsed) * moveHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Вращение
        Vector3 rotation = Vector3.zero;
        if (rotateX) rotation.x = rotationSpeed * Time.deltaTime;
        if (rotateY) rotation.y = rotationSpeed * Time.deltaTime;
        if (rotateZ) rotation.z = rotationSpeed * Time.deltaTime;
        transform.Rotate(rotation);
    }
}