using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour, IDamage
{
    #region Test Variables
    [SerializeField] LayerMask ignoreMask;
    #endregion
    #region wepon Stats
    [SerializeField] DamageEngine.damageType Weapon1Type;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;
    [SerializeField] int MaxAmmo;
    DamageEngine[] weaponInventory= new DamageEngine[3];

    public int CurrAmmo;
    bool isShooting;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        CurrAmmo = MaxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Shoot") && isShooting == false)
            StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
        {
            Debug.Log(hit.collider.name);

            IDamage damage = hit.collider.GetComponent<IDamage>();

            if (damage != null)
            {
                damage.takeDamage(shootDamage,Weapon1Type);
                CurrAmmo--;
            }

        };
        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }
}
