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
    public UnityEngine.UI.Text winFileText; //Add a new text box for holding the amount of wins
    private int wins = 0;
    public levelGen levelGEN;
    public TerminalController terminalController; // Reference to the TerminalController script

    public Timer timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        titleScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        Canvas.SetActive(false);
        TitleMusic.Play();
        terminalController.terminalPanel.SetActive(false);
    }


    public void startGame()
    {
        levelGEN.ResetLevel();
        SetRemainingTime(300f);
        LevelStart.Play();
        titleScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        Canvas.SetActive(true);
        TitleMusic.Stop();
        LevelStart.Play();
        terminalController.terminalPanel.SetActive(false);

        if (angerMeter != null)
        {
            angerMeter.StopAllCoroutines();         // stop any ongoing shake
            angerMeter.ResetAngerLevel();           // reset value to 0
            angerMeter.AppyDebuff(false);           // remove debuff icon
        }
    }

    public void startTutorial(){
        SetRemainingTime(300f);
        levelGEN.generateTutorial(terminalController);
        titleScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        Canvas.SetActive(true);
        TitleMusic.Stop();
        LevelStart.Play();
        terminalController.terminalPanel.SetActive(false); 
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
        levelGEN.ResetLevel();
        SetRemainingTime(300f);
        Canvas.SetActive(true);
        LevelStart.Play();
        TitleMusic.Stop();
        gameOverScreen.SetActive(false);
        titleScreen.SetActive(false);
        terminalController.terminalPanel.SetActive(false);
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

    public void TriggerGameOver()
    {
        Debug.Log("GAME OVER triggered.");
        gameOverScreen.SetActive(true);
        switch(wins){
            case 0:
                //If the score is 0: JAIL 
                Canvas.SetActive(false);
                break;
            case 1:
                //If the score is 1: BAD
                Canvas.SetActive(false);
                break;
            case 2:
                //If the score is 2: GOOD
                Canvas.SetActive(false);
                break;
            default:
                //If the score is 3 or more: GREAT
                Canvas.SetActive(false);
                break;
        }
    }

    public void WinFileSolved()
    {
        wins++;
        winFileText.text = $"{wins}"; //Replace this with whatever new win text box you want to add
    }

    public void SetRemainingTime(float time)
    {
        if (timer != null)
        {
            timer.remaningTime = time;
        }
        else
        {
            Debug.LogError("Timer script not found in the scene.");
        }
    }
}
