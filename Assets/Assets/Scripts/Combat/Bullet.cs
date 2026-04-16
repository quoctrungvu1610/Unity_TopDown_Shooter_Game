using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(TrailRenderer))]
public class Bullet : MonoBehaviour
{
    protected float impactForce;
    protected BoxCollider cd;
    protected Rigidbody rb;
    protected TrailRenderer trailRenderer;
    protected MeshRenderer meshRenderer;
    protected Vector3 startPosition;
    protected float flyDistance;
    protected bool bulletDisabled = false;
    protected int bulletDamage;
    protected LayerMask allyLayerMask;
    protected Vector3 bulletDirection;
    protected Transform origin;
    [SerializeField] private GameObject bulletImpactFX;


    protected virtual void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public virtual void BulletSetup(LayerMask allyLayer, BulletData data, Vector3 direction, Transform point)
    {
        this.allyLayerMask = allyLayer;

        this.impactForce = data.GetImpactForce();
        this.bulletDamage = data.GetBulletDamage();
        this.flyDistance = data.GetFlyDistance();
        this.bulletDirection = direction.normalized;
        this.origin = point;

        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.Clear();
        trailRenderer.time = 0.2f;

        startPosition = transform.position;
    }

    protected virtual void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    protected virtual void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0f)
        {
            ReturnBulletToPool();
        }
    }

    protected virtual void ReturnBulletToPool(float delay = 0)
    {
        ObjectPool.instance.ReturnObject(gameObject, delay);
    }

    protected virtual void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    protected virtual void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1f)
        {
            trailRenderer.time -= 1 * Time.deltaTime;
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
        ContactPoint contact = collision.contacts[0];
        CreateImpactFX(contact.point, contact.normal);
        ReturnBulletToPool();

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage(bulletDamage);
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

        ApplyBulletImpactToEnemy(collision);
    }

    protected virtual void ApplyBulletImpactToEnemy(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRigidBody = collision.collider.attachedRigidbody;

            enemy.BulletImpact(force, collision.contacts[0].point, hitRigidBody);
        }
    }

    protected virtual void CreateImpactFX(Vector3 pos, Vector3 normal)
    {
        GameObject impactFX = ObjectPool.instance.GetObject(bulletImpactFX, transform);

        impactFX.transform.position = pos;

        impactFX.transform.rotation = Quaternion.LookRotation(normal);

        ObjectPool.instance.ReturnObject(impactFX, 1f);
    }


    private bool FriendlyFireEnabled()
    {
        return GameManager.instance.friendlyFire;
    }
}
