using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private const string GROUND_LAYER_NAME = "Ground";
    private int ANIM_ID_PARAM_GROUNDED = Animator.StringToHash("IsGrounded");
    private int ANIM_ID_PARAM_RUNNING  = Animator.StringToHash("IsRunning");
    private int ANIM_ID_PARAM_DEAD     = Animator.StringToHash("IsDead");

    //! ========================= Variables ==========================
    //? JUMP
    [Header("Jump Configuration")]
    [SerializeField] private float groundDetectionRange =  1.1f;
    [SerializeField] private float jumpForce            = 11.0f;
    [SerializeField] private float jumpGravity          =  1.0f;
    [SerializeField] private float fallGravity          =  3.0f;
    [SerializeField] private float jumpMaxTime          =  0.2f;

    //? Interactions
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deathSound;

    //? Internal
    private float distanceTraveled;
    private int _health;
    public int Health {
        get => _health;
        set { _health = Mathf.Clamp(value, 0, GameManager.GetDifficultySettings.PlayerMaxHealth); }
    }

    //? Inputs & States
    bool iJumpPressed     = false;
    bool isTouchingGround = false;
    private Vector2 startPosition;
    private bool  isJumping, isRunning, isDead, isInvulnerable;
    private float jumpTimeCounter;

    //? References & Components
    Rigidbody2D _rb;
    Animator    _animator;
    LayerMask   _groundLayer;

    //! ========================= Unity Code =========================
    void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundLayer = LayerMask.GetMask(GROUND_LAYER_NAME);

        // Subscribe events
        GameManager.Instance.OnGameStart += OnGameStart;
        GameManager.Instance.OnGameOver  += OnGameOver;
    }

    void OnDestroy() {
        // Unsubscribe events
        if (GameManager.Instance) {
            GameManager.Instance.OnGameStart -= OnGameStart;
            GameManager.Instance.OnGameOver  -= OnGameOver;
        }
    }
    void Start() {
        startPosition = transform.position;
    }

    void Update() {
        if (GameManager.GetState() == GameManager.GameState.InGame) {
            UpdateInputs();
            PreHandleMovement();
        }
        UpdateAnimationState();
    }

    void FixedUpdate() {
        isTouchingGround = Physics2D.Raycast(transform.position, Vector2.down, groundDetectionRange, _groundLayer);
        if (GameManager.GetState() == GameManager.GameState.InGame) {
            HandleMovement();
            distanceTraveled = transform.position.x - startPosition.x;
            GameManager.UpdateDistanceTraveled(distanceTraveled);
        }
    }

    void OnDrawGizmos() {
        //Gizmos.DrawWireSphere(this.transform.position, _distanceRaycast);
        Gizmos.DrawRay(transform.position, Vector2.down * groundDetectionRange);
    }

    //! ========================= Custom Code ========================
    void OnGameStart() {
        isDead = false;
        isRunning = true;
        _rb.simulated = true;

        Health = GameManager.GetDifficultySettings.PlayerMaxHealth;
        // Start corroutines
        StartCoroutine(DrainHealth());
    }
    void OnGameOver() {
        SoundManager.PlayClip(deathSound);
        isDead = true;
        isRunning = false;
        _rb.simulated = false;
        // Stop corroutines
        StopAllCoroutines();
    }

    IEnumerator DrainHealth() {
        while (true) {
            yield return new WaitForSeconds(GameManager.GetDifficultySettings.PlayerHealthDrainRate);
            Health--;
        }
    }

    void ResetInvulneravility() {
        isInvulnerable = false;
    }

    void UpdateInputs() {
#if UNITY_STANDALONE || UNITY_EDITOR
        // PC specific code
        iJumpPressed = Input.GetKey(KeyCode.Space);
#elif UNITY_ANDROID    
        // Android specific code
        if (Input.touchCount > 0) {
            var touch = Input.GetTouch(0).phase;
            iJumpPressed = touch == TouchPhase.Began || touch == TouchPhase.Moved || touch == TouchPhase.Stationary;
        }

#endif
        // TODO: XBOX specific code
    }
    void PreHandleMovement() {
        //? JUMP
        if (iJumpPressed) {
            // Start jumping 
            if (isTouchingGround) {
                if (!isJumping) SoundManager.PlayClip(jumpSound);
                _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                jumpTimeCounter = jumpMaxTime;
                isJumping = true;
            }
            // Make higher jump is input was held
            else if (isJumping) {
                if (jumpTimeCounter > 0) {
                    _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                    jumpTimeCounter -= Time.deltaTime;
                }
                else {
                    isJumping = false;
                }
            }
        }
        else {
            isJumping = false;
        }

        // Update gravity when jumping/falling
        _rb.gravityScale = iJumpPressed && isJumping ? jumpGravity : fallGravity;
    }
    private void HandleMovement() {
        _rb.velocity = new Vector2(GameManager.Instance.PlatformSpeed, _rb.velocity.y);
    }
    void UpdateAnimationState() {
        _animator.SetBool(ANIM_ID_PARAM_GROUNDED, isTouchingGround);
        _animator.SetBool(ANIM_ID_PARAM_RUNNING, isRunning);
        _animator.SetBool(ANIM_ID_PARAM_DEAD, isDead);
    }

    // ===================== Outside Facing API ======================
    public void AddCoins(int value) {
        GameManager.AddCoins(value);
    }
    public void AddScore(int value) {
        GameManager.AddScore(value);
    }
    public void AddHealth(int value) {
        Health += value;
    }
    public void DealDamage(int value) {
        if (!isInvulnerable) {
            Health -= value;
            isInvulnerable = true;
            Debug.Log("Damaged");
            Invoke("ResetInvulneravility", GameManager.GetDifficultySettings.PlayerInvulnerabilityTime);
        }
    }
}
