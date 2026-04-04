using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridBuildSystemSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buildObjectName;
    [SerializeField] private TextMeshProUGUI buildObjectDescription;
    [SerializeField] private TextMeshProUGUI buildObjectPrice;
    [SerializeField] private TextMeshProUGUI placedObjectCount;
    [SerializeField] private Image buildObjectIcon;
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Image lockImage;
    [SerializeField] private GameObject clickToUnlockText;

    [SerializeField] private Transform ingredientSlotsParent;
    [SerializeField] private IngredientSlotUI ingredientSlotUIPrefab;

    [SerializeField] private Color unlockedColor;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button placeButton;

    private BuildObjectData buildObjectData;
    private BuildObjectStore buildObjectStore;

    private void Awake()
    {
        unlockButton.onClick.AddListener(UnlockObject);
        placeButton.onClick.AddListener(PlaceObject);
    }

    public void Setup(BuildObjectData data, bool isUnlocked, bool isUnlockable, BuildObjectStore store, bool canPlace) 
    {
        this.buildObjectData = data;
        this.buildObjectStore = store;

        buildObjectName.text = data.GetObjectName();
        buildObjectDescription.text = data.GetObjectDescription();
        buildObjectPrice.text = "$" + data.GetObjectPrice().ToString();
        placedObjectCount.text = $"Max Placed: {buildObjectStore.GetPlacedObjectsCount(data)}/{data.GetMaxObjectToPlace()}";
        buildObjectIcon.sprite = data.GetObjectIcon();
        backGroundImage.color = isUnlocked ? unlockedColor : Color.white;
        unlockButton.gameObject.SetActive(!isUnlocked);
        unlockButton.interactable = isUnlockable ? true : false;
        placeButton.interactable = isUnlocked && canPlace ? true : false;
        lockImage.gameObject.SetActive(!isUnlockable);
        clickToUnlockText.gameObject.SetActive(isUnlockable);

        foreach (Transform child in ingredientSlotsParent) 
        {
            Destroy(child.gameObject);
        }

        foreach (var ingredient in data.GetObjectIngredients())
        {
            IngredientSlotUI ingredientSlot = Instantiate(ingredientSlotUIPrefab, ingredientSlotsParent);
            ingredientSlot.Setup(ingredient.item.GetIcon(), $"({buildObjectStore.GetPlayerInventory().GetItemNumber(ingredient.item)}/{ingredient.quantity.ToString()})");
        }
    }

    private void UnlockObject() 
    {
        buildObjectStore.UnlockObject(buildObjectData);
    }

    private void PlaceObject() 
    {
        if (buildObjectStore.CheckIfCanPlaceObject(buildObjectData) == false) return;
        if (buildObjectData == null) return;
        buildObjectStore.GetPlacementSystem().StartPlacement(buildObjectData);
        //TO DO
        buildObjectStore.AddToPlacedObject(buildObjectData);
    
    }
}

