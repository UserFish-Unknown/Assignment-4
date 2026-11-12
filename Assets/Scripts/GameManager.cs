using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int SceneCounter;
    public static string PreviousSceneName { get; private set; }
    public static GameManager instance;

    private void Start(){
        instance = this;
        Debug.Log("Current Scene: "+SceneCounter);
    }

    public void ChangeLevelTo(string LevelName){
        PreviousSceneName = SceneManager.GetActiveScene().name;
        SceneCounter++;
        SceneManager.LoadScene(LevelName);
        Debug.Log("Current Scene: "+SceneCounter);
    }
}
