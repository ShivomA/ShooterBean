using UnityEngine;

public class HoamingMiissileTip : MonoBehaviour
{
    public GameObject hoamingMisssile, particleEffect;
    public float explosionRadius = 5, explosionForce = 10000;
    public int explosionDamage = 40;
    public string playerTag, enemyTag="Enemy";

    private void Start()
    {
        Invoke(nameof(enableCollider), 1f);
        if (playerTag.Equals("")) playerTag = FindObjectOfType<Player>().tag;
    }

    void enableCollider()
    {
        GetComponent<SphereCollider>().enabled = true;
    }

    private void FixedUpdate()
    {
        transform.position = hoamingMisssile.transform.position - hoamingMisssile.transform.forward * 4f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Base")) return;
        particleEffect = Instantiate(particleEffect, transform.position, Quaternion.identity);
        Destroy(particleEffect, particleEffect.GetComponent<ParticleSystem>().main.duration - 0.1f);
        FindObjectOfType<AudioManager>().Play("MissileExplosion");
        hoamingMisssile.GetComponent<HoamingMissile>().stats.DisableHealthBar();
        Destroy(hoamingMisssile);

        Collider[] colliders =  Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                if (rb.tag.Equals(playerTag))
                    rb.GetComponent<Player>().TakeDamage(explosionDamage);
                if (rb.tag.Equals(enemyTag))
                    rb.GetComponent<Enemy>().TakeDamage(explosionDamage);
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

}
