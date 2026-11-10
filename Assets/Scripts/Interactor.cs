using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

interface IInteractable {
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public Text InteractText;
    public Image Crosshair;
    public Transform InteractorSource;
    public float InteractRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InteractText.text = "Press E To Interact";
        InteractText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if(Physics.Raycast(r, out RaycastHit hitInfo, InteractRange)){
            if(hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj)){
                InteractText.enabled = true;
                if(Keyboard.current.eKey.wasPressedThisFrame){
                    interactObj.Interact();
                }
                return;
            }
        }
        InteractText.enabled = false;
    }
}
