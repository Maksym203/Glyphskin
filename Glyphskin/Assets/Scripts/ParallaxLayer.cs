using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float parallaxFactor = 0.5f;
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
        transform.position = startPos + (cam.position * parallaxFactor);
    }
}
