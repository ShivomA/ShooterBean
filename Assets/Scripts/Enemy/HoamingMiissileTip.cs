using UnityEngine;

public class HoamingMiissileTip : MonoBehaviour
{
    public GameObject hoamingMisssile, particleEffect;
    public float explosionRadius = 5;
    public float explosionForce = 10000;

    private void Start()
    {
        Invoke(nameof(enableCollider), 1f);
    }
    void enableCollider()
    {
        GetComponent<SphereCollider>().enabled = true;
    }

    private void FixedUpdate()
    {
        transform.position = hoamingMisssile.transform.position - hoamingMisssile.transform.forward * 3.4f;
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
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

    }

}
