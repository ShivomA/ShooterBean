using UnityEngine;

public class Potion : MonoBehaviour
{
    public float speed = 5;
    public string playerTag="Player", enemyTag="Enemy";
    float x=45, y=0, z=0;

    private void Start()
    {
        if (playerTag.Equals("")) playerTag = FindObjectOfType<Player>().tag;
        if (enemyTag.Equals("")) enemyTag = FindObjectOfType<Enemy>().tag;
    }

    void Update()
    {
        y = (y + speed * Time.deltaTime * 50) % 360;
        transform.localRotation = Quaternion.Euler(x, y, z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(playerTag))
        {
            Player player = other.GetComponent<Player>();
            if (player.currentHealth >= player.maxHealth) return;
            player.IncreaseHealth(30);
            Destroy(gameObject);
        }
        if (other.tag.Equals(enemyTag))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy.currentHealth >= enemy.maxHealth) return;
            enemy.IncreaseHealth(30);
            Destroy(gameObject);
        }
    }
}
