using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    //Basic animation controller, to be changed later on for a more specialized one
    public Sprite[] frames;
    public float framesPerSecond = 5f;

    private SpriteRenderer sr;
    private int currentFrame;
    private float timer;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / framesPerSecond)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            sr.sprite = frames[currentFrame];
        }
    }
}
