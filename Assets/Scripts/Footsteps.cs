using System.Collections;
using UnityEngine;

public class Footsteps : MonoBehaviour
{

    public GameObject player;
    public AudioSource audio1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio1.transform.position = player.transform.position - player.transform.forward * 2f;       
        if (GameManager.SceneCounter % 4 == 0)
        {
            StartCoroutine(PlayAfterRandomDelay());
        } else
        {
            audio1.Stop();
        }
    }

    IEnumerator PlayAfterRandomDelay()
    {
        yield return new WaitForSeconds(Random.Range(5f, 10f)); // Wait for a random delay between 5 and 10 seconds

        if (GameManager.SceneCounter % 4 == 0)
        {
            audio1.Play();
        }
    }
}
