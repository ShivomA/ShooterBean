using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    public GameObject grenade;
    public Transform player;
    public float throwSpeed = 30;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<Player>().transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ThrowGrenade();
    }

    void ThrowGrenade()
    {
        GameObject tempGrenade = Instantiate(grenade, player.position + player.forward, transform.rotation);
        Rigidbody rb = tempGrenade.GetComponent<Rigidbody>();
        Rigidbody rb2 = player.GetComponent<Rigidbody>();
        if ((rb2.velocity + player.forward).magnitude > rb2.velocity.magnitude)
            rb.AddForce(transform.forward * (throwSpeed + rb2.velocity.magnitude), ForceMode.VelocityChange);
        else
            rb.AddForce(transform.forward * throwSpeed, ForceMode.VelocityChange);
    }

}
