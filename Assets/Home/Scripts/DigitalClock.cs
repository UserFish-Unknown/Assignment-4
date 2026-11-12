using System;
using TMPro;
using UnityEngine;

public class DigitalClock : MonoBehaviour
{
    public TMP_Text display;
    public bool use24Hour = false;
    public bool blinkColon = true;

    void Update()
    {
        DateTime now = DateTime.Now;
        string colon = (blinkColon && now.Second % 2 == 1) ? " " : ":";
        string format = use24Hour ? "HH" : "hh";
        display.text = now.ToString(format) + colon + now.ToString("mm");
    }
}
