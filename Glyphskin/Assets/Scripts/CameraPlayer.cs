using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        if (target == null) return;

        // Keep camera centered on the player
        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );
    }
}
