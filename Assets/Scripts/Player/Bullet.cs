using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] string playerTag, EnemyTag, EnemyGlasses, MissileTag;
    public GameObject particleEffect;
    public int damage = 20, criticalDamage = 50;

    private float destroyTime = 10;
    private bool canInstantiate = true;
    private int muzzleVelocity = 500;
    Vector3 direction;
    Rigidbody rb;

    void Start()
    {
        if (playerTag == "")
            playerTag = FindObjectOfType<Player>().tag;
        if (EnemyTag == "")
            EnemyTag = FindObjectOfType<Enemy>().tag;
        if (EnemyGlasses == "")
            EnemyGlasses = FindObjectOfType<Enemy>().GetComponentInChildren<BoxCollider>().tag;
        if (MissileTag == "")
            MissileTag = FindObjectOfType<HoamingMissile>().tag;
    }
    public void Fire(Vector3 _direction, int muzzleVelocity = 500)
    {
        rb = GetComponent<Rigidbody>();
        direction = _direction;
        rb.AddForce(direction.normalized * muzzleVelocity, ForceMode.VelocityChange);
        Invoke(nameof(speedBoost), 3);
    }
    void speedBoost()
    {
        rb.AddForce(direction.normalized * muzzleVelocity, ForceMode.VelocityChange);
        Invoke(nameof(speedBoost), 3);
    }

    void Update()
    {
        destroy();
    }

    void destroy()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime <= 0)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        string colliderTag = collision.collider.tag;
        Enemy collider0 = collision.collider.gameObject.GetComponentInParent<Enemy>();
        if (colliderTag == EnemyGlasses)
        {
            collider0.TakeDamage(criticalDamage);
        }
        Enemy collider = collision.collider.gameObject.GetComponent<Enemy>();
        if (colliderTag == EnemyTag)
        {
            collider.TakeDamage(damage);
        }
        HoamingMissile collider2 = collision.collider.gameObject.GetComponent<HoamingMissile>();
        if (colliderTag == MissileTag)
        {
            collider2.TakeDamage(damage);
        }

        if ((colliderTag == "Untagged" || (colliderTag != this.tag && colliderTag != playerTag)) && canInstantiate)
        {
            particleEffect = Instantiate(particleEffect, transform.position, transform.rotation);
            Destroy(particleEffect, particleEffect.GetComponent<ParticleSystem>().main.duration - 0.1f);
            GetComponent<Rigidbody>().useGravity = true;
            canInstantiate = false;
            Destroy(gameObject, 0.5f);
        }
        Destroy(gameObject);
    }

}
