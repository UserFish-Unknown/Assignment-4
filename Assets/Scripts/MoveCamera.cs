using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition; // Reference to the camera's position

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position; // Update the camera's position to match the reference position
    }
}
