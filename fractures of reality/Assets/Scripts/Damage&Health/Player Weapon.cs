using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using UnityEditor;
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
    bool Cheat = false;
    bool SwitchingWeapon;
    int CurrAmmo;
    int currentWeapon;
    [SerializeField] EntTeleportation TPSpell;

    float PrimeFireRate;
    float SecondFireRate;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        CurrAmmo = MaxAmmo;
        //gameManager.instance.ammoMax = MaxAmmo;

        //debug for now, btw enums are pain
        //AddSpell(DamageEngine.ElementType.Lightning);
        for (int SpellType = 0; SpellType <(int)DamageEngine.ElementType.Water; SpellType++)
        {
            AddSpell((DamageEngine.ElementType)SpellType);
        }
        UpgradedElements = gameManager.instance.PCrystalManifest.DestroyList;
        UpdateSpellList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Shoot") && isShooting == false && gameManager.instance.menuActive == false)
            StartCoroutine(ShootPrimary());

        if ((Input.GetButton("Shoot 2") && isShooting == false && gameManager.instance.menuActive == false) &&!TPSpell)
            StartCoroutine(ShootSecondary());

        if (TPSpell)
        {
            if (Input.GetButtonDown("Shoot 2") && gameManager.instance.menuActive == false)
            {
                TPSpell.Teleport(true);
            }
            if (Input.GetButtonUp("Shoot 2") && gameManager.instance.menuActive == false)
            {
               TPSpell.Teleport(false);
            }
        }

        
        if (Input.GetButton("Cheat"))
        {
            Cheat = !Cheat;
        }
        if (Input.GetButtonDown("Reload") && Cheat)
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
    public float PrimaryFireRate
    {
        get { return PrimeFireRate; }
        set { PrimeFireRate = value; }
    }
    public float SecondaryFireRate
    {
        get { return SecondFireRate; }
        set { SecondFireRate = value; }
    }


    public bool PlayerShooting
    {
        get { return isShooting; }
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
    //May depreciate
  
    public LineRenderer GetLineRenderer()
    {
        return lightningVisual;
    }
    public GameObject GetCurrentWeapon(bool Secondary = false)
    {
        if (!Secondary)
            return currentSpellList.PrimarySpells[currentWeapon];
        else
            return currentSpellList.SecondarySpells[currentWeapon];
    }

    public DamageEngine.ElementType GetCurrentElement()
    {

        return GetCurrentWeapon().GetComponent<AttackCore>().ElementType;
    }

    public List<DamageEngine.ElementType> UpgradedList()
    {
        return UpgradedElements;
    }

    /// <summary>
    /// Adds The ability for the player to use a upgraded spell type
    /// </summary>
    /// <param name="Element"></param>
    /// <param name="Upgrade">if true upgrade, if false downgrade</param>
    public void UpgradedList(DamageEngine.ElementType Element, bool Upgrade = true)
    {
        if (Upgrade && !UpgradedElements.Contains(Element))
            UpgradedElements.Add(Element);
        else if (!Upgrade && UpgradedElements.Contains(Element))
            UpgradedElements.Remove(Element);

        UpdateSpellList();
    }

    #endregion
    IEnumerator ShootPrimary()
    {
        AttackCore castedSpell=null;
        isShooting = true;
        // All primary spells are summons
        if (((CurrAmmo - currentSpellList.PrimarySpellCost[currentWeapon]) >= 0) && currentSpellList.PrimarySpells[currentWeapon] != null)
        {
            castedSpell=Instantiate(currentSpellList.PrimarySpells[currentWeapon], SpellLaunchPos.position, SpellLaunchPos.rotation).GetComponent<AttackCore>();
            CurrAmmo -= currentSpellList.PrimarySpellCost[currentWeapon];
            gameManager.instance.PlayerController.UpdateFractureBar(castedSpell.GetFractureDamage());
        }
        else if (currentSpellList.PrimarySpells[currentWeapon] == null)
        {
            Debug.Log("Something Failed in ShootPrimary");
        }
        AmmoTest();
        yield return new WaitForSeconds(PrimeFireRate);
        isShooting = false;
    }


    IEnumerator ShootSecondary()
    {
        GameObject castedSpell;
        AttackCore attack;
        isShooting = true;
       
        if (((CurrAmmo - currentSpellList.SecondarySpellCost[currentWeapon]) >= 0) && currentSpellList.SecondarySpells[currentWeapon] != null)
        {

            castedSpell = Instantiate(currentSpellList.SecondarySpells[currentWeapon], SpellLaunchPos.position, SpellLaunchPos.rotation);
            TPSpell=castedSpell.GetComponent<EntTeleportation>();
            attack= castedSpell.GetComponent<AttackCore>();

            if (TPSpell)
                TPSpell.SetHealthCore(gameManager.instance.playerScript);
            CurrAmmo -= currentSpellList.SecondarySpellCost[currentWeapon];
            if(attack)
             gameManager.instance.PlayerController.UpdateFractureBar(attack.GetFractureDamage());
        }
        else if (currentSpellList.SecondarySpells[currentWeapon] == null)
        {
            Debug.Log("Something Failed in ShootSecondary");
        }
        AmmoTest();
        yield return new WaitForSeconds(SecondFireRate);
        isShooting = false;

    }
    IEnumerator WeaponMenuSystem()
    {
        SwitchingWeapon = true;
        //changed from IEnumerator to void -CM, changed it back to implement menu lock for fracturing-WC
       
        gameManager.instance.UpdateWeaponIconUI();
        UpdateSpellList();
        gameManager.instance.playerScript.ElementType = GetCurrentElement();
        yield return new WaitForSeconds(MenuDelay);
        SwitchingWeapon = false;
    }
    public void InstantWeaponSwitch(DamageEngine.ElementType spellType)
    {
        foreach (var spell in currentSpellList.PrimarySpells)
        {
            if (spell.GetComponent<AttackCore>().ElementType == spellType)
            {
                currentWeapon = currentSpellList.PrimarySpells.IndexOf(spell);
                break;
            }
        }
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
        bool spellTypeFound = false;

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
                if (basicSpells.PrimarySpells[i].GetComponent<AttackCore>().ElementType == spellType)
                {
                    AddSpell(currentSpellList.PrimarySpells, currentSpellList.PrimarySpellCost, currentSpellList.PrimaryFireRate,
                        basicSpells.PrimarySpells[i], basicSpells.PrimarySpellCost[i], basicSpells.PrimaryFireRate[i]);

                    AddSpell(currentSpellList.SecondarySpells, currentSpellList.SecondarySpellCost, currentSpellList.SecondaryFireRate,
                      basicSpells.SecondarySpells[i], basicSpells.SecondarySpellCost[i], basicSpells.SecondaryFireRate[i]);
                }
            }
        }
    }
    void AddSpell(List<GameObject> MasterList, List<int> MasterSpellCost, List<float> MasterFirerate, GameObject Spell, int SpellCost, float FireRate)
    {
        MasterList.Add(Spell);
        MasterSpellCost.Add(SpellCost);
        MasterFirerate.Add(FireRate);
    }

   public void ClearTP()
    {
        TPSpell=null;
    }
    public void UpdateSpellList()
    {
        UpdateSpellList(currentSpellList.PrimarySpells, currentSpellList.PrimarySpellCost,currentSpellList.PrimaryFireRate,true);
        UpdateSpellList(currentSpellList.SecondarySpells, currentSpellList.SecondarySpellCost,currentSpellList.SecondaryFireRate, false);
        PrimeFireRate = currentSpellList.PrimaryFireRate[currentWeapon];
        SecondFireRate = currentSpellList.SecondaryFireRate[currentWeapon];
    }

    void UpdateSpellList(List<GameObject> list,List<int>SpellCost,List<float>spellFireRate, bool isPrimary)
    {
        List<GameObject> newSpells = new List<GameObject>();
        List<int>NewSpellCost = new List<int>();
        List<float>NewFireRate = new List<float>();
        for (int i = 0; i < list.Count; i++)
        {
            DamageEngine.ElementType SpellElement = list[i].GetComponent<AttackCore>().ElementType;
            if (UpgradedElements.Contains(SpellElement))
            {
                if (isPrimary)
                {
                    newSpells = upgradedSpells.PrimarySpells;
                    NewSpellCost = upgradedSpells.PrimarySpellCost;
                    NewFireRate = upgradedSpells.PrimaryFireRate;
                }
                else
                {
                    newSpells = upgradedSpells.SecondarySpells;
                    NewSpellCost = upgradedSpells.SecondarySpellCost;
                    NewFireRate = upgradedSpells.SecondaryFireRate;

                }
            }
            else if ((currentSpellList.PrimarySpells[i] != basicSpells.PrimarySpells[i]) ||
                (currentSpellList.SecondarySpells[i] != basicSpells.SecondarySpells[i]))
            {
                if (isPrimary)
                {
                    newSpells = basicSpells.PrimarySpells;
                    NewSpellCost = basicSpells.PrimarySpellCost;
                    NewFireRate = basicSpells.PrimaryFireRate;
                }
                else
                {
                    newSpells = basicSpells.SecondarySpells;
                    NewSpellCost = basicSpells.SecondarySpellCost;
                    NewFireRate = basicSpells.SecondaryFireRate;
                }
            }
            for (int Spell = 0; Spell < newSpells.Count; Spell++)
            {
                
            if (newSpells[Spell].GetComponent<AttackCore>().ElementType == SpellElement)
                {
                    list[i] = newSpells[Spell];
                    SpellCost[i]=NewSpellCost[Spell];
                    spellFireRate[i]=NewFireRate[Spell];
                };
            }
        }
    }

}
