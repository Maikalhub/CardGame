using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteCycler : MonoBehaviour
{
    public Image TargetImage;
    public List<Sprite> Sprites = new List<Sprite>();
    public float timeBetweenSwitches;
    public bool randomize;
    private int imageInt;

    private void Start()
    {
        StartCoroutine(SpriteChangeLoop());
    }

    private void Update()
    {
        TargetImage.sprite = Sprites[imageInt];
    }

    IEnumerator SpriteChangeLoop()
    {
        yield return new WaitForSeconds(timeBetweenSwitches);
        if (randomize)
        {
            int targetImage = Random.Range(0, Sprites.Count - 1);
            imageInt = targetImage;
        }
        else
        {
            if(imageInt == Sprites.Count - 1)
            {
                imageInt = 0;
            }
            else
            {
                imageInt += 1;
            }
        }

        StartCoroutine(SpriteChangeLoop());
    }
}
