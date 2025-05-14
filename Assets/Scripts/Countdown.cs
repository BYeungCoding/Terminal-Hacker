using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public DebuffManager debuffManager;
    private int countdown; 
    public TMP_Text countDisplay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdown = (int)debuffManager.currDebuffLength;
        countDisplay = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        countdown = (int)debuffManager.currDebuffLength;
        countDisplay.text = countdown.ToString();
    }
}
