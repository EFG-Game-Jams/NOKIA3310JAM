using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarfieldGenerator : MonoBehaviour
{
    public PixelPerfectSprite sparkSprite;

    [Range(0, 100)]
    [Tooltip("At speed, about half of these will be visible at any given time")]
    public int starCount;

    [Range(-100, 100)]
    public float speed;

    private struct Star
    {
        public PixelPerfectSprite sprite;
        public Vector2 position;
    }
    private List<Star> stars = new List<Star>();

    private int StarWidth => Mathf.Max(1, Mathf.RoundToInt(5 * Mathf.Abs(speed) / 100));

    void AddStar()
    {
        var star = new Star();
        star.sprite = Instantiate(sparkSprite, transform);
        InitialiseStarPosition(ref star);
        UpdateStarSize(star);
        stars.Add(star);
    }
    void RemoveStar()
    {
        int index = stars.Count - 1;
        Destroy(stars[index].sprite.gameObject);
        stars.RemoveAt(index);
    }

    void InitialiseStarPosition(ref Star star)
    {
        int x = Random.Range(0, 83);
        int y = Random.Range(0, 47);
        if (speed < 0)
            x += 84 + StarWidth / 2;
        else if (speed > 0)
            x -= 84 + StarWidth / 2;

        star.position = new Vector2(x, y);
        star.sprite.transform.localPosition = star.position;
    }
    void UpdateStarSize(Star star)
    {
        star.sprite.transform.localScale = new Vector3(StarWidth, 1, 1);
    }

    private void Awake()
    {
        Update();
    }
    void Update()
    {
        // update count
        while (stars.Count < starCount)
            AddStar();
        while (stars.Count > starCount)
            RemoveStar();

        // move all
        Vector2 movement = new Vector2(speed * Time.deltaTime, 0);
        for (int i=0; i<stars.Count; ++i)
        {
            // move
            Star star = stars[i];
            star.position += movement;
            star.sprite.transform.position = star.position;
            UpdateStarSize(star);

            // off screen
            if (speed < 0 && star.position.x < -StarWidth)
                InitialiseStarPosition(ref star);
            else if (speed > 0 && star.position.x > 83 + StarWidth)
                InitialiseStarPosition(ref star);

            stars[i] = star;
        }
    }
}
