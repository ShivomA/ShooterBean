using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject BlastEffect;
    public float explosionRadius = 5, explosionForce = 10000;
    public int explosionDamage = 40;
    [SerializeField] string MissileTag, playerTag, enemyTag="Enemy";
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (MissileTag.Equals("")) MissileTag = FindObjectOfType<HoamingMissile>().tag;
        if (playerTag.Equals("")) playerTag = FindObjectOfType<Player>().tag;
        Invoke(nameof(Explode), 3);
    }

    private void FixedUpdate()
    {
        rb.AddForce(35 * Vector3.up);
    }

    void Explode()
    {
        BlastEffect = Instantiate(BlastEffect, transform.position, Quaternion.identity);
        Destroy(BlastEffect, BlastEffect.GetComponent<ParticleSystem>().main.duration - 0.1f);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (rb.tag.Equals(playerTag))
                    rb.GetComponent<Player>().TakeDamage(explosionDamage);
                if (rb.tag.Equals(enemyTag))
                    rb.GetComponent<Enemy>().TakeDamage(explosionDamage);
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        string colliderTag = collision.collider.tag;
        if (colliderTag.Equals(MissileTag)) collision.collider.GetComponent<HoamingMissile>().destroy();
        if (colliderTag.Equals(enemyTag)) Explode();
    }
}
