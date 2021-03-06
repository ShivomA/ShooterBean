﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotation : MonoBehaviour
{
    public GrappleGun grappler;

    private Quaternion desiredRotation;
    private float rotationSpeed = 5f;
    Vector3 rotation;

    private void Update()
    {
        if (!grappler.isGrappling())
        {
            rotation = transform.parent.rotation.eulerAngles;
            desiredRotation = Quaternion.Euler(new Vector3(rotation.x + 270, rotation.y, rotation.z + 175));
        }
        else
        {
            rotation = grappler.GetGrapplePoint() - transform.position;
            desiredRotation = Quaternion.LookRotation(rotation);
            rotation = desiredRotation.eulerAngles;
            desiredRotation = Quaternion.Euler(new Vector3(rotation.x + 270, rotation.y, rotation.z + 175));
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);

    }
}
