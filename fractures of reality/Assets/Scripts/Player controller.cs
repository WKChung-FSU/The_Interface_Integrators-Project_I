using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerController : MonoBehaviour
{

    //fields
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] float speed = 5;
    float originalSpeed;
    float baseSpeed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    
    [Header("-----Player Sounds-----")]
    [SerializeField] AudioClip[] AudioJump;
    [Range(0, 1)][SerializeField] float AudioJumpVol = 0.5f;

    [SerializeField] AudioClip[] AudioSteps;
    [Range(0, 1)][SerializeField] float AudioStepsVol = 0.5f;
    [Header("-----Fracturing-----")]
    [Range(0, 10)][SerializeField] float fractureRegenRate =0.5f;
    [Range(1, 50)][SerializeField] int MaxFractureResistance=5;
    [Range(1, 10)][SerializeField] int fractureRegenAmount = 1;
    public Dictionary<DamageEngine.ElementType, int> FractureBars=new Dictionary<DamageEngine.ElementType, int>();
    int CurrentFractureBar;
    Vector3 move;
    Vector3 playerVel;
    int jumpCount;
    public bool isSprinting;
    bool isShooting;
    bool isPlayingSteps;
    bool IsFractured;
    bool IsRegenFracture;
    public List<KeySystem> Keys = new List<KeySystem>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (DamageEngine.ElementType type in Enum.GetValues(typeof(DamageEngine.ElementType)))
        {
            if (!FractureBars.TryAdd(type, 0))
            {
                Debug.LogError("Fracture bar has failed to build");
            }
        }
        FractureListUpdate(DamageEngine.ElementType.Normal);
        originalSpeed = speed;
        baseSpeed = speed;
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

        if (!IsRegenFracture)
        {
            StartCoroutine(RegenFracture());
        }

    }
    void Movement()
    {
        
        
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel.y = 0;
        }

        move = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;

        controller.Move(move * speed * Time.deltaTime);
        // jump/grav
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            gameManager.instance.playAudio(AudioJump[UnityEngine.Random.Range(0, AudioJump.Length)], AudioJumpVol);
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (controller.isGrounded && move.magnitude > 0.3f && !isPlayingSteps)
        {
            StartCoroutine(PlayStep());
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            baseSpeed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            baseSpeed /= sprintMod;
            isSprinting = false;
        }
        speed = baseSpeed;
    }

    public void FractureListUpdate(DamageEngine.ElementType Element)
    {
        CurrentFractureBar=FractureBars[Element];
        updateFractureBar();
    }

    public void FractureListUpdate()
    {
        FractureBars[gameManager.instance.playerWeapon.GetCurrentElement()]=CurrentFractureBar;
    }


    public void updateFractureBar(int amount=0)
    {
        CurrentFractureBar += amount;

        if (CurrentFractureBar <= 0&&IsFractured) 
        {
            CurrentFractureBar = 0;
            IsFractured = false;
            gameManager.instance.playerWeapon.MenuLock = false;
        }
        else if(CurrentFractureBar >= MaxFractureResistance)
        {
            CurrentFractureBar=MaxFractureResistance;
            gameManager.instance.playerWeapon.MenuLock = true;
            IsFractured = true;
        }
    }
    IEnumerator RegenFracture()
    {
        FractureListUpdate();
        IsRegenFracture = true;
        foreach (DamageEngine.ElementType Type in Enum.GetValues(typeof(DamageEngine.ElementType)))
        {
            if (FractureBars[Type] < 0 ) 
                FractureBars[Type]= 0;

            else if(FractureBars[Type] > 0)
                FractureBars[Type] -= fractureRegenAmount;
        }
        FractureListUpdate(gameManager.instance.playerWeapon.GetCurrentElement());
        yield return new WaitForSeconds(fractureRegenRate);
        IsRegenFracture=false;
    }
    IEnumerator PlayStep()
    {
        isPlayingSteps = true;
        gameManager.instance.playAudio(AudioSteps[UnityEngine.Random.Range(0, AudioSteps.Length)], AudioStepsVol);
        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);
        isPlayingSteps = false;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void SetBaseSpeed(float newSpeed)
    {
        baseSpeed = newSpeed;
    }
    public float GetBaseSpeed()
    {
        return baseSpeed;
    }
    public float GetOriginalSpeed()
    {
        return originalSpeed;
    }
    public int GetSprintMod()
    {
        return sprintMod;
    }
    public void SetSprintMod(int newSprintMod)
    {
        sprintMod = newSprintMod;
    }

}
