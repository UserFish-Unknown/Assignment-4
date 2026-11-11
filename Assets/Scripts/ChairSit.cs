using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChairSit : MonoBehaviour, IInteractable
{
    public Rigidbody Player;
    private bool IsSeated = false;
    public Text DismountText;

    public void Interact(){
        DismountText.enabled = false;
        Debug.Log("T/F I am seated: "+IsSeated);
        if(IsSeated == false){
            Debug.Log("Sitting Down...");
            Player.isKinematic = true;
            Player.detectCollisions = false;
            GetComponent<Collider>().enabled = false;
            Player.transform.position = transform.position;
            Player.transform.position += new Vector3(0, 1f, -0.2f);
            Debug.Log("Seated at "+transform.position);
            IsSeated = true;
            return;
        }else if(IsSeated){
            return;
        }
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DismountText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsSeated){
            DismountText.text = "Press 1 To Stand Up";
            DismountText.enabled = true;
            if(Keyboard.current.digit1Key.wasPressedThisFrame){
                Debug.Log("Standing Up...");
                Player.isKinematic = false;
                Player.detectCollisions = true;
                GetComponent<Collider>().enabled = true;
                Player.transform.position += new Vector3(1f, 0, -0.2f);
                IsSeated = false;
            }
        }else{
            DismountText.enabled = false;
        }
    }
}
