using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeed : MonoBehaviour
{
    [SerializeField] private float currentMoveSpeed;
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
        currentMoveSpeed = GetComponent<BaseStat>().GetStat(Stat.MoveSpeed);

    }
}
