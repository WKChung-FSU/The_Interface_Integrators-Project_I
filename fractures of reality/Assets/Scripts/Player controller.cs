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
    [SerializeField] KeyPocket PlayerPocket;
    [Header("-----Player Sounds-----")]
    [SerializeField] AudioClip[] AudioJump;
    [Range(0, 1)][SerializeField] float AudioJumpVol = 0.5f;

    [SerializeField] AudioClip[] AudioSteps;
    [Range(0, 1)][SerializeField] float AudioStepsVol = 0.5f;
    [Header("-----Fracturing-----")]
    [Range(0, 10)][SerializeField] float fractureRegenRate =0.5f;
    [Range(1, 50)][SerializeField] int MaxFractureResistance=5;
    [Range(1, 10)][SerializeField] int fractureRegenAmount = 1;
    [Range(1, 10)][SerializeField] int FractureFireRateReduction;
   Dictionary<DamageEngine.ElementType, int> FractureBars=new Dictionary<DamageEngine.ElementType, int>();
   
    Vector3 move;
    Vector3 playerVel;
    int jumpCount;
    public bool isSprinting;
    bool isShooting;
    bool isPlayingSteps;
    bool IsFractured;
    bool IsRegenFracture;
    bool UpgradedSpell=false;

    // Start is called before the first frame update
    void Start()
    {
        InitializeFractureBars();
        originalSpeed = speed;
        baseSpeed = speed;
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

    #region Getters/Setters

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
    public List<KeySystem> PlayerKeys
    {
        get
        {
            return PlayerPocket.AccessKeys;
        }

        set 
        {
            PlayerPocket.AccessKeys = value;
        }
    }

    public Dictionary<DamageEngine.ElementType, int> fractureBars
    {
        get { return FractureBars; }
    }

    public int getMaxFractureAmount()
    {
        return MaxFractureResistance;
    }

    public bool FractureStatus
    {
        get
        {
            return IsFractured;
        }
    }

    #endregion
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


    #region Fracture Logic
    /// <summary>
    /// Clears and rebuilds the Fracture bars.
    /// </summary>
    public void InitializeFractureBars()
    {
        FractureBars.Clear();
        foreach (DamageEngine.ElementType type in Enum.GetValues(typeof(DamageEngine.ElementType)))
        {
            if (!FractureBars.TryAdd(type, 0))
            {
                Debug.LogError("Fracture bar has failed to build");
            }
        }
       
    }

    /// <summary>
    /// Adds number to the fracture bar of the current element, the correct bar is chosen automatically.
    /// </summary>
    /// <param name="amount">the amount that you wish to increase the fracture bar by</param>
    public void UpdateFractureBar(int amount=0)
    {
        FractureBars[gameManager.instance.playerWeapon.GetCurrentElement()] += amount;

        if (FractureBars[gameManager.instance.playerWeapon.GetCurrentElement()] <= 0 && IsFractured)
        {
            FractureBars[gameManager.instance.playerWeapon.GetCurrentElement()] = 0;
            IsFractured = false;
            //gameManager.instance.playerWeapon.PrimaryFireRate *= FractureFireRateReduction;
            //gameManager.instance.playerWeapon.SecondaryFireRate *= FractureFireRateReduction;
            gameManager.instance.playerWeapon.MenuLock = false;
            gameManager.instance.playerScript.ElementType = gameManager.instance.playerWeapon.GetCurrentElement();

            if (UpgradedSpell) { 
            gameManager.instance.playerWeapon.UpgradedList(gameManager.instance.playerWeapon.GetCurrentElement());
                UpgradedSpell = false;
            }
        }
        else if (FractureBars[gameManager.instance.playerWeapon.GetCurrentElement()] > MaxFractureResistance)
        {
            FractureBars[gameManager.instance.playerWeapon.GetCurrentElement()] = MaxFractureResistance;
            gameManager.instance.playerWeapon.MenuLock = true;
            IsFractured = true;
            gameManager.instance.playerScript.ElementType =DamageEngine.ElementType.Normal;
            //gameManager.instance.playerWeapon.PrimaryFireRate /= FractureFireRateReduction;
            //gameManager.instance.playerWeapon.SecondaryFireRate /= FractureFireRateReduction;
            if (gameManager.instance.playerWeapon.UpgradedList().Contains(gameManager.instance.playerWeapon.GetCurrentElement()))
            {
                gameManager.instance.playerWeapon.UpgradedList(gameManager.instance.playerWeapon.GetCurrentElement(), false);
                UpgradedSpell=true;
            }
        }
    }
    /// <summary>
    /// regenerates the fracture bare and brings it down to zero
    /// </summary>
    /// <returns></returns>
    IEnumerator RegenFracture()
    {
        IsRegenFracture = true;
        yield return new WaitForSeconds(fractureRegenRate);

        foreach (DamageEngine.ElementType Type in Enum.GetValues(typeof(DamageEngine.ElementType)))
        {
            if (FractureBars[Type] < 0 ) 
                FractureBars[Type]= 0;

            else if(FractureBars[Type] > 0)
                FractureBars[Type] -= fractureRegenAmount;
        }
        IsRegenFracture=false;
    }

    #endregion


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

}
