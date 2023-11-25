using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public Gradient gradient;

    private Slider healthSlider;
    private Text healthText;
    private Image healthFill;

    public Slider slowTimeSlider;
    public Text slowTimeText;
    public Image slowTimeFill;
    public Image slowTimeBackground;

    private Text reloadingText;

    private Text bulletCount;

    private void Awake()
    {
        healthSlider = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
        healthText = GameObject.FindGameObjectWithTag("HealthBar").GetComponentInChildren<Text>();
        healthFill = GameObject.FindGameObjectWithTag("HealthBar").GetComponentsInChildren<Image>()[1];

        slowTimeSlider = GameObject.FindGameObjectWithTag("SlowTime").GetComponent<Slider>();
        slowTimeText = GameObject.FindGameObjectWithTag("SlowTime").GetComponentInChildren<Text>();
        slowTimeBackground = GameObject.FindGameObjectWithTag("SlowTime").GetComponentsInChildren<Image>()[0];
        slowTimeFill = GameObject.FindGameObjectWithTag("SlowTime").GetComponentsInChildren<Image>()[1];

        reloadingText = GameObject.FindGameObjectWithTag("Reload").GetComponentInChildren<Text>();
        
        bulletCount = GameObject.FindGameObjectWithTag("BulletCount").GetComponentInChildren<Text>();
    }

    public void SetCurrentHealth(int _currentHealth, int _maxHealth = 100)
    {
        healthSlider.maxValue = _maxHealth;
        healthSlider.value = _currentHealth;
        healthText.text = _currentHealth.ToString();
        healthFill.color = gradient.Evaluate(healthSlider.normalizedValue);
    }

    public void setSlowMoTime(float _slowMoTime, float _maxSlowMoTime = 50)
    {
        enableSlowTime();

        slowTimeSlider.maxValue = _maxSlowMoTime;
        slowTimeSlider.value = _slowMoTime;
        slowTimeText.text = _slowMoTime.ToString("0.0");
        slowTimeFill.color = gradient.Evaluate(slowTimeSlider.normalizedValue);
    }

    void enableSlowTime()
    {
        slowTimeSlider.enabled = true;
        slowTimeFill.enabled = true;
        slowTimeText.enabled = true;
        slowTimeBackground.enabled = true;
    }
    public void disableSlowTime()
    {
        slowTimeSlider.enabled = false;
        slowTimeFill.enabled = false;
        slowTimeText.enabled = false;
        slowTimeBackground.enabled = false;
    }

    public void setReloading(string msg="Reloading")
    {
        reloadingText.text = msg;
    }

    public void setBulletCount(int _bulletCount)
    {
        bulletCount.text = _bulletCount.ToString();
    }

}
