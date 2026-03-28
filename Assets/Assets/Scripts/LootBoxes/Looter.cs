using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looter : MonoBehaviour
{
    [SerializeField] private LootBox currentActivelootBox;

    public void SetCurrentActiveLootBox(LootBox lootBox) 
    {
        if (currentActivelootBox != null) 
        {
            currentActivelootBox = null;
        }
        currentActivelootBox = lootBox;
    }

    public LootBox GetCurrentActiveLootBox() 
    {
        if(currentActivelootBox == null)
            return null;
        return currentActivelootBox;
    }
}
