using UnityEngine;

public class GunSwitch : MonoBehaviour
{
    public GameObject[] guns;
    public PlayerStats playerStats;
    public GrappleGun grappleGun;
    public GameObject bulletCount;

    int activeGun = 1;

    private void Awake()
    {
        if (grappleGun == null)
            grappleGun = FindObjectOfType<GrappleGun>();
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
        for (int i = 0; i < guns.Length; i++)
            if (guns[i].activeInHierarchy == true)
                activeGun = i;
    }

    private void Start()
    {
        playerStats.disableSlowTime();
        SwitchGun(activeGun);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            SwitchGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchGun(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchGun(3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchGun(4);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            SwitchGun(5);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            SwitchGun(6);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            SwitchGun(7);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            SwitchGun(8);
    }

    void SwitchGun(int inx)
    {
        if (!Input.GetMouseButton(0))
        {
            if (grappleGun.slowMo)
            {
                grappleGun.slowMotion();
            }
            if (activeGun == 0) playerStats.disableSlowTime();
            if (inx == 0) bulletCount.SetActive(false);
            else bulletCount.SetActive(true);
            guns[activeGun].SetActive(false);
            guns[inx].SetActive(true);
            activeGun = inx;
        }
    }
}
