using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float parallaxFactor = 0.5f;

    [SerializeField]
    private bool reduceVerticalSpeed = false;

    private Transform cam;
    private Vector3 startPos;

    private void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position;
    }

    private void LateUpdate()
    {
        if (!cam) return;

        Vector3 newPos = cam.position * parallaxFactor;

        if (reduceVerticalSpeed)
        {
            newPos.y = cam.position.y * (parallaxFactor * 0.25f);
        }

        transform.position = startPos + newPos;
    }
}
