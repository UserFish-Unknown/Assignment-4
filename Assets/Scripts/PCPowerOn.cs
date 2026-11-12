using UnityEngine;

public class PCPowerOn : MonoBehaviour, IInteractable
{
    public Canvas ComputerScreen;
    public Transform Player;

    public void Interact(){
        if(!ComputerScreen.enabled){
            Debug.Log("Powering On...");
            ComputerScreen.enabled = true;
            GetComponent<Collider>().enabled = false;
            //Cursor.lockState = CursorLockMode.None;
            //Player<Collider>().enabled = false;
            return;
            //.SetPositionAndRotation(lockedPosition, lockedRotation);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ComputerScreen.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
