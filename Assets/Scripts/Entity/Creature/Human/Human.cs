using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Omivorous
{
#nullable enable
    public Human? spouse = null;

#nullable disable
    public string firstName { get; set; }
    private string _firstNameMeaning { get; set; }
    public string lastName { get; set; }
    private string _lastNameMeaning { get; set; }

    private House _home;
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
        message += $"Дом: {(_homeNecessity.CurrentStatePercent() == 100 ? "Есть" : "Нет")}\n";
        message += $"{GetMarriageStr()}\n";
        SendText(message);
    }

    protected override string GetGenderStr()
    {
        return gender ? "Мужской" : "Женский";
    }


    ////////////////////////////////////    
    //Низкоуровневые методы перемещения, смерти и тд
    ///////////////////////////////////

    public Vector3 GetPosition() //NB Если напрямую обращаться к spouse, то spouse.transfrom.position не пашет. пришлось городить костыль
    {
        Vector3 pos = transform.position;
        return pos;
    }

        protected void Taxis(Human taxisTarget)
    {
        if (taxisTarget == null) return;
        _currentStep = (Vector2)Vector3.RotateTowards(_currentStep, taxisTarget.transform.position - transform.position, 7f, 0f);
        _currentStep = _currentStep.normalized * 0.7f;
        //Debug.DrawLine(transform.position, transform.position+_currentStep,Color.white,1f);
    }

    protected Vector3 FindNearest(Human[] array)
    {
        Human nearest = array[0];
        var nearestDistance = Vector3.Distance(nearest.transform.position, transform.position);
        foreach (Human currentObj in array)
        {
            var currentDistance = Vector3.Distance(currentObj.transform.position, transform.position);
            if (currentDistance < nearestDistance)
            {
                nearestDistance = currentDistance;
                nearest = currentObj;
            }
        }
        return nearest.transform.position;
    }

    protected GameObject[] FindHomes()
    {
        GameObject[] homes = GameObject.FindGameObjectsWithTag("House"); //NB когда появятся другие постройки, переделай
        if (homes.Length < 1) return null;
        return homes;
    }

    private void BuildHome()
    {
        _home = Instantiate(startHome, transform.position, Quaternion.identity).GetComponent<House>();
        _home.inhabitors.Add(this);
    }

    private void Marry(Human humanToMarry)
    {
        spouse = humanToMarry;
    }

    private string GetMarriageStr()
    {
        if (gender == FEMALE)
        {
            if (spouse == null)
            {
                return "Не замужем";
            }

            else return $"Замужем за {spouse.firstName} {spouse.lastName}";
        }

        else
        {
            if (spouse == null)
            {
                return "Не женат";
            }

            else return $"Женат на {spouse.firstName} {spouse.lastName}";

        }

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

            var partnerScript = collided.GetComponent<Human>();



            if (spouse == null && partnerScript._currentState == STATE.SEEK_FOR_PARTNER && partnerScript.gender != gender)
            {
                Marry(partnerScript);
            }

            else if (spouse != partnerScript) return;

            else
            {
                if (partnerScript.gender != gender && partnerScript._currentState == STATE.SEEK_FOR_PARTNER)
                {
                    _sexNecessity.Increase();
                    if (gender == MALE) Instantiate(child, transform.position, Quaternion.identity); //NB smotri class Creature
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
                if (spouse != null) Taxis(spouse.GetPosition());

                else
                {
                    var partners = FindPartners();
                    if (partners != null) Taxis(FindNearest(partners));
                }
                break;

            case STATE.SEEK_FOR_HOMEPLACE:
                var homes = FindHomes();
                if (homes != null)
                {
                    var nearest = FindNearest(homes);
                    if (Vector3.Distance(transform.position, nearest) < 1f && Vector3.Distance(transform.position, nearest) > 0.7f) //NB Магические числа!
                    {
                        BuildHome();
                        _homeNecessity.Increase();
                        _homeNecessity.locked = true;
                    }

                    else if (Vector3.Distance(transform.position, nearest) < 0.7f) Antitaxis(nearest);

                    else Taxis(nearest);
                }
                else
                {
                    BuildHome();
                    _homeNecessity.Increase();
                    _homeNecessity.locked = true;
                }

                break;
        }

    }
}
