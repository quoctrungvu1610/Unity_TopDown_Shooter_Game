using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainMissile_Boss : MonoBehaviour
{
    private Enemy_Boss enemy;

    [Header("References")]
    private Transform player;
    public Transform leftLauncher;
    public Transform rightLauncher;
    public GameObject rocketPrefab;

    [Header("Rain Settings")]
    public int minRocketsPerWave = 10;
    public int maxRocketsPerWave = 20;
        private int rocketsPerWave;
    public float delayBetweenShots = 0.1f;
    public float spreadRadius = 3f;

    [Header("Curve Settings")]
    public float curveStrength = 6f;
    public float flightTime = 1.2f;

    private void Start()
    {
        enemy = GetComponent<Enemy_Boss>();
        player = this.gameObject.GetComponent<Enemy_Boss>().player;
    }

    public void StartRocketRain()
    {
        StartCoroutine(RocketRainCoroutine());
    }

    IEnumerator RocketRainCoroutine()
    {
        int randomRocketCount = Random.Range(minRocketsPerWave, maxRocketsPerWave + 1);
        for (int i = 0; i < randomRocketCount; i++)
        {
            SpawnFromBothSides();
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    void SpawnFromBothSides()
    {

        Vector2 offset = Random.insideUnitCircle * spreadRadius;
        Vector3 targetPos = player.position + new Vector3(offset.x, 0f, offset.y);

        enemy.bossVisual.PlaceMissileLandingZone(targetPos, flightTime);

        SpawnRocket(leftLauncher, targetPos, -1f);

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
