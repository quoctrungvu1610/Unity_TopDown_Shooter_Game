using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    [Header("UI Groups & Canvas")]
    public CanvasGroup crosshairCanvasGroup;
    public GameObject normalGroup;
    public Image imageHit;
    public Image imageReload;
    public Image imageKill;

    [Header("4-Part Crosshair (Normal)")]
    public RectTransform topPart;
    public RectTransform bottomPart;
    public RectTransform leftPart;
    public RectTransform rightPart;

    [Header("Spread Settings (Gap)")]
    public float minGap = 10f;
    public float maxGap = 60f; 
    public float spreadPerShot = 8f;
    public float restoreSpeed = 25f;

    [Header("Effect Durations")]
    public float hitDuration = 0.1f;
    public float killDuration = 0.3f;
    public float killFlashSpeed = 12f;

    [Header("Reload Settings")]
    public float reloadRotationAngle = -45f;
    public float rotationSpeed = 10f;

    private Coroutine reloadCoroutine;
    private float targetRotationZ = 0f;

    private float currentGap;
    private Coroutine currentEffectCoroutine;
    private bool isReloading = false;
    float currentZ;
    float newZ;

    public static CrosshairManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        currentGap = minGap;
        ToggleCrosshair(true);
        ResetAllLayers();
    }

    void Update()
    {
        //if (!Cursor.visible)
        //{
        //}
        transform.position = Input.mousePosition;

        if (currentGap > minGap && isReloading == false)
        {
            currentGap -= restoreSpeed * Time.deltaTime;
            currentGap = Mathf.Max(minGap, currentGap);
        }

        currentZ = normalGroup.transform.localEulerAngles.z;
        newZ = Mathf.LerpAngle(currentZ, targetRotationZ, Time.deltaTime * rotationSpeed);
        normalGroup.transform.localEulerAngles = new Vector3(0, 0, newZ);

        ApplyGap(currentGap);
    }

    public void SetupGap(float minGap, float maxGap)
    {
        this.minGap = minGap;
        this.maxGap = maxGap;
        currentGap = minGap;

        ApplyGap(currentGap);
    }

    private void ApplyGap(float gap)
    {
        topPart.anchoredPosition = new Vector2(0, gap);
        bottomPart.anchoredPosition = new Vector2(0, -gap);
        leftPart.anchoredPosition = new Vector2(-gap, 0);
        rightPart.anchoredPosition = new Vector2(gap, 0);
    }

    public void ToggleCrosshair(bool isGaming)
    {
        if (isGaming)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            crosshairCanvasGroup.alpha = 1f;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            crosshairCanvasGroup.alpha = 0f;
        }
    }

    public void RegisterShotfired()
    {
        if (isReloading) return;
        currentGap += spreadPerShot;
        currentGap = Mathf.Min(currentGap, maxGap);
    }

    public void TriggerHit()
    {
        if (isReloading) return;
        StopActiveEffect();
        currentEffectCoroutine = StartCoroutine(HitRoutine());
    }

    public void TriggerKill()
    {
        if (isReloading) return;
        StopActiveEffect();
        currentEffectCoroutine = StartCoroutine(KillRoutine());
    }

    public void StartReload(float baseDuration, float multiplier)
    {
        normalGroup.SetActive(true);
        currentGap = maxGap;
        isReloading = true;
        targetRotationZ = reloadRotationAngle;

        // TÍNH TOÁN THỜI GIAN THỰC TẾ
        // Nếu multiplier = 2, animation chạy nhanh gấp đôi -> thời gian thực giảm 1 nửa
        float actualDuration = baseDuration / multiplier;

        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        reloadCoroutine = StartCoroutine(UpdateReloadProgressRoutine(actualDuration));
    }

    private IEnumerator UpdateReloadProgressRoutine(float duration)
    {
        imageReload.gameObject.SetActive(true);
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            imageReload.fillAmount = timer / duration;
            yield return null;
        }

        imageReload.fillAmount = 1f;
    }
    public void FinishReload()
    {
        isReloading = false;
        targetRotationZ = 0f;

        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        imageReload.gameObject.SetActive(false);
        normalGroup.SetActive(true);
    }

    private void HandleSpread()
    {
        if (currentGap > minGap && !isReloading)
        {
            currentGap -= restoreSpeed * Time.deltaTime;
            currentGap = Mathf.Max(minGap, currentGap);
        }
        ApplyGap(currentGap);
    }


    private IEnumerator HitRoutine()
    {
        imageHit.gameObject.SetActive(true);
        imageHit.transform.localScale = GetSpreadScale();

        yield return new WaitForSeconds(hitDuration);

        imageHit.gameObject.SetActive(false);
        if (!isReloading) normalGroup.SetActive(true);
    }

    private IEnumerator KillRoutine()
    {
        normalGroup.SetActive(false);
        imageKill.gameObject.SetActive(true);
        imageKill.transform.localScale = GetSpreadScale();

        Color originColor = imageKill.color;
        float timer = 0;
        while (timer < killDuration)
        {
            float lerp = Mathf.PingPong(Time.time * killFlashSpeed, 1f);
            imageKill.color = Color.Lerp(originColor, Color.red, lerp);
            timer += Time.deltaTime;
            yield return null;
        }

        imageKill.color = originColor;
        imageKill.gameObject.SetActive(false);
        if (!isReloading) normalGroup.SetActive(true);
    }

    private Vector3 GetSpreadScale()
    {
        float s = 1f + (currentGap / maxGap) * 0.2f;
        return new Vector3(s, s, 1f);
    }

    private void StopActiveEffect()
    {
        if (currentEffectCoroutine != null) StopCoroutine(currentEffectCoroutine);
        imageHit.gameObject.SetActive(false);
        imageKill.gameObject.SetActive(false);
    }

    private void ResetAllLayers()
    {
        normalGroup.SetActive(true);
        imageHit.gameObject.SetActive(false);
        imageReload.gameObject.SetActive(false);
        imageKill.gameObject.SetActive(false);
    }
}
