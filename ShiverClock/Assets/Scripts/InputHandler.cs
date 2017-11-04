using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public enum PlayerID
    {
        PlayerOne,
        PlayerTwo,
        PlayerThree,
        PlayerFour
    }
    public PlayerID playerID = 0;
    public int moveSpeed = 1;
    private IEnumerator energyBadCoroutine;
    private GameObject energyBar;

    private Rigidbody2D rb;
    private float jumpForce = 15.0f;
    private float maxHorVelocity = 5.0f;
    private float customGravity = 25.0f;
    private float speed = 3.5f;
    private string horizontalInput = "";
    private string jumpInput = "";
    private string adjustTimeInput = "";
    private string fireInput = "";
    bool adjustTimeDown = false;
    bool fireDown = false;
    bool isGrounded = false;
    public float xVel;
    private SoundCaller sc;
    private float walkSoundCooldown = 0.4f;
    public AudioClip[] walkSounds;
    private Timer walkSoundTimer;

    public AudioClip timeManipErrorSound;
    public AudioClip[] fireSounds;

    private int maxEnergy = 100;
    private int energy;
    private float displayedEnergy;
    private int castCost = 10;

    private float timeScaleAdjustment;
    
    private float localTimeModifier = 1.0f;
    private float localTimeModUpperLimit = 2.0f;
    private float localTimeModLowerLimit = 0.5f;

    internal void setEnergySlider(GameObject slider)
    {
        energyBar = slider;
        energyBar.GetComponent<Image>().fillAmount = energy / maxEnergy;
    }

    private float adjustmentPrTick = 0.25f;
    private void Awake()
    {
        energy = maxEnergy;
        displayedEnergy = energy;
        energyBadCoroutine = null;
        walkSoundTimer = new Timer(walkSoundCooldown);
        sc = GetComponent<SoundCaller>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start()
    {
        switch (playerID)
        {
            case PlayerID.PlayerOne:
                horizontalInput = "HorizontalOne";
                jumpInput = "JumpOne";
                adjustTimeInput = "AdjustTimeOne";
                fireInput = "FireOne";
                break;
            case PlayerID.PlayerTwo:
                horizontalInput = "HorizontalTwo";
                jumpInput = "JumpTwo";
                adjustTimeInput = "AdjustTimeTwo";
                fireInput = "FireTwo";
                break;
            case PlayerID.PlayerThree:
            case PlayerID.PlayerFour:
            default:
                //print("Error. wrong PlayerID. defaulting to player1");
                horizontalInput = "HorizontalOne";
                jumpInput = "JumpOne";
                adjustTimeInput = "AdjustTimeOne";
                fireInput = "FireOne";
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

        //if (!Mathf.Approximately(MyGameManager.instance.timeScale, 0))
        //{
        //    localDt = Time.deltaTime * (1.0f / MyGameManager.instance.timeScale);
        //}
        //else
        //{
        //    localDt = Time.deltaTime * (1.0f / 0.0001f);
        //}

        if (MyGameManager.instance.timeScale == 0) //fixedupdate doesn't run when timescale == 0, so need fail-safe in update
        {
            if (Mathf.Approximately(Input.GetAxisRaw(adjustTimeInput), 1))
            {
                fireDown = true;
                localTimeModifier += adjustmentPrTick;
                timeScaleAdjustment = adjustmentPrTick;
            }
        }
        //if (Mathf.Abs(rb.velocity.x) > 0.01 && walkSoundTimer.hasEnded())
        //{
        //    sc.attemptSound(walkSounds[UnityEngine.Random.Range(0, walkSounds.Length)],1.0f);
        //    walkSoundTimer.restart();
        //}
        MyGameManager.instance.timeScale += timeScaleAdjustment;
        timeScaleAdjustment = 0.0f; //if it's stupid but it works, it's still stupid but it works for gamejam

        
    }


    private void modifyEnergy(int value)
    {
        energy += value;
        if (energyBadCoroutine != null)
        {
            StopCoroutine(energyBadCoroutine);
        }
        energyBadCoroutine = smoothLerpEnergyBar(value);
        StartCoroutine(energyBadCoroutine);
        //energyBar.GetComponentsInChildren<Image>()[1].fillAmount = (float)energy / maxEnergy;
    }
    IEnumerator smoothLerpEnergyBar(int value)
    {
        float slerpTime = 0.4f;
        for (float i = 0; i < slerpTime; i+=Time.deltaTime)
        {
            float pd = i / slerpTime;
            float smooth = pd * pd * (3 - 2 * pd);
            displayedEnergy = Mathf.Lerp(displayedEnergy, energy, smooth);
            float displayEnergy = displayedEnergy-(value*(1.0f-smooth)) + (value * smooth);
            energyBar.GetComponentsInChildren<Image>()[1].fillAmount = displayedEnergy / maxEnergy;
            yield return null;
        }
    }

    void FixedUpdate()
    {
        //if (!Mathf.Approximately(MyGameManager.instance.timeScale, 0))
        //{
        //    localFDtMultiplier = (1.0f / MyGameManager.instance.timeScale);
        //}
        //else
        //{
        //    localFDtMultiplier = (1.0f / 0.0001f);
        //}
        //localFDt = Time.fixedDeltaTime * localFDtMultiplier;
        handleVelocity(Input.GetAxisRaw(horizontalInput));
        checkGroundCollision();
        if (Mathf.Approximately(Input.GetAxisRaw(jumpInput), 1) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        applyGravity();
        handleFireing(Mathf.Approximately(Input.GetAxisRaw(fireInput), 1));
        handleTimeModifications(Input.GetAxisRaw(adjustTimeInput));
    }

    private void checkGroundCollision()
    {
        CapsuleCollider2D cc = GetComponent<CapsuleCollider2D>();
        cc.enabled = false; //hacky but works 4 gamejam
        Vector2 origin = (Vector2)transform.position + Vector2.down * transform.lossyScale.y * 0.5f;
        Vector2 size = transform.lossyScale;
        size.y *= 0.05f;
        size.x *= 0.98f;
        RaycastHit2D boxHit = Physics2D.BoxCast(origin, size, 0, Vector2.down, 0.1f);
        if (boxHit && (rb.velocity.y < 0 || Mathf.Approximately(rb.velocity.y, 0.0f))) //dunno if approx check is needed, but just to be safe for the jam
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        cc.enabled = true;
    }

    private void handleVelocity(float input)
    {
        Vector2 newVelocity = rb.velocity;
        newVelocity.x = input * speed;//*localFDtMultiplier;
        rb.velocity = newVelocity;
        if (!Mathf.Approximately(input,0) && walkSoundTimer.hasEnded() && isGrounded) //strange place, for walksound, but isok
        {
            sc.attemptSound(walkSounds[UnityEngine.Random.Range(0, walkSounds.Length)]);
            walkSoundTimer.restart();
        }
        xVel = rb.velocity.x;
    }

    //only call during fixedUpdate
    private void applyGravity()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - customGravity*Time.fixedDeltaTime);//*localFDt);
    }

    private void handleTimeModifications(float input)
    {
        if (!Mathf.Approximately(input, 0))
        {
            if (adjustTimeDown == false)
            {
                adjustTimeDown = true;
                if (!Mathf.Approximately(localTimeModifier, localTimeModLowerLimit) && input < 0 && energy >= castCost)
                {

                    localTimeModifier -= adjustmentPrTick;
                    timeScaleAdjustment = -adjustmentPrTick;
                    modifyEnergy(-castCost);
                }
                else if (!Mathf.Approximately(localTimeModifier, localTimeModUpperLimit) && input > 0 && energy >= castCost) //too ugly?
                {
                    localTimeModifier += adjustmentPrTick;
                    timeScaleAdjustment = adjustmentPrTick;
                    modifyEnergy(-castCost);
                }
                else
                {
                    sc.attemptSound(timeManipErrorSound, 5.0f);
                    //play error sound, maybe animation
                }
            }

        }
        else
        {
            adjustTimeDown = false;
        }
    }

    private void handleFireing(bool isFireing)
    {
        if (isFireing)
        {
            if (fireDown == false)
            {
                fireDown = true;
            }
            sc.attemptSound(fireSounds[UnityEngine.Random.Range(0, fireSounds.Length)], 5.0f);

            //fire...
        }
        else
        {
            fireDown = false;
        }
    }
}
