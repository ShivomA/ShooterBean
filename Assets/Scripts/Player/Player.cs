using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public int currentHealth, maxHealth = 100;

    private PlayerStats stats;

    private void Start()
    {
        stats = GetComponent<PlayerStats>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        stats.SetCurrentHealth(currentHealth, maxHealth);
    }

}
