using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BossVisual : MonoBehaviour
{
    private Enemy_Boss enemy;

    [SerializeField] private float landingOffset = 1;
    [Header("Batteries")]
    [SerializeField] private GameObject[] batteries;
    [SerializeField] private float initialatteryScaleY = 0.2f;

    [Header("Fxs")]
    [SerializeField] private ParticleSystem landingZoneFx;
    [SerializeField] private ParticleSystem missileLandingFx;

    private float dischargeSpeed;
    private float rechargeSpeed;

    private bool isRecharging;

    private void Awake()
    {
        enemy = GetComponent<Enemy_Boss>();

        landingZoneFx.transform.parent = null;
        landingZoneFx.Stop();

        ResetBatteries();
    }

    private void Update() 
    {
        UpdateBatteriesScale();
    }

    public void PlaceLandingZone(Vector3 target) 
    {
        Vector3 dir = target - transform.position;
        Vector3 offset = dir.normalized * landingOffset;
        landingZoneFx.transform.position = target + offset;
        landingZoneFx.Clear();

        var mainModule = landingZoneFx.main;
        mainModule.duration = enemy.travelTimeToTarget;

        landingZoneFx.Play();
    }

    public void PlaceMissileLandingZone(Vector3 target, float duration)
    {
        Debug.Log("Place Missile Landing Zone");
        GameObject newFx = ObjectPool.instance.GetObject(missileLandingFx.gameObject, transform);

        newFx.transform.position = target;

        var mainModule = landingZoneFx.main;
        mainModule.duration = duration;

        ObjectPool.instance.ReturnObject(newFx, duration);
    }


    private void UpdateBatteriesScale() 
    {
        if (batteries.Length <= 0) 
        {
            return;
        }

        foreach (GameObject battery in batteries) 
        {
            if (battery.activeSelf) 
            {
                float scaleChange = (isRecharging ? rechargeSpeed : -dischargeSpeed) * Time.deltaTime;
                float newScaleY = Mathf.Clamp(battery.transform.localScale.y + scaleChange, 0, initialatteryScaleY);

                battery.transform.localScale = new Vector3(battery.transform.localScale.x, newScaleY, battery.transform.localScale.z);

                if(battery.transform.localScale.y <= 0) 
                {
                    battery.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ResetBatteries() 
    {
        isRecharging = true;
        rechargeSpeed = initialatteryScaleY / enemy.abilityCooldown;
        dischargeSpeed = initialatteryScaleY / (enemy.flamethrowDuration * 0.75f);

        foreach (GameObject battery in batteries) 
        {
            battery.gameObject.SetActive(true);
        }
    }

    public void DischargeBatteries() 
    {
        isRecharging = false;
    }
}
