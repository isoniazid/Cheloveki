using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Bush : Plant
{
    
    protected override void Divide()
    {
        Instantiate(this, GetNearPosition(),Quaternion.identity);
        //Debug.Log("Растение Размножилось");
    }

    private void ChangeSprite()
    {
        var rndGen = new System.Random();
        var randIndex = rndGen.Next(spriteArray.Length);

        spriteRenderer.sprite = spriteArray[randIndex];
    }
    public override void Start()
    {
        base.Start();
        ChangeSprite();
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();

        messageText+="Куст\n";
        messageText+=$"Положение: {transform.position}\n";
        SendText(messageText);
    }

    // Update is called once per frame
    public override void Update()
    {
        if(_divisionPeriod.isPassed((TickPassed())))
        {
            Divide();
        }
    }
}
