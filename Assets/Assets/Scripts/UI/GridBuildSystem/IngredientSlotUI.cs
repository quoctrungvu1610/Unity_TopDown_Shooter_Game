using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlotUI : MonoBehaviour
{
    [SerializeField] private Image ingredientIcon;
    [SerializeField] private TextMeshProUGUI ingredientText;

    public void Setup(Sprite ingredientIcon, string ingredientText) 
    {
        this.ingredientIcon.sprite =  ingredientIcon;
        this.ingredientText.text = ingredientText; 
    }
}
