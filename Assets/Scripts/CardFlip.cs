using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlip : MonoBehaviour
{
    public Sprite CardFront;
    public Sprite CardBack;

    public void Flip()
    {
        Sprite currentSprite = gameObject.GetComponent<Image>().sprite;

        if (currentSprite == CardBack)
        {
            gameObject.GetComponent<Image>().sprite = CardFront;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = CardBack;
        }
    }
}
