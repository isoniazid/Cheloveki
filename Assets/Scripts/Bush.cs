using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Bush : Plant
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;
    // Start is called before the first frame update

    private void ChangeSprite()
    {
        var rndGen = new System.Random();
        var randIndex = rndGen.Next(spriteArray.Length);

        spriteRenderer.sprite = spriteArray[randIndex];
    }
    public override void Start()
    {
        ChangeSprite();
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
