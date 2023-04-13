using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Omivorous
{
#nullable enable
    public GameObject? spouse = null;

#nullable disable
    public string firstName { get; set; }
    private string _firstNameMeaning { get; set; }
    public string lastName { get; set; }
    private string _lastNameMeaning { get; set; }

#nullable enable
    private House? _home = null;

#nullable disable
    [SerializeField] public GameObject startHome;
    [SerializeField] public RuntimeAnimatorController[] genderAnimatorController;

    private HomeNecessity _homeNecessity = new HomeNecessity();

    ////////////////////////////////////
    //Конструктор и взаимодействие с пользователем
    ///////////////////////////////////

    public override void Start()
    {
        base.Start();
        name = "человек:";
        (_firstNameMeaning, firstName) = Namer.MakeName(3);
        (_lastNameMeaning, lastName) = Namer.MakeName(3);
        _animator.runtimeAnimatorController = gender ? genderAnimatorController[0] : genderAnimatorController[1];
    }

    protected override void OnMouseDown()
    {
        string message = "";
        message += $"Это {name}\n";
        message += $"{firstName} {lastName}\n";
        message += $"{GetStateStr()}\n";
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

    private string GetMarriageStr()
    {
        if (gender == FEMALE)
        {
            if (spouse == null)
            {
                return "Не замужем";
            }

            else return $"Замужем за {spouse.GetComponent<Human>().firstName} {spouse.GetComponent<Human>().lastName}";
        }

        else
        {
            if (spouse == null)
            {
                return "Не женат";
            }

            else return $"Женат на {spouse.GetComponent<Human>().firstName} {spouse.GetComponent<Human>().lastName}";

        }

    }

    private string GetStateStr()
    {
        switch(_currentState)
        {
            case STATE.SEEK_FOR_FOOD:
            return "Ищет еду";

            case STATE.SEEK_FOR_PARTNER:
            if(spouse!=null) return "Собирается исполнять супружеский долг";
            return "Ищет любви";

            case STATE.SEEK_FOR_HOMEPLACE:
            return "Ищет место для жилья";

            case STATE.CHILL:
            return "Бродит без дела";

            default:
            return "НЕИЗВЕСТНОЕ СОСТОЯНИЕ";
        }
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

    public void SetHome(House newHome)
    {
        if (_home != null)
        {
            _home.RemoveInhabitor(this);
        }
        _home = newHome;
        _home.AddInhabitor(this);
    }

    private void BuildHome()
    {
        SetHome(Instantiate(startHome, transform.position, Quaternion.identity).GetComponent<House>());
    }

    private bool CanMarry(GameObject humanToMarry)
    {
        var humanScript = humanToMarry.GetComponent<Human>();

        if (humanScript._currentState == STATE.SEEK_FOR_PARTNER
        && spouse == null
        && humanScript.gender != this.gender
        && humanScript.spouse == null) return true;

        return false;
    }

    private void Marry(GameObject humanToMarry)
    {
        if (gender == MALE)
        {
            this.spouse = humanToMarry;

            var spouseScript = this.spouse.GetComponent<Human>();
            spouseScript.spouse = this.gameObject;
            spouseScript.SetHome(this._home);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collided)
    /*NB В будущем надо будет отвязать события потребностей от коллайдера*/
    {

        if (_currentState == STATE.SEEK_FOR_FOOD) //NB yнемного оптимизировал!
        {
            foreach (var collided_food in edibleFood)
            {
                if (collided.tag == collided_food.tag)
                {
                    Eat(collided.gameObject);
                    //Debug.Log("Покушал");
                }
            }
        }

        if (collided.tag == tag && _currentState == STATE.SEEK_FOR_PARTNER) //если это человек...
        {

            if (CanMarry(collided.gameObject))
            {
                Marry(collided.gameObject);
            }

            if (collided.gameObject == spouse)
            {
                _sexNecessity.Increase();
                if (gender == MALE)
                {
                    /*NB Спавн детей происходит только если объект мужского пола.
                    Связано это с тем, что коллизия происходит и  у самки, и у самца, и если этого условия не будет,
                    то сгенерируются два ребенка. Возможно, это костыль*/
                    var kid = Instantiate(child, transform.position, Quaternion.identity);


                    var kidScript = kid.GetComponent<Human>(); //Какого-то черта потомки создаются тоже с супругами....
                    kidScript.spouse = null;
                }
            }

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
            var partnerScript = partner.GetComponent<Human>();
            if (partnerScript.gender != gender && partnerScript.spouse == null)//Если пол партнера не сопадает с твоим, и у него нет супруга..
            {
                tmp.Add(partner);//Ура!
            }
        }
        partners = tmp.ToArray();
        if (partners.Length < 1) return null;
        return partners;
    }

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
                if (spouse != null)
                {
                    Taxis(spouse);
                    //Debug.Log("Партнер есть, поэтому иду к нему");
                }

                else
                {
                    var potentialPartners = FindPartners();
                    if (potentialPartners != null) Taxis(FindNearest(potentialPartners));
                    else _currentState = STATE.CHILL;
                    //Debug.Log("Партнера нет, иду к кому есть");
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
