using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PRIORITY : int { PHISIOLOGICAL, SAFETY, SOCIAL, RULE, CREATIVITY, HEDONISM, GOD };
public abstract class Necessity
{
    public bool locked = false;
    public string name;
    public int _priority;
    protected int _maxLevel; //Максимальный уровень потребности. Чем выше - тем дольше она будет снижаться
    protected int _minGenLevel = 60; //минимальное значение для генератора
    protected int _maxGenLevel = 1000; //максимальное значение для генератора
    protected int _thresholdLevel = 50; //Порог, по достижении которого начнется поиск еды
    public int currentState;


    public Necessity()
    {
        var rndGen = new System.Random();
        _maxLevel = rndGen.Next(_minGenLevel, _maxGenLevel);
        currentState = _maxLevel;
    }

    public Necessity(int min, int max)
    {
        if (min < _minGenLevel) min = _minGenLevel;
        if (max > _maxGenLevel) max = _maxLevel;
        var rndGen = new System.Random();
        _maxLevel = rndGen.Next(min, max);
        currentState = _maxLevel;
    }


    public bool isSatisfied()
    /*Если потребность ниже некоего порога, то она не удовлетворена, и нужно это исправить*/
    {
        if (currentState <= _thresholdLevel)
        {
            return false;
        }
        return true;
    }

    public int CurrentStatePercent()
    {
        return currentState * 100 / _maxLevel;
    }

    public int ThresholdPercent()
    {
        return _thresholdLevel * 100 / _maxLevel;
    }

    public bool IsCritical()
    /*Если потребность на полном нуле, то уже поздно что-то делать. Только мириться с необратимыми последствиями*/
    {
        if (currentState == 0) return true;
        else return false;
    }

    public void Increase(int val)
    {
        if (!locked)
        {
            if (currentState + val < _maxLevel)
            {
                currentState += val;
            }
            else currentState = _maxLevel;
        }
    }

    public virtual void Increase()
    {
        Increase(_maxLevel);
    }

    public void Decrease()
    {
        Decrease(1);
    }

    public void Decrease(int val)
    {
        if (!locked)
        {
            if (currentState - val >= 0)
            {
                currentState -= val;
            }
            else currentState = 0;
        }
    }

}









/* public class SleepNecessity: Necessity
{
    public SleepNecessity()
    {
        var rndGen = new System.Random();
        name = "Сон";
        _priority = rndGen.Next(safetyPriority,phisiologicalPriority);
    }
} */