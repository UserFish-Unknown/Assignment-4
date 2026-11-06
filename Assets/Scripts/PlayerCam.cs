using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float sensX; // Sensitivity for the X-axis (horizontal) movement of the camera
    public float sensY; // Sensitivity for the Y-axis (vertical) movement of the camera

    public Transform orientation; // Reference to the player's orientation (used for camera rotation)

    float xRotation; // Current rotation around the X-axis (vertical rotation)
    float yRotation; // Current rotation around the Y-axis (horizontal rotation)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current == null) // Check if the mouse is connected
            return;
        
        // Get mouse movement input
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * sensX;
        float mouseY = mouseDelta.y * sensY;

        // Update rotation
        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp vertical rotation to avoid flipping
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotation to the camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0); // Apply horizontal rotation to the player's orientation
    }
}
