using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBar;

    private Player player;
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()   
    {
       UpdateHealthBar();
    }

    private void UpdateHealthBar() 
    {
        healthText.text = player.health.currentHealth.ToString();
        healthBar.fillAmount = player.health.currentHealth / player.health.startHealth;

    }
}
