using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int SceneCounter = 1;
    public static GameManager instance;

    private void Awake(){
        instance = this;
        Debug.Log("Current Scene: "+SceneCounter);
    }

    public void ChangeLevelTo(string LevelName){
        SceneManager.LoadScene(LevelName);
        SceneCounter++;
        Debug.Log("Current Scene: "+SceneCounter);
    }
}
