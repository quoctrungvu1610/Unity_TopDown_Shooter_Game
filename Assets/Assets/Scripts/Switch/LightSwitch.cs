using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Switch
{
    [SerializeField] private List<BaseLight> lightsToSwitch = new List<BaseLight>();

    public override void SwitchObject()
    {
        base.SwitchObject();
        foreach (ISwitchable light in lightsToSwitch)
        {
            light.Switch();
        }
    }

    public override void Interaction()
    {
        base.Interaction();
        SwitchObject();
    }
}
