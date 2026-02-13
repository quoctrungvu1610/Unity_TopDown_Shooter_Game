using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainMissile_Boss : MonoBehaviour
{
    [Header("References")]
    private Transform player;
    public Transform leftLauncher;
    public Transform rightLauncher;
    public GameObject rocketPrefab;

    [Header("Rain Settings")]
    public int rocketsPerWave = 12;
    public float delayBetweenShots = 0.1f;
    public float spreadRadius = 3f;

    [Header("Curve Settings")]
    public float curveStrength = 6f; // độ cong ngang
    public float flightTime = 1.2f;

    private void Start()
    {
        player = this.gameObject.GetComponent<Enemy_Boss>().player;
    }

    public void StartRocketRain()
    {
        StartCoroutine(RocketRainCoroutine());
    }

    IEnumerator RocketRainCoroutine()
    {
        for (int i = 0; i < rocketsPerWave; i++)
        {
            SpawnFromBothSides();
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    void SpawnFromBothSides()
    {
        // Random điểm rơi quanh player (tạo mưa tên lửa)
        Vector2 offset = Random.insideUnitCircle * spreadRadius;
        Vector3 targetPos = player.position + new Vector3(offset.x, 0f, offset.y);

        // Bắn từ tay trái (cong sang trái)
        SpawnRocket(leftLauncher, targetPos, -1f);

        // Bắn từ tay phải (cong sang phải)
        SpawnRocket(rightLauncher, targetPos, +1f);
    }

    void SpawnRocket(Transform launcher, Vector3 target, float curveSide)
    {
        GameObject rocket = Instantiate(rocketPrefab, launcher.position, Quaternion.identity);
        //GameObject rocket = ObjectPool.instance.GetObject(rocketPrefab, launcher);

        BossMissile rocketBezier = rocket.GetComponent<BossMissile>();
        rocketBezier.Init(
            launcher.position,
            target,
            curveStrength,
            flightTime,
            curveSide
        );
    }
}
