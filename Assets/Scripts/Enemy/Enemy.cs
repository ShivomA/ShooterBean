using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject bullet, particleEffect;
    public Transform gunTip, player;

    private EnemyStats stats;
    private NavMeshAgent agent;
    private LineRenderer lr;
    private Rigidbody rb;

    private float attackRange = 100;
    [HideInInspector]
    public int currentHealth, maxHealth = 100;

    private void Awake()
    {
        if (player == null)
            player = FindObjectOfType<Player>().transform;
        stats = GetComponent<EnemyStats>();
        agent = GetComponent<NavMeshAgent>();
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        Invoke(nameof(Attack), 1);
    }

    private void Update()
    {
        agent.SetDestination(player.position);
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
                tempBullet.GetComponent<EnemyBullet>().Fire(player.position - gunTip.position,  10 + rb.velocity.magnitude);
                Invoke(nameof(Attack), 5f);
                if (rb.velocity.magnitude >= agent.speed + 2)
                    rb.velocity = Vector3.zero;
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

}
