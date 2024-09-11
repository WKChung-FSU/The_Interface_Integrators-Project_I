using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerWeapon : MonoBehaviour, IDamage
{
    [Header("-----RequiredFields-----")]
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] LineRenderer lightningVisual;
    [SerializeField] Transform SpellLaunchPos;
    [Range(0.01f, 2)][SerializeField] float MenuDelay;
    #region Spells
    [Header("-----Player Spell Lists-----")]
    //all should be summonable
    [SerializeField] SpellList basicSpells;
    [SerializeField] SpellList upgradedSpells;
    [SerializeField] SpellList currentSpellList;
    
    [Range(1, 100)][SerializeField] int MaxAmmo;

    [SerializeField] List<DamageEngine.ElementType> UpgradedElements;
    bool OutOfAmmo;
    bool isShooting;
    bool Cheat=false;
    bool SwitchingWeapon;
    int CurrAmmo;
    int currentWeapon;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        CurrAmmo = MaxAmmo;
        //gameManager.instance.ammoMax = MaxAmmo;

        //debug for now, btw enums are pain
        //AddSpell(DamageEngine.ElementType.Lightning);
        for (int SpellType = 0; SpellType <= (int)DamageEngine.ElementType.Water; SpellType++)
        {
            AddSpell((DamageEngine.ElementType)SpellType);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Shoot") && isShooting == false && gameManager.instance.menuActive == false)
            StartCoroutine(ShootPrimary());

        if (Input.GetButton("Shoot 2") && isShooting == false && gameManager.instance.menuActive == false)
            StartCoroutine(ShootSecondary());

        if ((Input.GetButtonDown("Switch Weapon") || Input.GetAxis("Mouse ScrollWheel") != 0)&& SwitchingWeapon==false)
            //No longer coroutine -CM, changed it back to implement menu lock-wc
            StartCoroutine( WeaponMenuSystem());
        if (Input.GetButton("Cheat"))
        {
            Cheat=!Cheat;
        }
        if (Input.GetButtonDown("Reload")&&Cheat)
        {
            ReloadAmmo();
        }
    }

    #region Public Getters

    public int Ammo
    {
        get
        {
            return CurrAmmo;
        }
        set
        {
            CurrAmmo = value;
        }
    }
    public int maxAmmo
    {
        get
        {
            return MaxAmmo;
        }
        set
        {
            MaxAmmo = value;
        }
    }
    public bool MenuLock
    {
        get { return SwitchingWeapon; }
        set { SwitchingWeapon = value; }
    }
    public int GetCurrentAmmo()
    { return CurrAmmo; }
    public int GetMaxAmmo()
    { return MaxAmmo; }
    public bool GetOutOfAmmo()
    {
        return OutOfAmmo;
    }
    public LineRenderer GetLineRenderer()
    {
        return lightningVisual;
    }
    public GameObject GetCurrentWeapon(bool Secondary=false)
    {
        if(!Secondary)
            return currentSpellList.PrimarySpells[currentWeapon];
        else 
            return currentSpellList.SecondarySpells[currentWeapon];
    }

    public List<DamageEngine.ElementType> UpgradedList()
    {
     return UpgradedElements;
    }

    public void UpgradedList(DamageEngine.ElementType Upgrade)
    {
        if (!UpgradedElements.Contains(Upgrade))
        {
            UpgradedElements.Add(Upgrade);
        }
    }

    private
    #endregion
        IEnumerator ShootPrimary()
    {
        isShooting = true;
        // All primary spells are summons
        if (((CurrAmmo - currentSpellList.PrimarySpellCost[currentWeapon]) >= 0) && currentSpellList.PrimarySpells[currentWeapon] != null)
        {
            Instantiate(currentSpellList.PrimarySpells[currentWeapon], SpellLaunchPos.position, SpellLaunchPos.rotation);
            CurrAmmo -= currentSpellList.PrimarySpellCost[currentWeapon];
        }
        else if (currentSpellList.PrimarySpells[currentWeapon] == null)
        {
            Debug.Log("Something Failed in ShootPrimary");
        }
        AmmoTest();
        yield return new WaitForSeconds(currentSpellList.PrimaryFireRate[currentWeapon]);
        isShooting = false;
    }


    IEnumerator ShootSecondary()
    {
        isShooting = true;

                if (((CurrAmmo - currentSpellList.SecondarySpellCost[currentWeapon]) >= 0) && currentSpellList.SecondarySpells[currentWeapon] != null)
                {
                    Instantiate(currentSpellList.SecondarySpells[currentWeapon], SpellLaunchPos.position, SpellLaunchPos.rotation);
                    CurrAmmo -= currentSpellList.SecondarySpellCost[currentWeapon];
                }
                else if (currentSpellList.SecondarySpells[currentWeapon] == null)
                {
                    Debug.Log("Something Failed in ShootSecondary");
                }
        AmmoTest();
        yield return new WaitForSeconds(currentSpellList.SecondaryFireRate[currentWeapon]);
        isShooting = false;

    }
    IEnumerator WeaponMenuSystem()
    {
        SwitchingWeapon=true;
        //changed from IEnumerator to void -CM, changed it back to implement menu lock for fracturing-WC
        if ((Input.GetAxis("Switch Weapon") > 0 || Input.GetAxis("Mouse ScrollWheel") > 0) && currentWeapon < currentSpellList.PrimarySpells.Count - 1)
        {
            currentWeapon++;
        }
        else if ((Input.GetAxis("Switch Weapon") < 0 || Input.GetAxis("Mouse ScrollWheel") < 0) && currentWeapon > 0)
        {
            currentWeapon--;
        }
        gameManager.instance.UpdateWeaponIconUI();
        UpdateSpellList();
        yield return new WaitForSeconds(MenuDelay);
        SwitchingWeapon=false;
    }
    void AmmoTest()
    {
        if (CurrAmmo > 0)
        {
            OutOfAmmo = false;
        }
        else
        {
            OutOfAmmo = true;
            CurrAmmo = 0;
        }
    }
    public void ReloadAmmo(int amount = 0)
    {
        if (amount == 0)
        {
            CurrAmmo = MaxAmmo;
        }
        else
        {
            CurrAmmo += amount;
        }
    }
    public void AddSpell(DamageEngine.ElementType spellType)
    {
       bool spellTypeFound=false;

        foreach (var spell in currentSpellList.PrimarySpells)
        {
            if (spell.GetComponent<AttackCore>().ElementType == spellType)
            {
                spellTypeFound = true;
            }
        }

        if (!spellTypeFound)
        {
            for (int i = 0; i < basicSpells.PrimarySpells.Count; i++)
            {
               if(basicSpells.PrimarySpells[i].GetComponent<AttackCore>().ElementType == spellType)
                {
                    AddSpell(currentSpellList.PrimarySpells, currentSpellList.PrimarySpellCost, currentSpellList.PrimaryFireRate, 
                        basicSpells.PrimarySpells[i],basicSpells.PrimarySpellCost[i], basicSpells.PrimaryFireRate[i]);

                    AddSpell(currentSpellList.SecondarySpells, currentSpellList.SecondarySpellCost, currentSpellList.SecondaryFireRate,
                      basicSpells.SecondarySpells[i], basicSpells.SecondarySpellCost[i], basicSpells.SecondaryFireRate[i]);
                }
            }
        }
    }

    void AddSpell(List<GameObject> MasterList,List<int>MasterSpellCost,List<float> MasterFirerate, GameObject Spell, int SpellCost, float FireRate)
    {
        MasterList.Add(Spell);
        MasterSpellCost.Add(SpellCost);
        MasterFirerate.Add(FireRate);
    }

    public void UpdateSpellList()
    {
        UpdateSpellList(currentSpellList.PrimarySpells, true);
        UpdateSpellList(currentSpellList.SecondarySpells,false);
    }

    void UpdateSpellList(List<GameObject> list, bool isPrimary)
    {
        List < GameObject > newSpells = new List < GameObject >();
  
        for (int i = 0; i < list.Count; i++)
        {
            DamageEngine.ElementType SpellElement = list[i].GetComponent<AttackCore>().ElementType;
            if (UpgradedElements.Contains(SpellElement))
            { 
                if (isPrimary)
                    newSpells = upgradedSpells.PrimarySpells;
                else
                    newSpells = upgradedSpells.SecondarySpells;

                foreach (var spell in newSpells)
                {
                    if (spell.GetComponent<AttackCore>().ElementType == SpellElement) 
                    {
                        list[i]=spell;
                    };
                }
            }
        }
    }
}
