using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shopper : MonoBehaviour
{
    private Shop activeShop = null;

    public event Action activeShopChange;

    public void SetActiveShop(Shop shop) 
    {
        if(activeShop != null) 
        {
            activeShop.SetShopper(null);
        }

        activeShop = shop;

        if(activeShop != null) 
        {
            activeShop.SetShopper(this);
        }

        if (activeShopChange != null)
        {
            activeShopChange();
        } 
    }

    public Shop GetActiveShop()
    {
        return activeShop;
    }
}
