using UnityEngine;

public class AttackSwing : MonoBehaviour
{
    public float lifetime = 0.08f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
