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
    private GameObject lifeRemainderText;
    public int remainingLives;

    bool alive = true;
    bool disabled = false;

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
    private float xVel;
    private SoundCaller sc;
    private float walkSoundCooldown = 0.4f;
    public AudioClip[] walkSounds;
    private Timer walkSoundTimer;
    private CapsuleCollider2D cc;
    public AudioClip timeManipErrorSound;
    public AudioClip[] fireSounds;
    public AudioClip deathSound;
    public float snowBallSpeed;
    private int maxEnergy = 100;
    private int energy;
    private float displayedEnergy;
    private int castCost = 10;
    public float invulTime;
    private Timer invulTimer;
    private float timeScaleAdjustment;
    
    private float localTimeModifier = 1.0f;
    private float localTimeModUpperLimit = 2.0f;
    private float localTimeModLowerLimit = 0.5f;

    private float adjustmentPrTick = 0.25f;



    public float snowballCooldown;
    private Timer snowballTimer;
    public GameObject snowballObject;

    private void Awake()
    {
        snowballTimer = new Timer(snowballCooldown);
        invulTimer = new Timer(invulTime);
        cc = GetComponent<CapsuleCollider2D>();
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
        StartCoroutine(getInvulnerable());
    }

    // Update is called once per frame
    void Update()
    {
        if (alive && !disabled)
        {
            if (MyGameManager.instance.timeScale == 0) //fixedupdate doesn't run when timescale == 0, so need fail-safe in update
            {
                if (Mathf.Approximately(Input.GetAxisRaw(adjustTimeInput), 1))
                {
                    fireDown = true;
                    localTimeModifier += adjustmentPrTick;
                    timeScaleAdjustment = adjustmentPrTick;
                }
            }
            MyGameManager.instance.timeScale += timeScaleAdjustment;
            timeScaleAdjustment = 0.0f; //if it's stupid but it works, it's still stupid but it works for gamejam
        }
    }

    void FixedUpdate()
    {
        if (alive && !disabled)
        {
            handleVelocity(Input.GetAxisRaw(horizontalInput));
            checkGroundCollision();
            if (Mathf.Approximately(Input.GetAxisRaw(jumpInput), 1) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            applyGravity();
            handleFireing(Mathf.Approximately(Input.GetAxisRaw(fireInput), 1));
            handleTimeModifications(Input.GetAxisRaw(adjustTimeInput) / Input.GetAxisRaw(adjustTimeInput));
        }
    }

    internal void setLifeCounterText(GameObject textObject)
    {
        lifeRemainderText = textObject;
        lifeRemainderText.GetComponent<Text>().text = "Lives: " + remainingLives.ToString();
    }

    public void getKnockedBack(Vector2 knockbackVelocity)
    {
        if (invulTimer.hasEnded())
        {
            StartCoroutine(getKnockedBackRoutine(knockbackVelocity));
        }   
    }

    IEnumerator getInvulnerable()
    {
        invulTimer.restart();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor= sr.color;
        Color newColor = originalColor;
        for (float i = 0; i < invulTime; i+=Time.deltaTime)
        {
            newColor.a = (Mathf.Sin(i*16.0f / invulTime)+1.0f)*0.33f+0.33f;
            sr.color = newColor;
            yield return null;
        }
        sr.color = originalColor;
    }

    public IEnumerator getKnockedBackRoutine(Vector2 knockbackVelocity)
    {
        disabled = true;
        rb.velocity += knockbackVelocity;
        Color hitColor = new Color(0.8f, 0.1f, 0);
        GetComponent<SpriteRenderer>().color = hitColor;
        yield return new WaitForSeconds(0.5f);
        disabled = false;
        hitColor = new Color(1, 1, 1);
        GetComponent<SpriteRenderer>().color = hitColor;
    }

    internal void setEnergySlider(GameObject slider)
    {
        energyBar = slider;
        energyBar.GetComponent<Image>().fillAmount = energy / maxEnergy;
    }

    public IEnumerator killPlayer()
    {
        if (alive && invulTimer.hasEnded())
        {
            remainingLives--;
            lifeRemainderText.GetComponent<Text>().text = "Lives: " + remainingLives.ToString();
            if (remainingLives == 0)
            {

            }
            else
            {
                Vector2 positionOfDeath = (Vector2)transform.position+Vector2.up*0.5f;
                rb.velocity = Vector2.zero;
                alive = false;
                sc.attemptSound(deathSound);
                cc.isTrigger = true;
                yield return new WaitForSeconds(0.2f);
                rb.velocity = Vector2.up * 10;
                for (float i = 0; i < 1; i += Time.deltaTime)
                {
                    rb.velocity += Vector2.down * Time.deltaTime * 20.0f;
                    yield return null;
                }
                float respawnDelay = 4.5f;
                GameObject respawnText = new GameObject("respawnText");
                respawnText.transform.position = positionOfDeath;
                TextMesh rst = respawnText.AddComponent<TextMesh>(); //restartText
                rst.anchor = TextAnchor.MiddleRight;
                rst.characterSize = 0.3f;
                rst.fontSize = 12;
                for (float i = 0; i < respawnDelay; i += Time.deltaTime)
                {
                    rst.text = "Respawn in: " + (respawnDelay - i).ToString(".0"); //may not need text
                    yield return null;
                }
                Destroy(respawnText);
                //respawn
                alive = true;
                cc.isTrigger = false;
                transform.position = positionOfDeath;
                StartCoroutine(getInvulnerable());
                rb.velocity = Vector2.zero;
            }            
        }

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
        if (isFireing && snowballTimer.hasEnded())
        {
            snowballTimer.restart();
            if (fireDown == false)
            {
                fireDown = true;
            }
            sc.attemptSound(fireSounds[UnityEngine.Random.Range(0, fireSounds.Length)], 5.0f);

            GameObject snowball = Instantiate(snowballObject);
            snowball.GetComponent<SnowballScript>().setThrower(gameObject);
            Vector2 shootDirection;
            shootDirection.x = Input.GetAxis("ShootAxisXOne");
            shootDirection.y = -Input.GetAxis("ShootAxisYOne");
            print(shootDirection);
            shootDirection.Normalize();
            Vector2 finaldirection = Vector2.zero;
            float threshold = 0.097f;
            if (shootDirection.x > threshold)
            {
                finaldirection.x = 1.0f;
            }
            if (shootDirection.x < -threshold)
            {
                finaldirection.x = -1.0f;
            }
            if (shootDirection.y > threshold)
            {
                finaldirection.y = 1.0f;
            }
            if (shootDirection.y < -threshold)
            {
                finaldirection.y = -1.0f;
            }
            if (finaldirection.x ==0 && finaldirection.y == 0)
            {
                finaldirection = Vector2.right;
            }
            else
            {
                finaldirection.Normalize();
            }

            snowball.transform.position = (Vector2)transform.position + finaldirection * 0.5f;
            snowball.GetComponent<Rigidbody2D>().velocity = snowBallSpeed*finaldirection;
        }
        else
        {
            fireDown = false;
        }
    }
    public bool getGrounded()
    {
        return isGrounded;
    }
}
