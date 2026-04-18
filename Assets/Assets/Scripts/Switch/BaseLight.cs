using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLight : MonoBehaviour, ISwitchable
{
    private bool isOn = true;

    public void Switch()
    {
        isOn = !isOn;
        this.gameObject.SetActive(isOn);
    }
}
