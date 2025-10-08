using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LifeManager : MonoBehaviour
{
    public int maxLife = 5;
    public int currentLife = 5;

    public GameObject heartPrefab;
    public Transform heartContainer;

    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Sprite hitEmptyHeart;
    public Sprite hitHeart;

    private List<Image> hearts = new List<Image>();

    void Start()
    {
        //Clear container first
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }
        hearts.Clear();

        currentLife = maxLife;

        //Create heart images
        for (int i = 0; i < maxLife; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            Image heartImage = heart.GetComponent<Image>();
            heartImage.sprite = i < currentLife ? fullHeart : emptyHeart;
            hearts.Add(heartImage);
        }

        UpdateHearts(currentLife);
    }

    public void UpdateHearts(int newLife)
    {
        currentLife = Mathf.Clamp(newLife, 0, maxLife);
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].sprite = i < currentLife ? fullHeart : emptyHeart;
        }
    }

    public void FlickerHearts()
    {
        StartCoroutine(FlickerCoroutine());
    }

    private IEnumerator FlickerCoroutine()
    {
        float flickerDuration = 0.8f;
        float flickerInterval = 0.1f;
        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            for (int i = 0; i < hearts.Count; i++)
            {
                if (i < currentLife)
                    hearts[i].sprite = hearts[i].sprite == fullHeart ? hitHeart : fullHeart;
                else
                    hearts[i].sprite = hearts[i].sprite == emptyHeart ? hitEmptyHeart : emptyHeart;
            }

            elapsed += flickerInterval;
            yield return new WaitForSeconds(flickerInterval);
        }

        UpdateHearts(currentLife);
    }
}
