using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject BlastEffect;
    public float explosionRadius = 5;
    public float explosionForce = 10000;
    [SerializeField] string MissileTag;

    private void Start()
    {
        if (MissileTag == null)
            MissileTag = FindObjectOfType<HoamingMissile>().tag;
        Invoke(nameof(Explode), 3);
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
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        string colliderTag = collision.collider.tag;
        HoamingMissile collider = collision.collider.GetComponent<HoamingMissile>();
        if (colliderTag == MissileTag)
        {
            collider.destroy();
        }
    }
}
