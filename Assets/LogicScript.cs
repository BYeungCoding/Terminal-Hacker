using TMPro;
using UnityEngine;

public class LogicScript : MonoBehaviour
{
        public GameObject titleScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        titleScreen.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame()
    {
        titleScreen.SetActive(false);
    }
}
