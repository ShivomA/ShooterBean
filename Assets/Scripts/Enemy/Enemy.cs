using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject bullet, particleEffect;
    public Transform gunTip, player;

    private EnemyStats stats;
    private NavMeshAgent agent;
    private Rigidbody rb;

    private float attackRange = 100;
    private bool isAttacking = true;
    [HideInInspector]
    public int currentHealth, maxHealth = 100;

    private void Awake()
    {
        if (player == null)
            player = FindObjectOfType<Player>().transform;
        stats = GetComponent<EnemyStats>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        Invoke(nameof(Attack), 1);
    }

    private void Update()
    {
        if (transform.position.y < -100)
            TakeDamage(maxHealth);
        if (rb.velocity.magnitude >= agent.speed + 20)
            rb.velocity = Vector3.zero;
        if (currentHealth <= 30)
        {
            Potion potionObject = FindObjectOfType<Potion>();
            if (potionObject != null && Vector3.Distance(potionObject.transform.position, transform.position) < 100)
            {
                Transform potion = potionObject.transform;
                agent.enabled = true;
                agent.SetDestination(potion.position);
                CancelInvoke(nameof(Attack));
                isAttacking = false;
            }
            else
            {
                GotoPosition();
            }
        }
        else
        {
            GotoPosition();
        }
    }

    public void GotoPosition()
    {
        if ((player.position - transform.position).magnitude >= 20)
        {
            agent.enabled = true;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.enabled = false;
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 rotateAmount = Vector3.Cross(direction, transform.forward);
            rb.angularVelocity = rotateAmount * 5000;
        }
        if (!isAttacking)
        {
            isAttacking = true;
            Attack();
        }
    }

    public void Attack()
    {
        Vector3 direction = player.position - gunTip.position;
        float distance = direction.magnitude;
        RaycastHit hit;
        if (Physics.Raycast(gunTip.position, direction, out hit))
        {
            if (hit.collider.tag == player.tag && distance < attackRange)
            {
                GameObject tempBullet = Instantiate(bullet, gunTip.position, Quaternion.LookRotation(player.position - gunTip.position));
                tempBullet.transform.Rotate(Vector3.right, 90);
                tempBullet.GetComponent<EnemyBullet>().Fire(player.position - gunTip.position,  50 + rb.velocity.magnitude);
                Invoke(nameof(Attack), 5f);
            }
            else
            {
                Invoke(nameof(Attack), 1);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        stats.SetCurrentHealth(currentHealth, maxHealth);
        if(currentHealth <= 0)
        {
            GameObject tempParticleEffect = Instantiate(particleEffect, transform.position, transform.rotation);
            Destroy(tempParticleEffect, tempParticleEffect.GetComponent<ParticleSystem>().main.duration);
            stats.DisableHealthBar();
            Destroy(gameObject);
        }
    }

    public void IncreaseHealth(int health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        stats.SetCurrentHealth(currentHealth, maxHealth);
    }

}
