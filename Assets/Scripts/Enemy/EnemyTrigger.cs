using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public Animator triggerAnimator;
    public GameObject Enemy;
    public string bulletTag = "Bullet", playerTag = "Player";

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.collider.tag.Equals(bulletTag) || collision.collider.tag.Equals(playerTag)) && !triggerAnimator.GetBool("Generate"))
            generateEnemy();
    }
    public void generateEnemy()
    {
        Instantiate(Enemy, transform.position, Quaternion.identity);
        triggerAnimator.SetBool("Generate", true);
        Invoke(nameof(ResetAnimator), 1);
    }
    void ResetAnimator()
    {
        triggerAnimator.SetBool("Generate", false);
    }

}
