using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Все по пирамиде Маслоу */


public abstract class Necessity 
{
    //private const int highestPriority = 95;
    /*
    Здесь - максимальные значения приоритетов
    Чем меньше число - тем больше времени понадобится, чтобы человек начал
    искать удовлетворения своей потребности
    */
    protected const int phisiologicalPriority = 95; //Еда, Секс, Сон
    /*
    Тут без комментариев. Это должно быть уже у животных
    */
    protected const int safetyPriority = 80; // Безопасность, стабильность, комфорт
    /*
    Люди не хотят, чтобы на них нападали, хотят жить в теплом доме и чтобы были запасы еды
    */
    protected const int socialPriority = 60; // Общение, внимание, забота, поддержка
    /*
    Люди сходят с ума, если у них нет семьи и друзей и они ни с кем не общаются
    Люди хотя заниматься работой и нести пользу
    */
    protected const int rulePriority = 40; // Власть, признание, самоуважение, значимость
    /*
    Люди хотят управлять другими людьми и подчинять их себе, либо влиять на них
    */
    protected const int creativePriority = 20; //Творчество, образование
    /*
    Люди хотят творить, учиться и изобретать.
    */
    protected const int hedonismPriority = 10; //Изысканность, красота, порядок
    /*
    Люди хотят изысков, красоты кутежа
    */
    protected const int lowest_priority = -1; //БОХ
    /*
    Эти очень сильно хотят в нирвану
    */

    protected string _name;
    protected int _priority;
    // Start is called before the first frame update
    
}


public class Satiety : Necessity
{
    public int currentState = 100; //На старте максимально сыт
    public Satiety()
    {
        var rndGen = new System.Random();
        _name = "Сытость";
        _priority = rndGen.Next(safetyPriority,phisiologicalPriority);

    }

    public void Increase(int val)
    {
        if(currentState-val <100)
        {
            currentState+=val;
        }
    }

    public void Decrease(int val)
    {
        if(currentState-val>=0)
        {
            currentState-=val;
        }
    }

    public bool isSatisfied()
    {
        if(currentState<_priority)
        {
            return false;
        }
        return true;
    }

}