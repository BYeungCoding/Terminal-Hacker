using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.AI;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] public float remaningTime;
    public LogicScript logicScript;

    void Update()
    {

        if (remaningTime > 0)
        {
            remaningTime -= Time.deltaTime;
        }
        else if (remaningTime < 0)
        {
            remaningTime = 0;
            logicScript.TriggerGameOver();
        }

        int minutes = Mathf.FloorToInt(remaningTime / 60);
        int seconds = Mathf.FloorToInt(remaningTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
