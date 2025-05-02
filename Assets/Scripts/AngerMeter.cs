using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AngerMeter : MonoBehaviour
{
    public Image progressBarFill;
    public float angerSpeed = 0.1f;
    private float _angerLevel = 0f;
    public GameObject debuffIcon;
    public GameObject neutralIcon;
    private bool isDebuffed = false;
    private bool isShaking = false;
    private Coroutine shakeCoroutine;
    public GameObject gameOverScreen;
    public GameObject locgicScript;
    private Vector3 originalBarPosition;


    void Start()
    {
        originalBarPosition = progressBarFill.rectTransform.localPosition;
    }
    void Update()
    {
        float increaseRate = angerSpeed * Time.deltaTime * (isDebuffed ? angerSpeed : 0.01f);
        _angerLevel += increaseRate;
        _angerLevel = Mathf.Clamp01(_angerLevel);
        progressBarFill.fillAmount = _angerLevel;
        progressBarFill.color = Color.Lerp(Color.green, Color.red, _angerLevel);

        if (_angerLevel >= 1f && !isShaking)
        {
            isShaking = true;
            shakeCoroutine = StartCoroutine(ShakeBarThenGameOver());
        }

        if (_angerLevel < 1f && isShaking)
        {
            StopCoroutine(shakeCoroutine);
            StopShake();
        }
    }

    public void SetAngerLevel(float level)
    {
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
        _angerLevel = 0f;
        progressBarFill.fillAmount = _angerLevel;
    }

    public void AppyDebuff(bool debuffed)
    {
        isDebuffed = debuffed;
        if (debuffIcon != null && debuffed == true)
        {
            debuffIcon.SetActive(true);
            neutralIcon.SetActive(false);
        }
        else if (debuffIcon != null && debuffed == false)
        {
            CalmDown();
            debuffIcon.SetActive(false);
            neutralIcon.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Debuff icon not assigned in the inspector.");
        }
    }

    public void CalmDown()
    {
        StartCoroutine(CalmDownCoroutine());
    }

    private IEnumerator CalmDownCoroutine()
    {
        float timer = 5f;
        float decreaseRate = 0.05f;

        while (timer > 0f)
        {
            _angerLevel -= Time.deltaTime * decreaseRate;
            _angerLevel = Mathf.Clamp01(_angerLevel);
            progressBarFill.fillAmount = _angerLevel;
            progressBarFill.color = Color.Lerp(Color.green, Color.red, _angerLevel);

            timer -= Time.deltaTime;
            yield return null;
        }
        progressBarFill.rectTransform.localPosition = originalBarPosition;
    }

    IEnumerator ShakeBarThenGameOver()
    {
        float timer = 5f;

        while (timer > 0f)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * 5f;
            shakeOffset.z = 0;
            progressBarFill.rectTransform.localPosition = originalBarPosition + shakeOffset;

            timer -= Time.deltaTime;
            yield return null;
        }

        progressBarFill.rectTransform.localPosition = originalBarPosition;
        TriggerGameOver();
    }

    void StopShake()
    {
        isShaking = false;
        if (progressBarFill != null)
            progressBarFill.rectTransform.localPosition = originalBarPosition;
    }

    void TriggerGameOver()
    {

        isShaking = false;
        Debug.Log("GAME OVER: Anger meter maxed out.");
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        if (locgicScript != null)
            locgicScript.SetActive(false);

    }
}
