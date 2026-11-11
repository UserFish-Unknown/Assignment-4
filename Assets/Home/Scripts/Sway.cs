using UnityEngine;

public class Sway : MonoBehaviour
{
    public Vector3 axis = Vector3.right;
    public float maxAngle = 3f;
    public float speed = 1.2f;
    Quaternion start;

    void Start() { start = transform.localRotation; }

    void Update()
    {
        float a = Mathf.Sin(Time.time * speed) * maxAngle;
        transform.localRotation = start * Quaternion.AngleAxis(a, axis);
    }
}