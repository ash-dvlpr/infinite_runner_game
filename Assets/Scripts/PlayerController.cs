using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private const string GROUND_LAYER_NAME = "Ground";

    // JUMP variables
    [Header("Jump Configuration")]
    [SerializeField] private float groundDetectionRange =  1.1f;
    [SerializeField] private float jumpForce            = 11.0f;
    [SerializeField] private float jumpGravity          =  1.0f;
    [SerializeField] private float fallGravity          =  3.0f;
    [SerializeField] private float jumpMaxTime          =  0.2f;

    private float jumpTimeCounter;
    private bool  isJumping;

    // Inputs & States
    bool iJumpClicked     = false;
    bool iJumpPressed     = false;
    bool isTouchingGround = false;

    // References & Components
    Rigidbody2D _rb;
    Animator    _animator;
    LayerMask   _groundLayer;

    // ========================= Unity Code =========================
    void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _groundLayer = LayerMask.GetMask(GROUND_LAYER_NAME);
    }

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        UpdateInputs();
        PreHandleMovement();
        UpdateAnimationState();
    }

    void FixedUpdate() {
        isTouchingGround = Physics2D.Raycast(transform.position, Vector2.down, groundDetectionRange, _groundLayer);
        // TODO: Reset extra jumps

        HandleMovement();
    }

    void OnDrawGizmos() {
        //Gizmos.DrawWireSphere(this.transform.position, _distanceRaycast);
        Gizmos.DrawRay(transform.position, Vector2.down * groundDetectionRange);
    }

    // ========================= Custom Code ========================
    void UpdateInputs() {
        // TODO: PC specific code
        iJumpClicked = Input.GetMouseButtonDown(0);
        iJumpPressed = Input.GetMouseButton(0);

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
        _rb.gravityScale = isJumping ? jumpGravity : fallGravity;
    }
    void HandleMovement() {

    }
    void UpdateAnimationState() {
        _animator.SetBool("IsGrounded", isTouchingGround);
    }
}
