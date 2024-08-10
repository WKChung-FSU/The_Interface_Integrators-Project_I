using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerControler : MonoBehaviour, IDamage
{
    //fields
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [SerializeField] int Hp;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    Vector3 move;
    Vector3 playerVel;
    int jumpCount;
    int hpOriginal;
     bool isSprinting;
    bool isShooting;
    // Start is called before the first frame update
    void Start()
    {
        hpOriginal = Hp;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!gameManager.instance.isPaused)
        //{
            Movement();
        //}
        Sprint();
    }
    void Movement()
    {
        
        
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel.y = 0;
        }

        move = Input.GetAxis("Vertical") * transform.forward +
            Input.GetAxis("Horizontal") * transform.right;

        controller.Move(move * speed * Time.deltaTime);
        // jump/grav
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if(Input.GetButton("Shoot")&& isShooting==false)
            StartCoroutine(Shoot());
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist,~ignoreMask)) 
        {
            //Debug.Log(hit.collider.name);

            IDamage damage = hit.collider.GetComponent<IDamage>();

            if (damage != null) {
                damage.takeDamage(shootDamage);
            
            }

        };
        yield return new WaitForSeconds(shootRate);
        isShooting=false;

    }
    public void takeDamage(int amount, DamageEngine.damageType DamageType)
    {
        Hp-=amount;

        if (Hp <= 0) {
            gameManager.instance.youLose();
        }
    }
}
