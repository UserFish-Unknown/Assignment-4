using UnityEngine;

public class DoorOpen : MonoBehaviour, IInteractable
{
    public void Interact(){
        Debug.Log("Door Opening...");
        gameObject.transform.Rotate(Vector3.up * Time.deltaTime * 1000);
        //TODO: stop the interaction once it has happened once.
        GetComponent<Collider>().enabled = false;
        //TODO: transition to next scene
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
