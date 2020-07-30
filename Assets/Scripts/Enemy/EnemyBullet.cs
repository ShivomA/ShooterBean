using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] string playerTag, EnemyTag;
    public GameObject particleEffect;

    private float destroyTime = 10;
    private bool canInstantiate = true;
    private int muzzleVelocity = 50;
    Vector3 direction;
    Rigidbody rb;

    void Start()
    {
        if (playerTag == null)
            playerTag = FindObjectOfType<Player>().tag;
        if (EnemyTag == null)
            EnemyTag = FindObjectOfType<Enemy>().tag;
    }
    public void Fire(Vector3 _direction, float muzzleVelocity = 5)
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
        Player collider = collision.collider.gameObject.GetComponent<Player>();
        if (colliderTag == playerTag)
        {
            collider.TakeDamage(10);
        }

        if ((colliderTag == "Untagged" || (colliderTag != this.tag && colliderTag != EnemyTag)) && canInstantiate)
        {
            particleEffect = Instantiate(particleEffect, transform.position, transform.rotation);
            Destroy(particleEffect, particleEffect.GetComponent<ParticleSystem>().main.duration - 0.1f);
            GetComponent<Rigidbody>().useGravity = true;
            canInstantiate = false;
            Destroy(gameObject, 0.5f);
        }
    }

}
