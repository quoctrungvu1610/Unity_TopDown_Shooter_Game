using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionFx;
    [SerializeField] private float impactRadius;
    [SerializeField] private float upwardMultiplier = 1;
    [SerializeField] private int grenadeDamage = 10;
    private Rigidbody rb;
    private float timer;
    private float impactPower;


    private LayerMask allyLayerMask;
    private bool canExplode;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0 && canExplode) 
        {
            Explode();
        }
    }

    public void SetupGrenade(LayerMask allyLayerMask, Vector3 target, float timeToTarget, float countdown, float impactPower) 
    {
        canExplode = true;

        this.allyLayerMask = allyLayerMask;
        rb.velocity = CalculateLaunchVelocity(target, timeToTarget);
        timer = countdown + timeToTarget;

        this.impactPower = impactPower;
    }

    private bool IsTargetValid(Collider collider) 
    {
        if (GameManager.instance.friendlyFire) 
        {
            return true;
        }
        //If Collider is in ally layer mask, return false
        if ((allyLayerMask.value & (1 << collider.gameObject.layer)) > 0)
        {
            return false;
        }
        return true;
        
    }

    private void Explode()
    {
        canExplode = false;

        PlayExplosionFx();

        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (Collider hit in colliders)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if(damageable != null) 
            {
                if (IsTargetValid(hit) == false)
                {
                    continue;
                }

                GameObject rootEntitiy = hit.transform.root.gameObject;
                if (uniqueEntities.Add(rootEntitiy) == false)
                {
                    continue;
                }
                damageable.TakeDamage(grenadeDamage);
            }
           
            ApplyPhysicalForceTo(hit);
        }
    }

    private void ApplyPhysicalForceTo(Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddExplosionForce(impactPower, transform.position, impactRadius, upwardMultiplier, ForceMode.Impulse);
        }
    }

    //private static void ApplyDamageTo(Collider hit)
    //{
    //    IDamageable damageable = hit.GetComponent<IDamageable>();
    //    damageable?.TakeDamage();
    //}

    private void PlayExplosionFx()
    {
        GameObject newFx = ObjectPool.instance.GetObject(explosionFx, transform);
        ObjectPool.instance.ReturnObject(newFx, 1);
        ObjectPool.instance.ReturnObject(gameObject);
    }

    private Vector3 CalculateLaunchVelocity(Vector3 target, float timeToTarget) 
    {
        Vector3 direction = target - transform.position;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);

        Vector3 velocityXZ = directionXZ / timeToTarget;
        float velocityY =
            (direction.y - (Physics.gravity.y * Mathf.Pow(timeToTarget, 2)) / 2) / timeToTarget;

        Vector3 launchVelocity = velocityXZ + Vector3.up * velocityY;
        return launchVelocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
