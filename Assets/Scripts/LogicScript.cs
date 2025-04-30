using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public GameObject titleScreen;
    public AudioSource TitleMusic;
    public AudioSource LevelStart;

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
        LevelStart.Play();
        titleScreen.SetActive(false);
        SceneManager.LoadScene("Main01");
    }
}
