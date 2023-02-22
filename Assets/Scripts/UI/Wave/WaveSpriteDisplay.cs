using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpriteDisplay : MonoBehaviour
{
    List<Sprite> spritesShowing;
    public void addWaveSprite(Sprite spriteToAdd)
    {
        spritesShowing.Add(spriteToAdd);

    }
    //public void removeWaveSprite()
}
