using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyStaticFriction : MonoBehaviour
{
    public float friction = 1;
    public bool applyOnContact = true;

    private Rigidbody rb;

    public bool isMakingContact { get; protected set; }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(applyOnContact)
        {
            if (isMakingContact)
                ApplyDrag();
        }
        else
        {
            ApplyDrag();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isMakingContact = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isMakingContact = false;
    }

    private void ApplyDrag()
    {
        if (rb == null) return;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;

        float coefficientOfFriction = friction / 100;

        rb.AddForce(-velocity * coefficientOfFriction, ForceMode.Impulse);
    }
}
