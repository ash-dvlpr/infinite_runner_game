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
    [Header("Difficulty/Survivavility")]
    [SerializeField] private int maxHealth;

    [Header("Jump Configuration")]
    [SerializeField] private float groundDetectionRange =  1.1f;
    [SerializeField] private float jumpForce            = 11.0f;
    [SerializeField] private float jumpGravity          =  1.0f;
    [SerializeField] private float fallGravity          =  3.0f;
    [SerializeField] private float jumpMaxTime          =  0.2f;

    private float jumpTimeCounter, distanceTraveled;
    private bool  isJumping, isRunning, isDead;
    private Vector2 startPosition;
    private int health;
    public int Health {
        get => health; 
        set { health = Mathf.Clamp(value, 0, maxHealth); }
    }

    //? Inputs & States
    bool iJumpPressed     = false;
    bool isTouchingGround = false;

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
        if(GameManager.GetState() == GameManager.GameState.InGame) {
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
    }
    void OnGameOver() {
        isDead = true;
        isRunning = false;
        _rb.simulated = false;
    }

    void UpdateInputs() {
        // TODO: PC specific code
        iJumpPressed = Input.GetKey(KeyCode.Space);

        // TODO: Android specific code
        // TODO: XBOX specific code
    }
    void PreHandleMovement() {
        //? JUMP
        if (iJumpPressed) {
            // Start jumping 
            if (isTouchingGround) {
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
        _rb.velocity = new Vector2(Vector2.right.x * GameManager.Instance.PlatformSpeed, _rb.velocity.y);
    }
    void UpdateAnimationState() {
        _animator.SetBool(ANIM_ID_PARAM_GROUNDED, isTouchingGround);
        _animator.SetBool(ANIM_ID_PARAM_RUNNING,  isRunning);
        _animator.SetBool(ANIM_ID_PARAM_DEAD,     isDead);
    }

    // ===================== Outside Facing API ======================
    public void AddScore(int value) {
        GameManager.AddScore(value);
    }
    public void AddHealth(int value) {
        Health += value;
    }
}
