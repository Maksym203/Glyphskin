using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;

    private PlayerState currentState;

    private enum PlayerState
    {
        IdleLeft,
        IdleRight,
        WalkLeft,
        WalkRight
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
        moveInput.x = Input.GetAxisRaw("Horizontal");
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void UpdateState()
    {
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
        }
    }
}
