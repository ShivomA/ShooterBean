using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public Gradient gradient;

    private Slider healthSlider;
    private Text healthText;
    private Image healthFill;

    private Slider slowMoSlider;
    private Text slowMoText;
    private Image slowMoFill;

    private Text bulletCount;

    private void Awake()
    {
        healthSlider = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
        healthText = GameObject.FindGameObjectWithTag("HealthBar").GetComponentInChildren<Text>();
        healthFill = GameObject.FindGameObjectWithTag("HealthBar").GetComponentsInChildren<Image>()[1];

        slowMoSlider = GameObject.FindGameObjectWithTag("SlowMo").GetComponent<Slider>();
        slowMoText = GameObject.FindGameObjectWithTag("SlowMo").GetComponentInChildren<Text>();
        slowMoFill = GameObject.FindGameObjectWithTag("SlowMo").GetComponentsInChildren<Image>()[1];
        
        bulletCount = GameObject.FindGameObjectWithTag("BulletCount").GetComponentInChildren<Text>();
    }

    public void SetCurrentHealth(int _currentHealth, int _maxHealth = 100)
    {
        healthSlider.maxValue = _maxHealth;
        healthSlider.value = _currentHealth;
        healthText.text = _currentHealth.ToString();
        healthFill.color = gradient.Evaluate(healthSlider.normalizedValue);
    }
    
    public void setSlowMoTime(float _slowMoTime)
    {
        float maxSlowMoTime = FindObjectOfType<Gun>().maxSlowMoTime;
        slowMoSlider.maxValue = maxSlowMoTime;
        slowMoSlider.value = _slowMoTime;
        slowMoText.text = _slowMoTime.ToString("0.0");
        slowMoFill.color = gradient.Evaluate(slowMoSlider.normalizedValue);
    }

    public void setBulletCount(int _bulletCount)
    {
        bulletCount.text = _bulletCount.ToString();
    }

}
