using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Creature
{
    private string _firstName { get; set; }
    private string _firstNameMeaning { get; set; }
    private string _lastName { get; set; }
    private string _lastNameMeaning { get; set; }
    [SerializeField] public RuntimeAnimatorController[] genderAnimatorController;


    ////////////////////////////////////
    //Конструктор и взаимодействие с пользователем
    ///////////////////////////////////

    public override void Start()
    {
        base.Start();
        (_firstNameMeaning, _firstName) = Namer.MakeName(3);
        (_lastNameMeaning, _lastName) = Namer.MakeName(3);
        _animator.runtimeAnimatorController = gender ? genderAnimatorController[0] : genderAnimatorController[1];
    }

    protected override void OnMouseDown()
    {
        string message = "";
        message += $"Это человек: {_name}\n";
        message += $"{_firstName} {_lastName}\n";
        message += $"Пол: {GetGenderStr()}\n";
        message += $"Позиция: {transform.position}\n";
        message += $"Сытость: {_satiety.CurrentStatePercent()}%\n";
        message += $"Порог сытости: {_satiety.ThresholdPercent()}%\n";
        message += $"Секс: {_sexNecessity.CurrentStatePercent()}%\n";
        message += $"Порог для поиска партнера: {_sexNecessity.ThresholdPercent()}%\n";
        SendText(message);
    }

    protected override string GetGenderStr()
    {
        return gender ? "Мужской" : "Женский";
    }


    ////////////////////////////////////
    //Низкоуровневые методы перемещения, смерти и тд
    ///////////////////////////////////

    protected override void OnTriggerEnter2D(Collider2D collided)
    /*NB В будущем надо будет отвязать события потребностей от коллайдера*/
    {
        foreach (var collided_food in edibleFood)
        {
            if (collided.tag == collided_food.tag && _currentState == STATE.SEEK_FOR_FOOD)
            {
                Eat(collided.gameObject);
                //Debug.Log("Покушал");
            }
        }

        if (collided.tag == tag && _currentState == STATE.SEEK_FOR_PARTNER)
        {
            //Debug.Log("Вот вот начнется...");
            var partnerScript = collided.GetComponent<Human>();

            if (partnerScript.gender != gender && partnerScript._currentState == STATE.SEEK_FOR_PARTNER)
            {
                /*NB обратите внимание, что я обращаюсь к скрипту партнера, а тут могут быть проблемы, о которых
                я уже писал в FindPartners.

                Более того, я не проверяю, что это именно тот партнер, который был в getPartners, возможно,
                одна хорни коза столкнулась с другой хорни козой, которые шли к третьей, но это, я думаю, неважно, главное, что все счастливы
                И да, НИКАКОГО ХАРАССМЕНТА! Партнер тоже должен хотеть близости*/

                //Debug.Log("Ура давай ебаться!");
                _sexNecessity.Increase();
                if (gender == MALE)
                {
                    /*NB Спавн детей происходит только если объект мужского пола.
                    Связано это с тем, что коллизия происходит и  у самки, и у самца, и если этого условия не будет,
                    то сгенерируются два ребенка. Возможно, это костыль*/
                    Instantiate(child, transform.position, Quaternion.identity);
                }
            }
        }
    }

    protected override void Eat(GameObject food)
    {
        _satiety.Increase();
        if (food.tag == "Bush") Destroy(food);

        else
        {
            var eatenAnimalScript = food.GetComponent<Animal>();
            eatenAnimalScript.Die();
        }
    }


    ////////////////////////////////////
    //Высокоуровневая логика и поведение
    ///////////////////////////////////


    protected override GameObject[] FindPartners()
    {
        GameObject[] partners = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> tmp = new List<GameObject>(); //Временный список...
        foreach (var partner in partners)
        {
            var partnerScript = partner.GetComponent<Human>();//NB вот здесь могут возникнуть проблемы...
            //...Если я чето в названии скрипта, или сделаю наследование, надо будет поменять Анимал на Human или Creature
            if (partnerScript.gender != gender)//Если пол партнера не сопадает с твоим....
            {
                tmp.Add(partner);//Ура!
            }
        }
        partners = tmp.ToArray();
        if (partners.Length < 1) return null;
        return partners;
    }

}
