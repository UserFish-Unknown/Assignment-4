using UnityEngine;

public class Blinker : MonoBehaviour
{
    public GameObject target;   // a CHILD object to show/hide
    public float interval = 0.5f;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            target.SetActive(!target.activeSelf);
        }
    }
}