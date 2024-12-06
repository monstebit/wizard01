using UnityEngine;

public class OrbitalPlacement : MonoBehaviour
{
    public Transform[] objectsToOrbit; // Объекты, которые будут размещены на орбите
    public float orbitRadius = 5f; // Радиус орбиты
    public float orbitSpeed = 1f; // Скорость движения по орбите

    private float angleStep; // Угол между объектами на орбите

    void Start()
    {
        if (objectsToOrbit.Length == 0)
        {
            Debug.LogError("No objects to orbit assigned.");
            return;
        }

        angleStep = 360f / objectsToOrbit.Length;

        // Размещаем объекты на орбите
        for (int i = 0; i < objectsToOrbit.Length; i++)
        {
            float angle = i * angleStep;
            Vector3 position = GetOrbitPosition(angle);
            objectsToOrbit[i].position = position;
        }
    }

    void Update()
    {
        // Перемещаем объекты по орбите
        for (int i = 0; i < objectsToOrbit.Length; i++)
        {
            float angle = (i * angleStep + Time.time * orbitSpeed) % 360f;
            Vector3 position = GetOrbitPosition(angle);
            objectsToOrbit[i].position = position;
        }
    }

    // Вычисляем позицию на орбите по углу
    private Vector3 GetOrbitPosition(float angle)
    {
        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * orbitRadius;
        float z = Mathf.Cos(angle * Mathf.Deg2Rad) * orbitRadius;
        return new Vector3(x, 0, z) + transform.position;
    }
}