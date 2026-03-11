using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Inventory/Equipable Stat Item"))]
public class StatEquipableItem : EquipableItem, IModifierProvider
{

    [SerializeField] List<BonusData> bonusDatas = new List<BonusData>();

    public IEnumerable<float> GetAdditiveModifiers(Stat stat)
    {
        foreach (var data in bonusDatas) 
        {
            if (data.stat == stat && data.modifierType == ModifierType.Add) 
            {
                yield return data.data;
            }
        }
    }

    public IEnumerable<float> GetPercentageModifiers(Stat stat)
    {
        foreach (var data in bonusDatas)
        {
            if (data.stat == stat && data.modifierType == ModifierType.AddPercentage)
            {
                yield return data.data;
            }
        }
    }

    [System.Serializable]
    public class BonusData 
    {
        public ModifierType modifierType;
        public Stat stat;
        public float data;
    }
}
