using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStat : MonoBehaviour
{
    [SerializeField] private int currentLevel;
    [SerializeField] private Progression progression;

    public float GetPlayerBaseStat(Stat stat) 
    {
        return progression.GetBaseStatDataByLevel(stat, currentLevel);
    }

    public float GetStat(Stat stat) 
    {
        return (GetPlayerBaseStat(stat) + GetAdditiveModifierNumber(stat)) * (1 + GetPercentageModifierNumber(stat) / 100);
    }

    public IEnumerable<float> GetAdditiveModifier(Stat stat) 
    {
        Equipment equipment = this.GetComponent<Equipment>();
        foreach (var slot in equipment.GetAllPopulatedSlots()) 
        {
            var item = equipment.GetItemInSlot(slot) as IModifierProvider;
            if (item == null) continue;
            foreach (float modifier in item.GetAdditiveModifiers(stat)) 
            {
                yield return modifier;
            }
        }
    }

    public float GetAdditiveModifierNumber(Stat stat)
    {
        float total = 0;
        foreach (float modifier in GetAdditiveModifier(stat)) 
        {
            total += modifier;
        }
        return total;
    }

    public IEnumerable<float> GetPercentageModifier(Stat stat)
    {
        Equipment equipment = this.GetComponent<Equipment>();
        foreach (var slot in equipment.GetAllPopulatedSlots())
        {
            var item = equipment.GetItemInSlot(slot) as IModifierProvider;
            if (item == null) continue;
            foreach (float modifier in item.GetPercentageModifiers(stat))
            {
                yield return modifier;
            }
        }
    }

    public float GetPercentageModifierNumber(Stat stat)
    {
        float total = 0;
        foreach (float modifier in GetPercentageModifier(stat))
        {
            total += modifier;
        }
        return total;
    }
}
