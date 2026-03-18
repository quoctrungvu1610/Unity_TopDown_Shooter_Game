using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PurseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI balanceField;

    private Purse playerPurse;

    private void Start()
    {
        playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().purse;
        if (playerPurse != null) 
        {
            playerPurse.onchange += RefreshUI;
        }
        RefreshUI();

    }

    private void RefreshUI() 
    {
        balanceField.text = "$" + playerPurse.GetBalance().ToString("N2");
    }
}
