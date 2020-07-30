using UnityEngine;
using UnityEngine.UI;

public class GunSwitch : MonoBehaviour
{
    LineRenderer lr;
    GrappleGun grappleGun;
    GunRotation gunRotation;
    Gun gun;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        grappleGun = GetComponent<GrappleGun>();
        gunRotation = GetComponent<GunRotation>();
        gun = GetComponent<Gun>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !grappleGun.isGrappling() && !gun.isReloading)
            switchGun();
    }

    void switchGun()
    {
        if(gun.slowMo == true)
        {
            Time.timeScale = 1;
            gun.slowMo = false;
        }
        CancelInvoke(nameof(gun.disableLineRender));
        lr.positionCount = 0;

        GameObject.FindGameObjectWithTag("BulletCount").GetComponentInChildren<Text>().enabled = !gun.enabled;
        GameObject.FindGameObjectWithTag("BulletCount").GetComponentInChildren<Image>().enabled = !gun.enabled;
        gun.enabled = !gun.enabled;
        grappleGun.enabled = !grappleGun.enabled;
        gunRotation.enabled = !gunRotation.enabled;
    }

}
