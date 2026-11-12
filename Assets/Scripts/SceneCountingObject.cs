using UnityEngine;

public class SceneCountingObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.SceneCounter = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
