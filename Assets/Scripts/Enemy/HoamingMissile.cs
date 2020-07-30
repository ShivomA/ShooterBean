using UnityEngine;

public class HoamingMissile : MonoBehaviour
{
    public GameObject blastEffect;
    public EnemyStats stats;
    [HideInInspector]
    public int currentHealth, maxHealth = 50;
    
    public float explosionRadius = 5;
    public float explosionForce = 10000;

    private Transform target;
    private Rigidbody rb;
    private ParticleSystem ps;

    [SerializeField] float speed = 25, rotateSpeed = 5;

    private void Awake()
    {
        FindObjectOfType<AudioManager>().Play("MissileLaunch", 0.7f);
        target = FindObjectOfType<MovementController>().transform;
        stats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody>();
        ps = blastEffect.GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        Invoke(nameof(AddInitialForce), 1);
    }

    void AddInitialForce()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = true;
        rb.AddForce(-transform.forward * 10, ForceMode.Impulse);
        
        Invoke(nameof(ChaseTarget), 1f);
        Invoke(nameof(destroy), 50);
    }

    void ChaseTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 rotateAmount = Vector3.Cross(direction, transform.forward);
        rb.angularVelocity = rotateAmount * rotateSpeed;
        rb.velocity = -transform.forward * speed;
        Invoke(nameof(ChaseTarget), .1f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        stats.SetCurrentHealth(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            GameObject tempParticleEffect = Instantiate(blastEffect, transform.position, transform.rotation);
            Destroy(tempParticleEffect, tempParticleEffect.GetComponent<ParticleSystem>().main.duration);
            stats.DisableHealthBar();
            Destroy(gameObject);


            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider nearbyObject in colliders)
            {
                rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }

        }
    }

    public void destroy()
    {
        blastEffect = Instantiate(blastEffect, transform.position, transform.rotation);
        Destroy(blastEffect, ps.main.duration - 0.1f);
        Destroy(gameObject);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

    }
    
}
