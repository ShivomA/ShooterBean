using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BoxBase : MonoBehaviour
{
    public string playerTag;
    public GameObject Enemy, healthPotion, missile;

    void Start()
    {
        if (playerTag.Equals("")) playerTag = FindObjectOfType<Player>().tag;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals(playerTag))
        {
            Vector3 position = transform.position + new Vector3(Random.Range(-20.0f, 20.0f), 1.5f, Random.Range(-20.0f, 20.0f));

            int randomNum = Random.Range(0, 10);
            if (randomNum <= 2)
                Instantiate(healthPotion, position, Quaternion.identity);
            else if (randomNum >= 8)
                Instantiate(missile, position, Quaternion.identity);
            else
                Instantiate(Enemy, position, Quaternion.identity);
        }
    }
}
