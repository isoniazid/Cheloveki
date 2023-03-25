using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Все по пирамиде Маслоу */

/* //private const int highestPriority = 95;
    /*
    Здесь - максимальные значения приоритетов
    Чем меньше число - тем больше времени понадобится, чтобы человек начал
    искать удовлетворения своей потребности
    */
    //protected const int phisiologicalPriority = 95; //Еда, Секс, Сон
    /*
    Тут без комментариев. Это должно быть уже у животных
    */
    //protected const int safetyPriority = 80; // Безопасность, стабильность, комфорт
    /*
    Люди не хотят, чтобы на них нападали, хотят жить в теплом доме и чтобы были запасы еды
    */
    //protected const int socialPriority = 60; // Общение, внимание, забота, поддержка
    /*
    Люди сходят с ума, если у них нет семьи и друзей и они ни с кем не общаются
    Люди хотя заниматься работой и нести пользу
    */
    //protected const int rulePriority = 40; // Власть, признание, самоуважение, значимость
    /*
    Люди хотят управлять другими людьми и подчинять их себе, либо влиять на них
    */
    //protected const int creativePriority = 20; //Творчество, образование
    /*
    Люди хотят творить, учиться и изобретать.
    */
    //protected const int hedonismPriority = 10; //Изысканность, красота, порядок
    /*
    Люди хотят изысков, красоты кутежа
    */
    //protected const int lowest_priority = -1; //БОХ
    /*
    Эти очень сильно хотят в нирвану
    */ 

enum PRIORITY : int {PHISIOLOGICAL,SAFETY,SOCIAL,RULE,CREATIVITY,HEDONISM,GOD};
public abstract class Necessity 
{

    public string _name;
    public int _priority;
    protected  int _maxLevel; //Максимальный уровень потребности. Чем выше - тем дольше она будет снижаться
    protected  int _minGenLevel = 60; //минимальное значение для генератора
    protected  int _maxGenLevel = 1000; //максимальное значение для генератора
    protected int _thresholdLevel = 50; //Порог, по достижении которого начнется поиск еды
    public int currentState;
    
    
    public Necessity()
    {
        var rndGen = new System.Random();
        _maxLevel = rndGen.Next(_minGenLevel,_maxGenLevel);
        currentState = _maxLevel;
    }

    public Necessity(int min, int max)
    {
        if(min<_minGenLevel) min = _minGenLevel;
        if(max > _maxGenLevel) max = _maxLevel; 
        var rndGen = new System.Random();
        _maxLevel = rndGen.Next(min,max);
        currentState = _maxLevel;
    }


    public bool isSatisfied()
        /*Если потребность ниже некоего порога, то она не удовлетворена, и нужно это исправить*/
    {
        if(currentState<_thresholdLevel)
        {
            return false;
        }
        return true;
    }

    public int CurrentStatePercent()
    {
        return currentState*100/_maxLevel;
    }

    public int ThresholdPercent()
    {
        return _thresholdLevel*100/_maxLevel;
    }

    public bool IsCritical()
    /*Если потребность на полном нуле, то уже поздно что-то делать. Только мириться с необратимыми последствиями*/
    {
        if (currentState == 0) return true;
        else return false;
    }

    public void Increase(int val)
    {
        if(currentState+val <_maxLevel)
        {
            currentState+=val;
        }
        else currentState = _maxLevel;
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
        if(currentState-val>=0)
        {
            currentState-=val;
        }
        else currentState = 0;
    }
    
}


public class Satiety : Necessity
{
    public new int _priority = (int)PRIORITY.PHISIOLOGICAL;
     //На старте максимально сыт
    public Satiety() : base()
    {
        _name = "Сытость";
    }

    public Satiety(int min, int max) : base(min, max)
    {
        _name = "Сытость";

    }
}

public class SexNecessity : Necessity
{
    public new int _priority = (int)PRIORITY.PHISIOLOGICAL;

    public SexNecessity() : base()
    {
        _name = "Секс";
    }

    public SexNecessity(int min, int max) : base(min, max)
    {
        _name = "Секс";
    }
}


/* public class SleepNecessity: Necessity
{
    public SleepNecessity()
    {
        var rndGen = new System.Random();
        _name = "Сон";
        _priority = rndGen.Next(safetyPriority,phisiologicalPriority);
    }
} */