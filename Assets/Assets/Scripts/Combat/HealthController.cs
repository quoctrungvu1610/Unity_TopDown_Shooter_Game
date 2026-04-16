using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] protected GameObject damageTextPrefab;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void ReduceHealth(int damage) 
    {
        Debug.Log("Health reduced by: " + damage);
        currentHealth -= damage;
        SpawnDamageText(damage);
        if (currentHealth <= 0) 
        {
            currentHealth = 0;
            ShouldDie();
            TriggerDeadCrosshair();
            StartCoroutine(RemoveHealthBar());
        }

    }
    public virtual void IncreaseHealth() 
    {
        currentHealth++;

        if(currentHealth > maxHealth) 
        {
            currentHealth = maxHealth;
        }
    }

    public virtual bool ShouldDie() 
    {
        return currentHealth <= 0;
    }

    public virtual void TriggerDeadCrosshair() 
    {
        CrosshairManager.Instance.TriggerKill();
    }

    public virtual IEnumerator RemoveHealthBar() 
    {
        yield return null;
    }

    protected virtual void SpawnDamageText(int damage) 
    {
        if (damageTextPrefab != null)
        {
            GameObject textObj = ObjectPool.instance.GetObject(damageTextPrefab, transform);
            textObj.transform.localPosition = this.transform.position + new Vector3(0, 1.5f, 0);
            textObj.GetComponent<DamageText>().Setup();
            textObj.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
            textObj.AddComponent<Billboard>();
        }
    }
}
