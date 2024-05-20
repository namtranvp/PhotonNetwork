using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    GameController controller;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse);
    }

    public void SetControl(GameController controller)
    {
        this.controller = controller;
    }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

    public Vector3 GetVelocity() => rb.velocity;

    public float GetSpeed() => rb.velocity.magnitude;

    public void SetVel(Vector3 vel) => rb.velocity = vel;

    public void SetZeroVel(bool iskinematic) => rb.isKinematic = iskinematic;
}
