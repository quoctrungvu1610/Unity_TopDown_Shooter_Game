using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    public Vector3 randomizeIntensity = new Vector3(0, 0, 0);
    public float disappearTimer = 1f;

    private const float DISAPPEAR_TIMER_MAX = 1f;
    private Color textColor;
    private TextMeshProUGUI textMesh;
    private float disappearSpeed = 1f;
    private float increaseScaleAmount = 0.5f;

    void Start()
    {
        Setup();
    }

    public void Setup() 
    {
        disappearTimer = DISAPPEAR_TIMER_MAX;
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        textColor = textMesh.color;
        textColor.a = 1;
        textMesh.color = textColor;
        transform.localScale = Vector3.one * Random.Range(0.01f, 0.012f);
        transform.position += new Vector3(
            Random.Range(-randomizeIntensity.x, randomizeIntensity.x),
            Random.Range(-randomizeIntensity.y, randomizeIntensity.y),
            Random.Range(-randomizeIntensity.z, randomizeIntensity.z)
        );
    }

    void Update()
    {
        disappearTimer -= Time.deltaTime;
        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5f)
        {
            transform.localScale += new Vector3(0.003f, 0.003f, 0.003f) * increaseScaleAmount * Time.deltaTime;
        }
        else 
        {
            transform.localScale -= new Vector3(0.003f, 0.003f, 0.003f) * increaseScaleAmount * Time.deltaTime;
        }
        if (disappearTimer < 0f) 
        {
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0f) 
            {
                ObjectPool.instance.ReturnObject(gameObject);
            }
        }
        transform.position += offset * Time.deltaTime;

    }
}
