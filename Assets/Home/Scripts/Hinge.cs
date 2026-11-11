using UnityEngine;

public class HingeDoor : MonoBehaviour
{
    public float openAngle = -90f;      // negative = swings INTO the room
    public float speed = 120f;
    public KeyCode testKey = KeyCode.O; // temporary, for testing
    bool open;
    float current;
    Quaternion closedRot;

    void Start() { closedRot = transform.localRotation; }

    void Update()
    {
        if (Input.GetKeyDown(testKey)) Toggle();
        float target = open ? openAngle : 0f;
        current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
        transform.localRotation = closedRot * Quaternion.Euler(0, current, 0);
    }

    public void Toggle() { open = !open; }
}
