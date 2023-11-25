using System.Threading.Tasks;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public Transform cameraTransform, gunTip, myTransform;
    public GameObject bullet, particleEffect;
    public PlayerStats playerStats;
    //public Animator gunAnimator;

    int bulletCount, maxBullet = 8, muzzleVelocity = 350;
    bool isReloading = false, canFire = true;
    float reloadingTime = 3.5f, recoilDuration = 1f;
    int damage = 45, criticalDamage = 100;

    float xSpread = 0.005f, ySpread = 0.012f, zSpread = 0, gunAngle = 0;
    float spreadAngleX = 0, spreadAngleY = 0, spreadAngleZ = 0;
    float recoilAngle = -20, recoilStep = -2.5f, maxRecoil = 100;
    int recoilDelay = 20;

    MovementController controller;
    Camera cam;

    void Awake()
    {
        controller = FindObjectOfType<MovementController>();
        myTransform = GetComponent<Transform>();
        cam = GetComponentInParent<Camera>();
        if (bullet == null)
            bullet = FindObjectOfType<Bullet>().gameObject;
        if (cameraTransform == null)
            cameraTransform = FindObjectOfType<CameraController>().transform;
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Start()
    {
        bulletCount = maxBullet;
        playerStats.setBulletCount(bulletCount);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            fire();
        if (Input.GetKeyDown(KeyCode.R))
        {
            //GetComponent<Animator>().enabled = true;
            isReloading = true;
            playerStats.setReloading("Reloading");
            Invoke(nameof(Reload), reloadingTime);
        }
    }

    Vector3 hitPosition;
    bool isHit;
    //float lrTime = 0.5f;

    async void recoil(string dir, float totalAngle)
    {
        await Task.Delay(recoilDelay);
        if (dir == "up")
        {
            myTransform.Rotate(recoilStep, 0, 0);
            totalAngle += recoilStep;
            gunAngle += recoilStep;
            if (totalAngle < recoilAngle)
                recoil("down", totalAngle);
            else
                recoil("up", totalAngle);
        }
        else if (dir == "down")
        {
            myTransform.Rotate(-recoilStep, 0, 0);
            totalAngle -= recoilStep;
            gunAngle -= recoilStep;
            if (myTransform.localEulerAngles.x < maxRecoil)
            {
                myTransform.eulerAngles = cameraTransform.eulerAngles;
                gunAngle = 0;
                return;
            }
            if (totalAngle > 0)
                return;
            else
                recoil("down", totalAngle);
        }
    }
    void fire()
    {
        if (bulletCount == 0 || isReloading || !canFire)
            return;

        recoil("up", 0);
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position + cameraTransform.forward, cameraTransform.forward, out hit))
        {
            isHit = true;
            hitPosition = hit.point;
        }
        else
        {
            isHit = false;
            hitPosition = cameraTransform.forward;
        }

        FindObjectOfType<AudioManager>().Play("GunShot");
        canFire = false;
        Invoke(nameof(canFireNow), recoilDuration);

        bulletCount -= 1;
        playerStats.setBulletCount(bulletCount);
        if (bulletCount == 0)
        {
            playerStats.setReloading("Reloading");
            //GetComponent<Animator>().enabled = true;
            Invoke(nameof(Reload), reloadingTime);
        }

        GameObject tempBullet = Instantiate(bullet, gunTip.position, gunTip.rotation);
        Bullet tempBulletScript = tempBullet.GetComponent<Bullet>();
        tempBulletScript.damage = damage;
        tempBulletScript.criticalDamage = criticalDamage;
        tempBulletScript.Fire(fireDirection(), muzzleVelocity);
        GameObject tempParticleEffect = Instantiate(particleEffect, gunTip.position, gunTip.rotation);
        Destroy(tempParticleEffect, tempParticleEffect.GetComponent<ParticleSystem>().main.duration);
    }
    public void canFireNow()
    {
        canFire = true;
    }
    public void Reload()
    {
        //GetComponent<Animator>().enabled = false;
        bulletCount = maxBullet;
        isReloading = false;
        playerStats.setBulletCount(maxBullet);
        playerStats.setReloading("");
    }
    public Vector3 fireDirection()
    {
        spreadAngleX = Random.Range(-gunAngle, gunAngle) * xSpread;
        spreadAngleY = Random.Range(-gunAngle * 0.2f, gunAngle) * ySpread;
        spreadAngleZ = Random.Range(-gunAngle, gunAngle) * zSpread;
        if (isHit)
            return (hitPosition - gunTip.position).normalized - new Vector3(spreadAngleX, spreadAngleY, spreadAngleZ);
        else
            return hitPosition - new Vector3(spreadAngleX, spreadAngleY, spreadAngleZ);
    }
}