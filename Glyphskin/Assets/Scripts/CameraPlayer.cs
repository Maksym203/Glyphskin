using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    public Transform target;
    public Player player;

    public float maxDownOffset = -1f;
    public float fallDelay = 0.1f;
    public float fallDuration = 1f;
    public float riseDuration = 0.3f;

    private float currentYOffset = 0f;
    private float offsetTimer = 0f;
    private bool wasFalling = false;

    private void LateUpdate()
    {
        if (target == null || player == null) return;

        float targetX = target.position.x;

        if (player.currentState == Player.PlayerState.JumpDown)
        {
            if (!wasFalling)
            {
                offsetTimer = 0f;
                wasFalling = true;
            }

            offsetTimer += Time.deltaTime;

            if (offsetTimer >= fallDelay)
            {
                float progress = Mathf.Clamp01((offsetTimer - fallDelay) / fallDuration);
                currentYOffset = Mathf.Lerp(0f, maxDownOffset, progress);
            }
        }
        else
        {
            if (wasFalling)
            {
                offsetTimer = 0f;
                wasFalling = false;
            }

            offsetTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(offsetTimer / riseDuration);
            currentYOffset = Mathf.Lerp(currentYOffset, 0f, progress);
        }

        transform.position = new Vector3(
            targetX,
            target.position.y + currentYOffset,
            transform.position.z
        );
    }
}
