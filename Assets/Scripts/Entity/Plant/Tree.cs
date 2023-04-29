using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Plant
{
    [SerializeField] GameObject FruitType;

    protected Period _fruitGrowthPeriod = new Period(TIME_LEN.DAYNIGHT_LEN);
    

    protected override void Divide()
    {
        Instantiate(this, GetNearPosition(), Quaternion.identity);
        //Debug.Log("Растение Размножилось");
    }

    protected string CountFruitsStr()
    {
        int counter = 0;
        foreach(var item in Inventory)
        {
            if(item.tag == FruitType.tag) counter++;
        }
        return $"Плодов: {counter}";
    }

    protected void GrowFruits()
    {
        for (int i = 0; i < Random.Range(1, 50); i++) Inventory.Add(FruitType); //NB магические числа!
    } 

    public override void Start()
    {
        base.Start();
        _coordDispersionMin = 0.7f;
        _coordDispersionMax = 1.5f;
        GrowFruits();
    }

    protected override void OnMouseDown()
    {
        messageText = "";
        messageText += $"Это дерево: {name}\n";
        messageText += $"Положение: {transform.position}\n";
        messageText+= $"{CountFruitsStr()}\n";
        SendText(messageText);
    }

    // Update is called once per frame
    public override void Update()
    {
        bool tick = TickPassed();

        if (_divisionPeriod.isPassed(tick))
        {
            Divide();
        }

        if(_fruitGrowthPeriod.isPassed(tick))
        {
            GrowFruits();
        }
    }
}