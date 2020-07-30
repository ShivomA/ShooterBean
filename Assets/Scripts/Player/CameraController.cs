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
        transform.position = player.transform.position + new Vector3(0, 0.6f, .0f);
    }
}
