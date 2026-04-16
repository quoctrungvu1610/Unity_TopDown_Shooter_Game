using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image mainBar;
    [SerializeField] private Image redBar;

    [Header("Settings")]
    [SerializeField] private float shrinkSpeed = 2f;

    private float targetFillAmount = 1f;

    void Update()
    {
        if (redBar.fillAmount > mainBar.fillAmount)
        {
            redBar.fillAmount -= shrinkSpeed * Time.deltaTime;
        }
        else
        {
            redBar.fillAmount = mainBar.fillAmount;
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        targetFillAmount = currentHealth / maxHealth;

        mainBar.fillAmount = targetFillAmount;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
