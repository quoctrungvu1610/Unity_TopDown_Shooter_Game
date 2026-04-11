using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Inventory/Item Bullet"))]
public class BulletItem : EquipableItem
{
    [SerializeField] private BulletData bulletData;

    public BulletData GetBulletData()
    {
        return bulletData;
    }
}
