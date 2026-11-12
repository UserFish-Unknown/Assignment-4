using UnityEngine;

public class DoorOpen : MonoBehaviour, IInteractable
{
    [SerializeField] private string nextSceneName;
    private int seconds = 0;
    private float elapsedTime = 0f;
    private bool DoorOpening = false;
    private AudioSource audioSource;

    public void Interact(){
        Debug.Log("Door Opening...");
        if(DoorOpening == false){
            audioSource = GetComponent<AudioSource>();
            audioSource.Play();
        }
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        DoorOpening = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(DoorOpening){
            gameObject.transform.Rotate(Vector3.up * Time.deltaTime * 10);

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 1f)
            {
                elapsedTime -= 1f; 
                seconds++;
                Debug.Log("Timer: " + seconds);
            }
        }

        if(seconds>=2){
            GameManager.instance.ChangeLevelTo(nextSceneName);
        }
    }
}
