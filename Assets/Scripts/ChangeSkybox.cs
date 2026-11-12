using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{

    public Material daySkybox;
    public Material nightSkybox;
    public Light directionalLight;
    public GameObject lamppostParent;
    public GameObject NPCParent;
    public AudioSource audio1;
    public AudioSource audio2;
    public AudioSource audio3;

    void Start()
    {
        if (GameManager.SceneCounter % 4 == 0)
        {
            RenderSettings.skybox = nightSkybox;
            directionalLight.enabled = false;
            lamppostParent.SetActive(true);
            NPCParent.SetActive(false);
            audio1.Stop();
            audio2.Stop();
            audio3.Play();
        }
        else
        {
            RenderSettings.skybox = daySkybox;
            directionalLight.enabled = true;
            lamppostParent.SetActive(false);
            NPCParent.SetActive(true);
            audio1.Play();
            audio2.Play();
            audio3.Stop();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
