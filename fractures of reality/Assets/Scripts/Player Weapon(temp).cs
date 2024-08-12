using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour, IDamage
{
    #region Test Variables
    [SerializeField] int Hp;
    [SerializeField] LayerMask ignoreMask;

    int hpOriginal;
    bool isSprinting;
    bool isShooting;

    #endregion
    #region wepon Stats
    [SerializeField] DamageEngine.damageType Weapon1Type;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    int MaxAmmo;
    int CurrAmmo;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        hpOriginal = Hp;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Shoot") && isShooting == false)
            StartCoroutine(Shoot());
    }

    #region Ammo Getters
    int getMaxAmmo()
    {
        return MaxAmmo;
    }

    int getCurrentAmmo()
    {
        return CurrAmmo;
    }

    #endregion

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            //Debug.Log(hit.collider.name);

            IDamage damage = hit.collider.GetComponent<IDamage>();

            if (damage != null)
            {
                damage.takeDamage(shootDamage,Weapon1Type);
            }

        };
        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }
    public void takeDamage(int amount, DamageEngine.damageType DamageType)
    {
        Hp -= amount;

        if (Hp <= 0)
        {
            gameManager.instance.youLose();
        }
    }
}
