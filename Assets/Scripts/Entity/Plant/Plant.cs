using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Plant : Entity
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;

    protected Period _divisionPeriod = new Period(TIME_LEN.WEEK_LEN); //Размножение раз в week

    private float _coordDispersionMax = 0.7f; //Насколько далеко может спавниться потомок растения
    private float _coordDispersionMin = 0.5f; //Насколько близко потомок растения может спавниться

    protected Vector3 GetNearPosition()
    {
        float coMultiplier = Random.Range(1.0f, 3.0f) >= 2f ? -1.0f : 1.0f; //+- на рандом
        float xPosition = coMultiplier * Random.Range(_coordDispersionMin, _coordDispersionMax);
        coMultiplier = Random.Range(1.0f, 3.0f) >= 2f ? -1.0f : 1.0f;
        float yPosition = coMultiplier * Random.Range(_coordDispersionMin, _coordDispersionMax);

        var nearPosition = transform.position + new Vector3(xPosition, yPosition, 0f);
        return nearPosition;
    }
    
    protected abstract void Divide(); //Растения размножаются.

    protected override void OnMouseDown()
    {
        messageText = "";
        messageText += "Это растение.";
        //SendText("Это растение.");

    }

}
