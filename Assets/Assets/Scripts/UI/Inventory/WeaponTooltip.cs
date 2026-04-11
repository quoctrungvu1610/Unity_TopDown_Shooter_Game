using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponTooltip : MonoBehaviour
{
    // CONFIG DATA
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] TextMeshProUGUI bodyText = null;
    [SerializeField] TextMeshProUGUI magCapacityText = null;
    [SerializeField] TextMeshProUGUI bulletPerShotText = null;
    [SerializeField] TextMeshProUGUI fireRateText = null;
    [SerializeField] TextMeshProUGUI burstAvailableText = null;
    [SerializeField] TextMeshProUGUI burstBulletPerShotText = null;
    [SerializeField] TextMeshProUGUI burstFireRateText = null;
    [SerializeField] TextMeshProUGUI burstFireDelayText = null;
    [SerializeField] TextMeshProUGUI baseSpreadText = null;
    [SerializeField] TextMeshProUGUI maxSpreadText = null;
    [SerializeField] TextMeshProUGUI spreadIncreaseRateText = null;
    [SerializeField] TextMeshProUGUI reloadSpeedText = null;
    [SerializeField] TextMeshProUGUI equipmentSpeedText = null;
    [SerializeField] TextMeshProUGUI gunDistanceText = null;
    [SerializeField] TextMeshProUGUI bulletDamageText = null;
    [SerializeField] TextMeshProUGUI compatibleBulletsText = null;

    // PUBLIC

    public void Setup(InventoryItem item)
    {
        WeaponEquipableItem weaponInventoryItem = item as WeaponEquipableItem;
        titleText.text = item.GetDisplayName();
        bodyText.text = item.GetDescription();

        bulletDamageText.text = weaponInventoryItem.GetWeaponData().damage.ToString();
        magCapacityText.text = weaponInventoryItem.GetWeaponData().magazineCapacity.ToString();
        bulletPerShotText.text = weaponInventoryItem.GetWeaponData().bulletPerShot.ToString();
        fireRateText.text = weaponInventoryItem.GetWeaponData().fireRate.ToString();
        burstAvailableText.text = weaponInventoryItem.GetWeaponData().burstAvailable ? "Yes" : "No";
        burstBulletPerShotText.text = weaponInventoryItem.GetWeaponData().burstBulletPerShot.ToString();
        burstFireRateText.text = weaponInventoryItem.GetWeaponData().burstFireRate.ToString();
        burstFireDelayText.text = weaponInventoryItem.GetWeaponData().burstFireDelay.ToString();
        maxSpreadText.text = weaponInventoryItem.GetWeaponData().maxSpread.ToString();
        spreadIncreaseRateText.text = weaponInventoryItem.GetWeaponData().spreadIncreaseRate.ToString();
        reloadSpeedText.text = weaponInventoryItem.GetWeaponData().reloadSpeed.ToString();
        equipmentSpeedText.text = weaponInventoryItem.GetWeaponData().equipmentSpeed.ToString();
        gunDistanceText.text = weaponInventoryItem.GetWeaponData().gunDistance.ToString();
        baseSpreadText.text = weaponInventoryItem.GetWeaponData().baseSpread.ToString();

        foreach (var data in weaponInventoryItem.GetWeaponData().GetCompatibleBullets()) 
        {
            compatibleBulletsText.text += "- " + data.GetBulletName() + "\n";
        }
    }
}
