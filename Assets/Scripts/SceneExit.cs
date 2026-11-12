using UnityEngine;

public class SceneExit : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter(Collider collision){
        if(collision.GetComponentInParent<PlayerMovement>() != null){
            Debug.Log("This Worked");
            GameManager.instance.ChangeLevelTo(nextSceneName);
        }
    }
}
