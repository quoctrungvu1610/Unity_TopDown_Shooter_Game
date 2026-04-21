using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingGroundSwitch : Switch
{
    [SerializeField] private TrainingGroundManager trainingGroundManager;

    public override void SwitchObject()
    {
        base.SwitchObject();
        trainingGroundManager.Switch();
    }

    public override void Interaction()
    {
        base.Interaction();
        SwitchObject();
    }
}
