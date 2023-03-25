using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Сущности меняются со временем, это время может быть разным,
причем не только для разных сущностей, но и для их разных элементов.
Период - это, по-сути, произвольно задаваемый "мега-тик", состоящий из N тиков
*/

public class Period
{
    private int _len;
    private int currentNumberOfTicks = 0;

    public Period(int len)
    {
        _len = len;
    }

    public bool isPassed(bool tick)
    {
        if(tick) currentNumberOfTicks++;
        if(currentNumberOfTicks >= _len)
        {
         currentNumberOfTicks = 0;
         return true;   
        }
        else return false;
    }


    }
