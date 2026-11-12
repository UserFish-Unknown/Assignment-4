using UnityEngine;

public class CheckLocation : MonoBehaviour
{   
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.SceneCounter % 4 == 2)
        {
            player.transform.position = new Vector3(-71.89f, 0.06f, 3.46f);
        }
        else if (GameManager.SceneCounter % 4 == 0)
        {
            player.transform.position = new Vector3(68.61f, 0.06f, 2.65f);
        }
    }
}
