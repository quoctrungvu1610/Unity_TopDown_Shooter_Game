using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float impactForce;

    private BoxCollider cd;
    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;


    [SerializeField] private GameObject bulletImpactFX;
   
    private Vector3 startPosition;
    private float flyDistance;
    public bool bulletDisabled = false;


    private int bulletDamage;


    private LayerMask allyLayerMask;

    protected virtual void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BulletSetup(LayerMask allyLayer, float flyDistance = 100, float impactForce = 100, int damage = 1)
    {
        this.impactForce = impactForce;
        this.allyLayerMask = allyLayer;
        this.bulletDamage = damage;

        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.Clear();
        trailRenderer.time = 0.2f;

        startPosition = transform.position;
        this.flyDistance = flyDistance;
    }

    protected virtual void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    protected void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0f)
        {
            ReturnBulletToPool();
        }
    }

    protected void ReturnBulletToPool(float delay = 0)
    {
        ObjectPool.instance.ReturnObject(gameObject, delay);
    }

    protected void DisableBulletIfNeeded()
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
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 2f)
        {
            trailRenderer.time -= 2 * Time.deltaTime;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (FriendlyFireEnabled() == false)
        {
            if((allyLayerMask.value & (1 << collision.gameObject.layer)) > 0)
            {
                ReturnBulletToPool(1);
                return;
            }
        }

        CreateImpactFX();
        ReturnBulletToPool();

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage(bulletDamage);
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

        ApplyBulletImpactToEnemy(collision);
    }

    private void ApplyBulletImpactToEnemy(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRigidBody = collision.collider.attachedRigidbody;

            enemy.BulletImpact(force, collision.contacts[0].point, hitRigidBody);
        }
    }

    protected void CreateImpactFX()
    {
            GameObject impactFX = ObjectPool.instance.GetObject(bulletImpactFX, transform);
            ObjectPool.instance.ReturnObject(impactFX, 1f);
    }

    private bool FriendlyFireEnabled()
    {
        return GameManager.instance.friendlyFire;
    }
}
