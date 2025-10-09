using UnityEngine;

public class AttackSwing : MonoBehaviour
{
    public float lifetime = 0.08f;

    public float xOffset = 0f;
    public float yOffset = 0f;
    public bool downwardsAttack = false;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = new Vector3(
                playerTransform.position.x + xOffset,
                playerTransform.position.y + yOffset,
                transform.position.z
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && downwardsAttack)
        {
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            player.EnemyJump();
        }
    }
}

