using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int SceneCounter = 1;
    public static string PreviousSceneName { get; private set; }
    public static GameManager instance;

    private void Awake(){
        instance = this;
        Debug.Log("Current Scene: "+SceneCounter);
    }

    public void ChangeLevelTo(string LevelName){
        PreviousSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(LevelName);
        SceneCounter++;
        Debug.Log("Current Scene: "+SceneCounter);
    }
}
