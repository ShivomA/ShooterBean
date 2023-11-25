using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    public Gradient gradient;

    private Slider slider;
    private Text healthText;
    private Image fill;
    private Image background;

    private void Awake()
    {
        slider = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponent<Slider>();
        healthText = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponentInChildren<Text>();
        background = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponentsInChildren<Image>()[0];
        fill = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponentsInChildren<Image>()[1];
    }

    private void Start()
    {
        DisableHealthBar();
    }

    public void SetCurrentHealth(int currentHealth, int maxHealth = 100)
    {
        EnableHealthBar();

        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        fill.color = gradient.Evaluate(slider.normalizedValue);

        CancelInvoke(nameof(DisableHealthBar));
        Invoke(nameof(DisableHealthBar), 1);
    }

    void EnableHealthBar()
    {
        slider.enabled = true;
        fill.enabled = true;
        background.enabled = true;
        healthText.enabled = true;
    }
    public void DisableHealthBar()
    {
        slider.enabled = false;
        fill.enabled = false;
        background.enabled = false;
        healthText.enabled = false;
    }

}
