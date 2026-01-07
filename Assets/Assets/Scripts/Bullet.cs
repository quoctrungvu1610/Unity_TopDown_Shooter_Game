using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private BoxCollider cd;
    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;

    [SerializeField] private GameObject bulletImpactFX;
   
    private Vector3 startPosition;
    private float flyDistance;
    public bool bulletDisabled = false;

    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BulletSetup(float flyDistance)
    {
        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;
        trailRenderer.time = 0.2f;

        startPosition = transform.position;
        this.flyDistance = flyDistance;
    }

    private void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0f)
        {
            trailRenderer.time = 0f;
            ReturnBulletToPool();
        }
    }

    private void ReturnBulletToPool()
    {
        ObjectPool.instance.ReturnObject(gameObject);
    }

    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
        {
            trailRenderer.time -= 2 * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);
        ReturnBulletToPool();
    }

    private void CreateImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0) 
        {
            ContactPoint contact = collision.contacts[0];
            //Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            //Vector3 pos = contact.point;
            //GameObject impactFX = Instantiate(bulletImpactFX, pos, rot);

            GameObject impactFX = ObjectPool.instance.GetObject(bulletImpactFX);
            impactFX.transform.position = contact.point;
            
            ObjectPool.instance.ReturnObject(impactFX, 1f);
        }
    }
}
