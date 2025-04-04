using UnityEngine;
using UnityEngine.UI;

public class AngerMeter : MonoBehaviour
{
    public Image progressBarFill;
    private float _angerLevel = 0f; // Value between 0 and 1

    // Update is called once per frame
    void Update()
    {
        /*
         * This method updates the anger level over time. You can adjust the rate of increase by changing the multiplier.
         * The value will be clamped between 0 and 1 to ensure it stays within the valid range for the progress bar.
         */
        _angerLevel += Time.deltaTime * 0.1f; 
        _angerLevel = Mathf.Clamp01(_angerLevel);
        progressBarFill.fillAmount = _angerLevel; 

        progressBarFill.color = Color.Lerp(Color.green, Color.red, _angerLevel); 
        if (_angerLevel >= 1)
            Debug.Log("Character is fully angry!");
            transform.position += new Vector3(Random.Range(-0.1f, 0.1f), 0, 0);
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
}
