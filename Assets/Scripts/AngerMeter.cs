using UnityEngine;
using UnityEngine.UI;

public class AngerMeter : MonoBehaviour
{
    public Image progressBarFill;
    public float angerSpeed = 0.1f;
    private float _angerLevel = 0f; // Value between 0 and 1
    public GameObject debuffIcon;
    public GameObject neutralIcon; 
    private bool isDebuffed = false; // Flag to indicate if the character is debuffed
    // Update is called once per frame
    void Update()
    {
        /*
         * This method updates the anger level over time. You can adjust the rate of increase by changing the multiplier.
         * The value will be clamped between 0 and 1 to ensure it stays within the valid range for the progress bar.
         */
         float increaseRate = angerSpeed * Time.deltaTime * (isDebuffed ? angerSpeed : 0.01f);
        _angerLevel += increaseRate; // Increase the anger level based on the speed and time delta
        _angerLevel = Mathf.Clamp01(_angerLevel); // Ensure the anger level stays within 0 and 1
        progressBarFill.fillAmount = _angerLevel; // Update the UI to reflect the current anger level
        progressBarFill.color = Color.Lerp(Color.green, Color.red, _angerLevel); // Update the color of the progress bar based on the anger level

    }

     public void SetAngerLevel(float level)
    {   
         /*
         * This method allows external scripts to set the anger level directly.
         * Level should be between 0 and 1.
         */
         if (level < 0f || level > 1f)
        {
            Debug.LogWarning("Anger level must be between 0 and 1.");
            return;
        }
        _angerLevel = Mathf.Clamp01(level);
        progressBarFill.fillAmount = _angerLevel;
    }

     public void ResetAngerLevel()
    {
        _angerLevel = 0f; // Reset the anger level to 0
        progressBarFill.fillAmount = _angerLevel; // Update the UI to reflect the reset
    }


    public void AppyDebuff(bool debuffed)
    {
        /*
         * This method allows external scripts to apply or remove a debuff which affects the anger level increase rate.
         */
        isDebuffed = debuffed;
        if (debuffIcon != null && debuffed == true){
            debuffIcon.SetActive(true);
            neutralIcon.SetActive(false); // Hide neutral icon when debuffed
        }
        else if (debuffIcon != null && debuffed == false){
            CalmDown();
            debuffIcon.SetActive(false);
            neutralIcon.SetActive(true); // Show neutral icon when not debuffed
            
        }
        else 
            Debug.LogWarning("Debuff icon not assigned in the inspector.");
    }

    public void CalmDown()
    {
        float timer = 5f;

        while (timer > 0f){
            _angerLevel -= Time.deltaTime * 0.05f; // Simulate calming down when called
            _angerLevel = Mathf.Clamp01(_angerLevel); // Ensure it stays within 0 and 1
            progressBarFill.fillAmount = _angerLevel; // Update the UI to reflect the new anger level
            progressBarFill.color = Color.Lerp(Color.green, Color.red, _angerLevel); // Update the color of the progress bar based on the new anger level   

            timer -= Time.deltaTime; // Decrease the timer
            if (timer <= 0f)
            {
                break; // Exit the loop when the timer is up
            }
        }
    }
}
