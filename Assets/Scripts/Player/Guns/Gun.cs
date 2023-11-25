using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform cameraTransform, gunTip;
    public GameObject bullet, particleEffect;
    public PlayerStats playerStats;
    public Animator gunAnimator;

    [HideInInspector]
    public bool slowMo = false, isReloading = false;
    [HideInInspector]
    public float slowMoTime, maxSlowMoTime = 50f, refillRate = 1;
    [HideInInspector]
    public int bulletCount, maxBullet = 10;

    private LineRenderer lineRenderer;
    private float slowScale = 10;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (bullet == null)
            bullet = FindObjectOfType<Bullet>().gameObject;
        if (cameraTransform == null)
            cameraTransform = FindObjectOfType<CameraController>().transform;
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Start()
    {
        slowMoTime = maxSlowMoTime;
        bulletCount = maxBullet;
        playerStats.setBulletCount(bulletCount);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) fire();
        if (Input.GetKeyDown(KeyCode.E)) slowMotion();
        if (Input.GetKeyDown(KeyCode.R))
        {
            GetComponent<Animator>().enabled = true;
            isReloading = true;
            Invoke(nameof(Reload), 1f);
        }
        if (slowMo)
        {
            slowMoTime -= Time.deltaTime * slowScale;
            if(slowMoTime <= 0) slowMotion();
        }
        else
        {
            slowMoTime += Time.deltaTime * refillRate;
            if(slowMoTime >= maxSlowMoTime) slowMoTime = maxSlowMoTime;
        }
    }

    Vector3 hitPosition;
    bool isHit;
    float lrTime = 0.5f;
    void fire()
    {
        if (bulletCount == 0 || isReloading )
            return;

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position + cameraTransform.forward, cameraTransform.forward, out hit))
        {
            isHit = true;
            hitPosition = hit.point;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, gunTip.position);
            lineRenderer.SetPosition(1, hitPosition);
            CancelInvoke(nameof(disableLineRender));
            Invoke(nameof(disableLineRender), lrTime);
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
            GetComponent<Animator>().enabled = true;
            Invoke(nameof(Reload), 1f);
        }

        GameObject tempBullet =  Instantiate(bullet, gunTip.position, gunTip.rotation);
        tempBullet.GetComponent<Bullet>().Fire(fireDirection());
        GameObject tempParticleEffect = Instantiate(particleEffect, gunTip.position, gunTip.rotation);
        Destroy(tempParticleEffect, tempParticleEffect.GetComponent<ParticleSystem>().main.duration);
    }
    public void disableLineRender()
    {
        lineRenderer.positionCount = 0;
    }
    public void Reload()
    {
        GetComponent<Animator>().enabled = false;
        bulletCount = maxBullet;
        isReloading = false;
        playerStats.setBulletCount(maxBullet);
    }

    void slowMotion()
    {
        if (slowMo)
        {
            Time.timeScale = 1;
            slowMo = !slowMo;
        }
        else
        {
            Time.timeScale = 1 / slowScale;
            slowMo = !slowMo;
        }
    }

    public Vector3 fireDirection()
    {
        if (isHit)
            return hitPosition - gunTip.position;
        else
            return hitPosition;
    }

}
