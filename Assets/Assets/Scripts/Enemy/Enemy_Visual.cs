using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum Enemy_MeleeWeaponType 
{
    OneHand,
    Throw,
    Unarmed
}

public enum Enemy_RangeWeaponType
{
    Pistol,
    Revolver,
    Shotgun,
    AutoRifle,
    Rifle
}
public class Enemy_Visual : MonoBehaviour
{ 
    public GameObject currentWeaponModel { get; private set; }
    public GameObject grenadeModel;

    [Header("Corruption Visuals")]
    [SerializeField] private GameObject[] corruptionCrystals;
    [SerializeField] private int corruptionAmount;

    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinedMeshRenderer;

    [Header("Rig References")]
    [SerializeField] private Transform leftHandIK;
    [SerializeField] private Transform leftElbowIK;
    [SerializeField] private TwoBoneIKConstraint leftHandIKConstraint;
    [SerializeField] private MultiAimConstraint weaponAimConstraint;

    private float leftHandTargetWeight;
    private float weaponAimTargetWeight;
    private float rigChangeRate;

    private void Awake()
    {
        CollectCorruptionCrystals(); 
    }

    private void Update()
    {
        leftHandIKConstraint.weight = AdjustIKWeight(leftHandIKConstraint.weight, leftHandTargetWeight);
        weaponAimConstraint.weight = AdjustIKWeight(weaponAimConstraint.weight, weaponAimTargetWeight);
    }

    public void EnableGrenadeModel(bool active) 
    {
        grenadeModel?.gameObject.SetActive(active);
    }

    public void EnableWeaponModel(bool active)
    {
        currentWeaponModel.gameObject.SetActive(active);
    }

    public void EnableSecondaryWeaponModel(bool active) 
    {
        FindSecondaryWeaponModel()?.gameObject.SetActive(active);
    }

    public void SetupLook() 
    {
        SetUpRandomColor();
        SetupRandomWeapon();
        SetupRandomCorruption();
    }

    private void SetupRandomCorruption() 
    {
        List<int> availableIndexs = new List<int>();
        corruptionCrystals = CollectCorruptionCrystals();

        for (int i = 0; i < corruptionCrystals.Length; i++) 
        {
            availableIndexs.Add(i);
            corruptionCrystals[i].SetActive(false);
        }

        for(int i = 0; i < corruptionAmount; i++) 
        {
            if (availableIndexs.Count == 0)
            {
                break;
            }
            int randomIndex = Random.Range(0, availableIndexs.Count);
            int objectIndex = availableIndexs[randomIndex];

            corruptionCrystals[objectIndex].SetActive(true);
            availableIndexs.RemoveAt(randomIndex);
        }
    }

    private void SetupRandomWeapon()
    {
        bool thisEnemyIsMelee = GetComponent<Enemy_Melee>() != null;
        bool thisEnemyIsRange = GetComponent<Enemy_Range>() != null;

        if(thisEnemyIsRange) 
        {
            currentWeaponModel = FindRangeWeaponModel();
        }

        if (thisEnemyIsMelee)
        {
            currentWeaponModel = FindMeleeWeaponModel();
        }

        currentWeaponModel.gameObject.SetActive(true);

        OverrideAnimatorControllerIfCan();
    }

    private void SetUpRandomColor()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);
        Material newMat = new Material(skinedMeshRenderer.material);

        newMat.mainTexture = colorTextures[randomIndex];
        skinedMeshRenderer.material = newMat;
    }

    private GameObject FindRangeWeaponModel() 
    {
        Enemy_RangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_RangeWeaponModel>(true);
        Enemy_RangeWeaponType weaponType = GetComponent<Enemy_Range>().weaponType;

        foreach (var weaponModel in weaponModels) 
        {
            if(weaponModel.weaponType == weaponType) 
            {
                SwitchAnimationLayer(((int)weaponModel.weaponHoldType));
                SetupLeftHandIK(weaponModel.leftHandTarget, weaponModel.leftElbowTarget);
                return weaponModel.gameObject;
            }
        }

        Debug.LogWarning("No Range Weapon Model Found");
        return null;
    }

    private GameObject FindMeleeWeaponModel()
    {
        Enemy_WeaponModel[] weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);

        Enemy_MeleeWeaponType weaponType = GetComponent<Enemy_Melee>().weaponType;

        List<Enemy_WeaponModel> filteredWeaponModels = new List<Enemy_WeaponModel>();



        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {
                filteredWeaponModels.Add(weaponModel);
            }
        }

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);

        return filteredWeaponModels[randomIndex].gameObject;
    }

    private GameObject[] CollectCorruptionCrystals()
    {
        Enemy_CorruptionCrystal[] crystalComponents = GetComponentsInChildren<Enemy_CorruptionCrystal>(true);

        GameObject[] corruptionCrystals = new GameObject[crystalComponents.Length];

        for (int i = 0; i < crystalComponents.Length; i++)
        {
            corruptionCrystals[i] = crystalComponents[i].gameObject;
        }

        return corruptionCrystals;
    }

    private GameObject FindSecondaryWeaponModel() 
    {
        Enemy_SecondaryRangeWeaponModel[] weaponModels = GetComponentsInChildren<Enemy_SecondaryRangeWeaponModel>(true);
        Enemy_RangeWeaponType weaponType = GetComponent<Enemy_Range>().weaponType;

        foreach (var weaponModel in weaponModels) 
        {
            if (weaponModel.weaponType == weaponType) 
            {

                return weaponModel.gameObject;
            }
        }
        return null;
    }

    private void OverrideAnimatorControllerIfCan()
    {
        AnimatorOverrideController overrideController = currentWeaponModel.GetComponent<Enemy_WeaponModel>()?.overrideController;

        if (overrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }

    private void SwitchAnimationLayer(int layerIndex) 
    {
        Animator anim = GetComponentInChildren<Animator>();
        for(int i = 1; i < anim.layerCount; i++) 
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }

    public void EnableIK(bool enableLeftHand, bool enableAim, float changeRate = 10) 
    {
        rigChangeRate = changeRate;
        leftHandTargetWeight = enableLeftHand ? 1 : 0;
        weaponAimTargetWeight = enableAim ? 1 : 0;
    }

    private void SetupLeftHandIK(Transform leftHandTarget, Transform leftElbowTarget) 
    {
        leftHandIK.localPosition = leftHandTarget.localPosition;
        leftHandIK.localRotation = leftHandTarget.localRotation;

        leftElbowIK.localPosition = leftElbowTarget.localPosition;
        leftElbowIK.localRotation = leftElbowTarget.localRotation;
    }

    private float AdjustIKWeight(float currentWeight, float targetWeight) 
    {
        if (Mathf.Abs(currentWeight - targetWeight) > 0.05f)
        {
            return Mathf.Lerp(currentWeight, targetWeight, rigChangeRate * Time.deltaTime);

        }
        else 
        {
            return targetWeight;
        }
    }
}
