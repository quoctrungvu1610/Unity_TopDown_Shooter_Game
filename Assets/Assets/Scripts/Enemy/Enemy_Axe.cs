using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Axe : MonoBehaviour
{
    [SerializeField] GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;

    private Transform player;
    private float flySpeed;
    private float rotationSpeed = 1600;
    private Vector3 direction;
    private float timer = 1;

    public void AxeSetup(float flySpeed, Transform player, float timer) 
    {
        rotationSpeed = 1600;
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }

    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        timer -= Time.deltaTime;
        if (timer > 0f) 
        {
            direction = player.position + Vector3.up - transform.position;
        }

        transform.forward = rb.velocity;
    }

    private void FixedUpdate()
    {
        rb.velocity = direction.normalized * flySpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage();

        GameObject newFx = ObjectPool.instance.GetObject(impactFx, transform);
        newFx.transform.position = transform.position;

        ObjectPool.instance.ReturnObject(gameObject);
        ObjectPool.instance.ReturnObject(newFx, 1);
    }
}
