using System.Threading.Tasks;
using UnityEngine;

public class AUG : MonoBehaviour
{
    public Transform cameraTransform, gunTip, myTransform;
    public GameObject bullet, particleEffect;
    MovementController controller;
    Camera cam;
    public PlayerStats playerStats;
    //public Animator gunAnimator;

    bool isReloading = false, canFire = true;
    float reloadingTime = 3f, zoomFactor = 1.75f;
    int bulletCount, maxBullet = 40, firingRate = 150, muzzleVelocity = 300;
    int damage = 25, criticalDamage = 40;

    float xSpread = 0.003f, ySpread = 0.008f, zSpread = 0, gunAngle = 0;
    float spreadAngleX = 0, spreadAngleY = 0, spreadAngleZ = 0;
    float recoilAngle = -7, recoilStep = -0.7f, maxRecoil = 25;
    int recoilDelay = 20;

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
        if (Input.GetMouseButtonDown(1))
            Zoom(1, zoomFactor, false);
        if (Input.GetMouseButtonUp(1))
            Zoom(1, 1, true);

        if (Input.GetMouseButtonDown(0) && canFire)
        {
            canFire = false;
            fire();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerStats.setReloading("Reloading");
            //GetComponent<Animator>().enabled = true;
            isReloading = true;
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
    async void fire()
    {
        if (bulletCount == 0 || isReloading)
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

        await Task.Delay(firingRate);
        if (Input.GetMouseButton(0))
            fire();
        else
        {
            canFire = true;
            return;
        }
    }
    public void Reload()
    {
        playerStats.setReloading("");
        canFire = true;
        //GetComponent<Animator>().enabled = false;
        bulletCount = maxBullet;
        isReloading = false;
        playerStats.setBulletCount(maxBullet);
    }
    void Zoom(float multiplier, float divider, bool reset)
    {
        if (!reset)
            myTransform.localPosition = new Vector3(0, -0.95f, -0.45f);
        if (reset)
            myTransform.localPosition = new Vector3(0.95f, -1, 0.43f);
        cam.fieldOfView *= multiplier;
        cam.fieldOfView /= divider;

        controller.sensMultiplier *= multiplier;
        controller.sensMultiplier /= divider;

        if (reset || cam.fieldOfView > 60)
        {
            cam.fieldOfView = 60;
            controller.sensMultiplier = 1;
        }
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