using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBuildPC : Interactable
{
   public override void Interaction()
    {
        base.Interaction();
        UIManager.Instance.ShowPanel(PanelName.BuildingPanel, true);
    }
}
