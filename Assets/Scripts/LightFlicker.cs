using UnityEngine;
using UnityEngine.InputSystem;

public class LightFlicker : MonoBehaviour
{
    private int ran;
    private int flickerLength;
    public Light light1;
    public Light light2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flickerLength = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Start Here");
        if(flickerLength == 0){
            light1.enabled = true;
            light2.enabled = true;
            ran = Random.Range(0,1600);
            if(ran==50){
                //Debug.Log("It is 50");
                flickerLength = Random.Range(10, 40);
                light1.enabled = false;
                light2.enabled = false;
                //Debug.Log("this ran");
            }
        }else{
            flickerLength--;
            //Debug.Log("flickerLength: "+ flickerLength);
        }
    }
}
