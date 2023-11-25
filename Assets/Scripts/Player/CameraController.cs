using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player;
    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<MovementController>().transform;
    }
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, 1, 0);
    }
}
