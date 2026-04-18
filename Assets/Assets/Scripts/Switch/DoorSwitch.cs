using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : Switch
{
    [SerializeField] private List<Door> doorsToSwitch = new List<Door>();

    public override void SwitchObject()
    {
        base.SwitchObject();
        foreach (ISwitchable door in doorsToSwitch)
        {
            door.Switch();
        }
    }

    public override void Interaction()
    {
        base.Interaction();
        SwitchObject();
    }
}
