using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    public bool occupied = false;

    public void SetOccupied(bool occupied) 
    {
        this.occupied = occupied;
    }
}
