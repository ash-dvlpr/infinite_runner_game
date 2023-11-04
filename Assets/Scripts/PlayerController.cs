using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private const string GROUND_LAYER_NAME = "Ground";
    private float _groundDetectionRange = 1.1f;

    // Movement variables
    [SerializeField] private float mvSpeed      = 1f;
    [SerializeField] private float jumpForce    = 90f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float minJumpDelay = 0.1f;
    private float velocity, lastJumpT;


    // Inputs & States
    private float iHorAxis;
    private bool  iJumpPressed     = false;
    private bool  isTouchingGround = false;
    

    // References & Components
    private Rigidbody2D _rb;
    private Animator _animator;
    private LayerMask _groundLayer;

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
        HandleMovement();
    }

    void OnDrawGizmos() {
        //Gizmos.DrawWireSphere(this.transform.position, _distanceRaycast);
        Gizmos.DrawRay(transform.position, Vector2.down * _groundDetectionRange);
    }

    // ========================= Custom Code ========================
    void UpdateInputs() {
        isTouchingGround = Physics2D.Raycast(transform.position, Vector2.down, _groundDetectionRange, _groundLayer);
        
        // TODO: PC specific code
        iJumpPressed = Input.GetMouseButton(0);
        iHorAxis = Input.GetAxisRaw("Horizontal");

        // TODO: Android specific code
        // TODO: XBOX specific code
    }
    void PreHandleMovement() {
        //? Moving
        iHorAxis = 1f;
        if (iHorAxis != 0.0f) {
            velocity = Mathf.Clamp(
                velocity + iHorAxis * acceleration * Time.deltaTime,
                -1.0f, 1.0f
            );
        }
        // Gradual slow down
        else {
            velocity -= velocity * acceleration * Time.deltaTime;
        }

        //? Jumping
        iJumpPressed = true;
        if (iJumpPressed && isTouchingGround && lastJumpT < Time.time - minJumpDelay) {
            _rb.AddForce(Vector2.up * jumpForce);
            lastJumpT = Time.time;
        }
    }
    void HandleMovement() {
        float hVel = Mathf.Abs(velocity) < 0.01f ? 0.0f : velocity;

        _rb.velocity = new Vector2(hVel * mvSpeed, _rb.velocity.y);
    }
    void UpdateAnimationState() { 
        
    }
}
