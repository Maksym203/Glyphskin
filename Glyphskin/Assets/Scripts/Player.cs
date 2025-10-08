using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 2f;
    public float jumpDuration = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;

    private PlayerState currentState;
    private bool isJumping = false;
    private float jumpTimeCounter = 0f;
    private bool isGrounded = true;

    private float zeroTimerDuration = 0.05f;
    private float zeroThreshold = 0.01f;
    private float zeroTimer = 0f;
    private bool zeroTimerRunning = false;
    private bool zeroTimerExpired = false;
    private float maxZeroTime = 0.15f;

    private bool jumpTopAnimation = false;
    private float jumpTopCounter = 0f;

    private bool isAttacking = false;
    private float attackTimer = 0f;
    private bool facingRight = true;

    private enum PlayerState
    {
        IdleLeft,
        IdleRight,
        WalkLeft,
        WalkRight,
        JumpStart,
        JumpUp,
        JumpTop,
        JumpDown,
        JumpLand,
        AttackLeft,
        AttackRight,
        AttackDown,
        AttackUp
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        UpdateState();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void HandleInput()
    {
        //Get stick input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical);

        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J)) && !isAttacking)
        {
            isAttacking = true;
            attackTimer = 0f;

            //Directional attacks
            if (vertical > 0.5f)
            {
                currentState = PlayerState.AttackUp;
            }
            else if (vertical < -0.5f && !isGrounded)
            {
                currentState = PlayerState.AttackDown;
            }
            else
            {
                currentState = facingRight ? PlayerState.AttackRight : PlayerState.AttackLeft;
            }

            return;
        }

        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= 0.3f)
            {
                isAttacking = false;
                attackTimer = 0f;
            }
        }

        if (!isGrounded)
        {
            float vy = rb.linearVelocity.y;
            bool nearlyZero = Mathf.Abs(vy) <= zeroThreshold;

            if (!zeroTimerRunning && !zeroTimerExpired && nearlyZero)
            {
                zeroTimerRunning = true;
                zeroTimer = 0f;
            }

            if (zeroTimerRunning)
            {
                zeroTimer += Time.deltaTime;

                if (zeroTimer >= maxZeroTime)
                {
                    isJumping = false;
                    isGrounded = true;
                    zeroTimerRunning = false;
                    zeroTimerExpired = false;
                    zeroTimer = 0f;
                }

                if (zeroTimer >= zeroTimerDuration)
                {
                    zeroTimerRunning = false;
                    zeroTimerExpired = true;
                }
            }

            if (zeroTimerExpired && nearlyZero)
            {
                isJumping = false;
                isGrounded = true;
                zeroTimerExpired = false;
                zeroTimer = 0f;
                zeroTimerRunning = false;
            }
        }
        else
        {
            zeroTimer = 0f;
            zeroTimerRunning = false;
            zeroTimerExpired = false;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && currentState != PlayerState.JumpDown)
        {
            isJumping = true;
            jumpTimeCounter = 0f;
            isGrounded = false;

            float horizontalVelocity = rb.linearVelocity.x;
            if (Mathf.Abs(moveInput.x) > 0.1f)
                horizontalVelocity = moveInput.x * moveSpeed;
            else
                horizontalVelocity = 0f;

            rb.linearVelocity = new Vector2(horizontalVelocity, jumpForce);
        }

        if (Input.GetButton("Jump") && isJumping && currentState != PlayerState.JumpDown)
        {
            jumpTimeCounter += Time.deltaTime;
            if (jumpTimeCounter < jumpDuration)
            {
                float t = jumpTimeCounter / jumpDuration;
                float curve = Mathf.Pow(1f - t, 1.1f);
                float adjustedForce = jumpForce * curve;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, adjustedForce);
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump") && currentState != PlayerState.JumpDown)
        {
            isJumping = false;
        }
    }


    private void Move()
    {
        float horizontalDirection = 0f;

        if (moveInput.x >= 0.1f)
            horizontalDirection = 1f;
        else if (moveInput.x <= -0.1f)
            horizontalDirection = -1f;

        float currentMoveSpeed = moveSpeed;

        if (isGrounded)
        {
            // Grounded: full speed if moving, else 0
            rb.linearVelocity = new Vector2(horizontalDirection * currentMoveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Midair: full speed if moving horizontally, else slower drift
            if (Mathf.Abs(horizontalDirection) > 0f)
            {
                rb.linearVelocity = new Vector2(horizontalDirection * currentMoveSpeed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.8f, rb.linearVelocity.y); // slower drift
            }
        }

        //Update facing direction
        if (horizontalDirection > 0f)
            facingRight = true;
        else if (horizontalDirection < 0f)
            facingRight = false;
    }

    private void UpdateState()
    {
        if (isAttacking)
        {
            //Dont override if we are already in AttackUp or AttackDown
            if (currentState == PlayerState.AttackUp || currentState == PlayerState.AttackDown)
                return;

            //Otherwise use left/right attack as default
            currentState = facingRight ? PlayerState.AttackRight : PlayerState.AttackLeft;
            return;
        }

        float vy = rb.linearVelocity.y;

        //Midair
        if (!isGrounded)
        {
            if (isJumping && jumpTimeCounter < 0.05f)
            {
                currentState = PlayerState.JumpStart;
                jumpTopAnimation = false;
            }
            else if (vy > 0.01f)
                currentState = PlayerState.JumpUp;
            else if (Mathf.Abs(vy) <= 0.01f)
            {
                currentState = PlayerState.JumpTop;
            }
            else if (vy < -0.01f)
            {
                if (jumpTopAnimation == false && jumpTopCounter <= 0.1f)
                {
                    currentState = PlayerState.JumpTop;
                    jumpTopCounter += Time.deltaTime;
                }
                else
                {
                    jumpTopAnimation = true;
                    jumpTopCounter = 0f;
                    currentState = PlayerState.JumpDown;
                }
            }

            return;
        }

        //Falling check even if grounded
        if (vy < -0.01f)
        {
            currentState = PlayerState.JumpDown;
            jumpTopAnimation = true;
            jumpTopCounter = 0f;
            return;
        }

        //To fix landing stuck animation
        currentState = PlayerState.IdleRight;

        //Grounded
        if (moveInput.x > 0.1f)
            currentState = PlayerState.WalkRight;
        else if (moveInput.x < -0.1f)
            currentState = PlayerState.WalkLeft;
        else if (currentState == PlayerState.WalkRight)
            currentState = PlayerState.IdleRight;
        else if (currentState == PlayerState.WalkLeft)
            currentState = PlayerState.IdleLeft;
    }

    private void UpdateAnimation()
    {
        switch (currentState)
        {
            case PlayerState.IdleRight:
                animator.Play("Idle_Right");
                break;
            case PlayerState.IdleLeft:
                animator.Play("Idle_Left");
                break;
            case PlayerState.WalkRight:
                animator.Play("Walk_Right");
                break;
            case PlayerState.WalkLeft:
                animator.Play("Walk_Left");
                break;
            case PlayerState.JumpStart:
                animator.Play("Jump_Start");
                break;
            case PlayerState.JumpUp:
                animator.Play("Jump_Up");
                break;
            case PlayerState.JumpTop:
                animator.Play("Jump_Start");
                break;
            case PlayerState.JumpDown:
                animator.Play("Jump_Down");
                break;
            case PlayerState.JumpLand:
                animator.Play("Jump_Start");
                break;
            case PlayerState.AttackRight:
                animator.Play("Attack_Right");
                break;
            case PlayerState.AttackLeft:
                animator.Play("Attack_Left");
                break;
            case PlayerState.AttackUp:
                animator.Play("Attack_Up");
                break;
            case PlayerState.AttackDown:
                animator.Play("Attack_Down");
                break;
        }
    }
}
