using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMissile : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 controlPoint;
    private Vector3 endPoint;

    private float duration;
    private float timer;

    [SerializeField] private GameObject explosionFx;
    [SerializeField] private float impactRadius;
    [SerializeField] private float upwardMultiplier = 1;
    [SerializeField] private float impactPower;

    [SerializeField] private LayerMask allyLayerMask;

    // curveSide: -1 = cong trái, +1 = cong phải
    public void Init(Vector3 start, Vector3 target, float curveStrength, float flightTime, float curveSide)
    {
        startPoint = start;
        endPoint = target;
        duration = flightTime;
        timer = 0f;

        Vector3 mid = (start + target) / 2f;

        // Hướng từ launcher -> player (trên mặt phẳng XZ)
        Vector3 dir = (target - start).normalized;
        dir.y = 0f;

        // Vector lệch ngang (vuông góc với hướng bay)
        Vector3 perpendicular = Vector3.Cross(Vector3.up, dir).normalized;

        // Nhân curveSide để phân biệt trái/phải
        Vector3 sideOffset = perpendicular * curveStrength * curveSide;

        // Control point lệch ngang (parabol ngang)
        controlPoint = mid + sideOffset;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // Quadratic Bezier
        Vector3 newPos =
            Mathf.Pow(1 - t, 2) * startPoint +
            2 * (1 - t) * t * controlPoint +
            Mathf.Pow(t, 2) * endPoint;

        // Xoay theo hướng bay (rất đẹp trong top-down)
        Vector3 direction = newPos - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        transform.position = newPos;

        if (t >= 1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        PlayExplosionFx();

        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (Collider hit in colliders)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable != null)
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
                damageable.TakeDamage();
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

    private void PlayExplosionFx()
    {
        GameObject newFx = ObjectPool.instance.GetObject(explosionFx, transform);
        ObjectPool.instance.ReturnObject(newFx, 1);
        gameObject.SetActive(false);
        Destroy(this.gameObject, 1);
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
}
