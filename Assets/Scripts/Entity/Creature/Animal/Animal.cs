using UnityEngine;
using System.Collections.Generic;
using System;

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
/*Там, где есть какие-то недоделанные моменты, плохо сделанные места или просто
участки кода, на которые стоит обратить внимание, комментарий начинается с NB*/
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////





public abstract class Animal : Creature
{
    protected override void Eat(GameObject food)
    {
        Debug.Log("Животное ест");
    }

    protected override void OnMouseDown()
    {
        string message = "";
        message += $"Это животное: {name}\n";
        message += $"Пол: {GetGenderStr()}\n";
        message += $"Позиция: {transform.position}\n";
        message += $"Сытость: {_satiety.CurrentStatePercent()}%\n";
        message += $"Порог сытости: {_satiety.ThresholdPercent()}%\n";
        message += $"Секс: {_sexNecessity.CurrentStatePercent()}%\n";
        message += $"Порог для поиска партнера: {_sexNecessity.ThresholdPercent()}%\n";
        SendText(message);
    }


    ////////////////////////////////////
    //Низкоуровневые методы перемещения, смерти и тд
    ///////////////////////////////////


    ////////////////////////////////////
    //Высокоуровневая логика и поведение
    ///////////////////////////////////

}