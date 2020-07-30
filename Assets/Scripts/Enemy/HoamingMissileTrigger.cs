using UnityEngine;

public class HoamingMissileTrigger : MonoBehaviour
{
    public Animator triggerAnimator;
    public GameObject missile;
    public string bulletTag = "Bullet", playerTag = "Player";
    
    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.collider.tag.Equals(bulletTag) || collision.collider.tag.Equals(playerTag)) && !triggerAnimator.GetBool("Launch"))
            generateMissile();
    }
    public void generateMissile()
    {
       Instantiate(missile, transform.position, Quaternion.identity);
        triggerAnimator.SetBool("Launch", true);
        Invoke(nameof(ResetAnimator), 1);
    }
    void ResetAnimator()
    {
        triggerAnimator.SetBool("Launch", false);
    }

}
