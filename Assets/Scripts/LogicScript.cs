using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public GameObject Canvas;
    public AudioSource TitleMusic;
    public AudioSource LevelStart;
    public AngerMeter angerMeter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        titleScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        Canvas.SetActive(false);
        TitleMusic.Play();
    }


    public void startGame()
    {
        LevelStart.Play();
        titleScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        Canvas.SetActive(true);
        TitleMusic.Stop();
        LevelStart.Play();

        if (angerMeter != null)
        {
            angerMeter.StopAllCoroutines();         // stop any ongoing shake
            angerMeter.ResetAngerLevel();           // reset value to 0
            angerMeter.AppyDebuff(false);           // remove debuff icon
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }
    public void restartGame()
    {
        Canvas.SetActive(true);
        LevelStart.Play();
        TitleMusic.Stop();
        gameOverScreen.SetActive(false);
        titleScreen.SetActive(false);
        if (angerMeter != null)
        {
            angerMeter.StopAllCoroutines();         // stop any ongoing shake
            angerMeter.ResetAngerLevel();           // reset value to 0
            angerMeter.AppyDebuff(false);           // remove debuff icon
        }
    }
    public void returnToTitle()
    {
        gameOverScreen.SetActive(false);
        Canvas.SetActive(false);
        TitleMusic.Play();
        LevelStart.Stop();
        titleScreen.SetActive(true);
        
    }
}
