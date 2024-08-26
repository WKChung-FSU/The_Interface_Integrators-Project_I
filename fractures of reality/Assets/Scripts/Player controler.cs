using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerControler : MonoBehaviour
{

    //fields
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;


    Vector3 move;
    Vector3 playerVel;
    int jumpCount;
    bool isSprinting;
    bool isShooting;
    public List<KeySystem> Keys = new List<KeySystem>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public List<KeySystem> PlayerKeys
    {
        get
        {
            return Keys;
        }

        set 
        { 
        Keys= value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            Movement();
        }
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
    //public void damageEffect(int amount, DamageEngine.ElementType ElementType)
    //{
    //    //gameManager.instance.DamageFlashScreen();
    //}
}
