using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Omivorous : Animal
{

    protected override void Eat(GameObject food)
    {
        _satiety.Increase();
        if (food.tag == "Bush") Destroy(food);
        
        else
        {
            var eatenAnimalScript = food.GetComponent<Creature>();
            eatenAnimalScript.Die();
        }
    }


    protected override void OnMouseDown()
    {
        string message = "";
        message += $"Это всеядное животное: {name}\n";
        message += $"Пол: {GetGenderStr()}\n";
        message += $"Позиция: {transform.position}\n";
        message += $"Сытость: {_satiety.CurrentStatePercent()}%\n";
        message += $"Порог сытости: {_satiety.ThresholdPercent()}%\n";
        message += $"Секс: {_sexNecessity.CurrentStatePercent()}%\n";
        message += $"Порог для поиска партнера: {_sexNecessity.ThresholdPercent()}%\n";
        SendText(message);
    }
}
