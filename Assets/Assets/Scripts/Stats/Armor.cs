using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour
{
    [SerializeField] private float currentArmor;
    private void Awake()
    {
        GetComponent<Equipment>().equipmentUpdated += Redraw;
    }

    private void Start()
    {
        Redraw();
    }

    public void Redraw()
    {
        currentArmor = GetComponent<BaseStat>().GetStat(Stat.Armor);

    }
}
