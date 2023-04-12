using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Omivorous
{
    public string firstName { get; set; }
    private string _firstNameMeaning { get; set; }
    public string lastName { get; set; }
    private string _lastNameMeaning { get; set; }
    [SerializeField] public GameObject startHome;
    [SerializeField] public RuntimeAnimatorController[] genderAnimatorController;

    private HomeNecessity _homeNecessity = new HomeNecessity();

    ////////////////////////////////////
    //Конструктор и взаимодействие с пользователем
    ///////////////////////////////////

    public override void Start()
    {
        base.Start();
        (_firstNameMeaning, firstName) = Namer.MakeName(3);
        (_lastNameMeaning, lastName) = Namer.MakeName(3);
        _animator.runtimeAnimatorController = gender ? genderAnimatorController[0] : genderAnimatorController[1];
    }

    protected override void OnMouseDown()
    {
        string message = "";
        message += $"Это человек: {name}\n";
        message += $"{firstName} {lastName}\n";
        message += $"Пол: {GetGenderStr()}\n";
        message += $"Позиция: {transform.position}\n";
        message += $"Сытость: {_satiety.CurrentStatePercent()}%\n";
        message += $"Порог сытости: {_satiety.ThresholdPercent()}%\n";
        message += $"Секс: {_sexNecessity.CurrentStatePercent()}%\n";
        message += $"Порог для поиска партнера: {_sexNecessity.ThresholdPercent()}%\n";
        message += $"Дом: {_homeNecessity.CurrentStatePercent()}%\n";
        message += $"Порог строительства дома: {_homeNecessity.ThresholdPercent()}%\n";
        SendText(message);
    }

    protected override string GetGenderStr()
    {
        return gender ? "Мужской" : "Женский";
    }


    ////////////////////////////////////
    //Низкоуровневые методы перемещения, смерти и тд
    ///////////////////////////////////

    protected GameObject[] FindHomes()
    {
        GameObject[] homes = GameObject.FindGameObjectsWithTag("House"); //NB когда появятся другие постройки, переделай
        if (homes.Length < 1) return null;
        return homes;
    }

    private void BuildHome()
    {
        Instantiate(startHome, transform.position, Quaternion.identity);
    }

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


    ////////////////////////////////////
    //Высокоуровневая логика и поведение
    ///////////////////////////////////

    protected override void CheckNecessities()
    /*Проверка потребностей. Если есть неудовлетворенные потребности, состояние животного изменится, и оно начнет их удовлетворять*/
    {
        if (!_satiety.isSatisfied())
        {
            if (!_satiety.IsCritical())
            {
                _currentState = STATE.SEEK_FOR_FOOD;
                return;
            }
            else
            {
                Die();
            }
        }

        if (!_homeNecessity.isSatisfied())
        {
            _currentState = STATE.SEEK_FOR_HOMEPLACE;
            return;
        }

        if (!_sexNecessity.isSatisfied())
        {
            if (!_satiety.IsCritical())
            {
                _currentState = STATE.SEEK_FOR_PARTNER;
                return;
            }
            else
            {
                /*NB ЕЩЕ не допилил, что делать, если долго не сексил*/
                Debug.Log("Forever alone...");
            }
        }

        _currentState = STATE.CHILL;
    }

    protected override void UpdateNecessities(bool ready)
    {
        base.UpdateNecessities(ready);

        if (ready) _homeNecessity.Decrease();
    }

    protected override void HandleState(bool ready)
    { /*В зависимости от состояния изменяется логика, по которой существо куда-то идет*/
        if (!ready) return;

        switch (_currentState)
        {
            case STATE.CHILL:
                Wobble();
                break;

            case STATE.SEEK_FOR_FOOD:
                //Debug.Log("I am hungry!");
                var food = FindFood();
                if (food != null) Taxis(FindNearest(food));
                else _currentState = STATE.CHILL;
                break;

            case STATE.SEEK_FOR_PARTNER:
                var partners = FindPartners();
                if (partners != null) Taxis(FindNearest(partners));
                break;

            case STATE.SEEK_FOR_HOMEPLACE:
                var homes = FindHomes();
                if (homes != null)
                {
                    var nearest = FindNearest(homes);
                    if (Vector3.Distance(transform.position, nearest) < 1.5f && Vector3.Distance(transform.position, nearest) > 1f) //NB Магические числа!
                    {
                        BuildHome();
                        _homeNecessity.Increase();
                        _homeNecessity.locked = true;
                    }

                    else if (Vector3.Distance(transform.position, nearest) < 1f) Antitaxis(nearest);

                    else Taxis(nearest);
                }
                else
                {
                    BuildHome();
                }

                break;
        }

    }
}
