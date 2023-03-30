using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
Это  класс существ. Все существа:
1) Едят и размножаются
2) Двигаются 
3) способны к таксису
4) имеют потомков и трупы
5) Взаимодействуют с другими существами
6) имеют пол
*/

public enum DIRECTIONS : int
{
    FORWARD,
    LEFT_FORWARD,
    RIGHT_FORWARD,
    BACKWARD,
    RIGHT_BACKWARD,
    LEFT_BACKWARD,
    LEFT,
    RIGHT,
    LEN
};

abstract public class Creature : Entity
{
    const bool MALE = true;
    const bool FEMALE = false;
    //6,5rad ~= 360deg
    public bool gender;
    [SerializeField] GameObject child; //Объект, создающийся после размножения
    [SerializeField] GameObject corpse; //Объект, создающийся, когда животное умирает
    
    [SerializeField] protected List<GameObject> edibleFood = new List<GameObject>(); //Список того, что можно есть
    protected Vector3 _step_size = new Vector3(0.5f, 0.5f, 0f); //размер шага
    protected Satiety _satiety = new Satiety(); //сытость
    protected SexNecessity _sexNecessity = new SexNecessity();
    protected string _currentAnimation = "walk_forward"; //текущая анимация
    protected Animator _animator; //для проигрывания анимаций
    protected Vector3 _currentStep = new Vector3(0.5f, 0.5f, 0f);
    public STATE _currentState = STATE.CHILL; //Текущее состояние. По умолчанию - бродить без дела

    public enum STATE : int
    {
        CHILL,
        SEEK_FOR_FOOD,
        SEEK_FOR_PARTNER
    };

    ////////////////////////////////////
    //Конструктор и взаимодействие с пользователем
    ///////////////////////////////////

    public override void Start()
    {
        base.Start();
        //SendText = GameObject.FindGameObjectWithTag("MainUI").GetComponent<InfoText>().ChangeText;
        System.Random rnd = new System.Random();
        gender = (rnd.Next(2) == 0);
        //_timerStart = Time.time;
        _animator = GetComponent<Animator>();

    }

    protected override void OnMouseDown()
    {
        messageText = "";
        messageText += "Это существо";
        SendText(messageText);
        //Debug.Log("Это сущность");
    }


    protected virtual string GetGenderStr()
    {
        return gender ? "Самец" : "Самка";
    }

    ////////////////////////////////////
    //Низкоуровневые методы перемещения, смерти и тд
    ///////////////////////////////////

    protected virtual void Eat(GameObject food)
    {}
    //{
            //_satiety.Increase();
            //Destroy(collided.gameObject);
    //}

    protected virtual void OnTriggerEnter2D(Collider2D collided)
    /*NB В будущем надо будет отвязать события потребностей от коллайдера*/
    {
        foreach(var collided_food in edibleFood)
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
            var partnerScript = collided.GetComponent<Animal>();

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

    protected Vector3 FindNearest(GameObject[] array)
    {
        GameObject nearest = array[0];
        var nearestDistance = Vector3.Distance(nearest.transform.position, transform.position);
        foreach (GameObject currentObj in array)
        {
            var currentDistance = Vector3.Distance(currentObj.transform.position, transform.position);
            if (currentDistance < nearestDistance)
            {
                nearestDistance = currentDistance;
                nearest = currentObj;
            }
        }
        //Debug.Log($"Nearest bush {nearest.transform.position}");
        return nearest.transform.position;
    }
    protected void Taxis(Vector3 taxisTarget)
    /*Вектор шага поворачивается в сторону объекта, к которому надо идти...*/
    {
        if (taxisTarget == null) return;
        //6.5f - Чуть больше 360 градусов в радианах.
        // 0f - это что размер вектора нельзя менять.
        _currentStep = Vector3.RotateTowards(_currentStep, taxisTarget - transform.position, 7f, 0f);
    }

    protected void Taxis(GameObject taxisTarget)
    {
        if (taxisTarget == null) return;
        _currentStep = Vector3.RotateTowards(_currentStep, taxisTarget.transform.position - transform.position, 7f, 0f);
    }

    protected void ChangeAnimation(string aanimation)
    {
        if (_currentAnimation != aanimation)
        {
            _animator.Play(aanimation);
            _currentAnimation = aanimation;
        }
    }

    protected void ChangePosition(Vector3 newCoords)
    {

        transform.Translate(newCoords * Time.deltaTime);
    }

    protected DIRECTIONS HandleDirections(Vector3 vectorToHandle)
    /*Здесь есть магические числа*/
    {
        if (Math.Abs(vectorToHandle.x) < 0.5f) //Сначала вырожденные ситуации, потом 
        //Варианты - либо вверх, либо вниз
        {
            if (vectorToHandle.y >= 0f) return DIRECTIONS.BACKWARD;
            else return DIRECTIONS.FORWARD;

        }

        else if (Math.Abs(vectorToHandle.y) < 0.5f)
        {//Либо влево, либо вправо

            if (vectorToHandle.x >= 0f) return DIRECTIONS.RIGHT;
            else return DIRECTIONS.LEFT;

        }

        else //если оба вектора ненулевые...
        {
            if (vectorToHandle.x > 0f)
            {
                if (vectorToHandle.y > 0f) return DIRECTIONS.RIGHT_BACKWARD;
                else return DIRECTIONS.RIGHT_FORWARD;
            }

            else
            {
                if (vectorToHandle.y > 0f) return DIRECTIONS.LEFT_BACKWARD;
                else return DIRECTIONS.LEFT_FORWARD;

            }

        }



    }

    protected void Move()
    {
        switch (HandleDirections(_currentStep))
        {
            /*пока нет анимаций для некоторых направлений!*/
            case DIRECTIONS.FORWARD:
                ChangeAnimation("walk_forward");
                break;

            case DIRECTIONS.LEFT_FORWARD:
                ChangeAnimation("walk_left");
                break;

            case DIRECTIONS.RIGHT_FORWARD:
                ChangeAnimation("walk_right");
                break;

            case DIRECTIONS.BACKWARD:
                ChangeAnimation("walk_backward");
                break;

            case DIRECTIONS.RIGHT_BACKWARD:
                ChangeAnimation("walk_right");
                break;

            case DIRECTIONS.LEFT_BACKWARD:
                ChangeAnimation("walk_left");
                break;

            case DIRECTIONS.LEFT:
                ChangeAnimation("walk_left");
                break;

            case DIRECTIONS.RIGHT:
                ChangeAnimation("walk_right");
                break;
        }

        ChangePosition(_currentStep);
    }

    public void Die()
    {
        Instantiate(corpse, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    ////////////////////////////////////
    //Высокоуровневая логика и поведение
    ///////////////////////////////////


    protected void Wobble()
    {
        float randomX = UnityEngine.Random.Range(-10f, 10f);
        float randomY = UnityEngine.Random.Range(-10f, 10f);
        Vector3 randomPoint = transform.position + new Vector3(randomX, randomY, 0f);
        Taxis(randomPoint);
    }
    protected GameObject[] FindFood()
    {
        List<GameObject> foodList = new List<GameObject>();
        foreach(GameObject food_type in edibleFood)
        {
        GameObject[] food = GameObject.FindGameObjectsWithTag(food_type.tag);
        if (!(food.Length < 1))
        {
        foodList.AddRange(food);
        }
        }
        if(foodList.Count < 1) return null;
        else return foodList.ToArray(); 
        
    }

    protected GameObject[] FindPartners()
    {
        GameObject[] partners = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> tmp = new List<GameObject>(); //Временный список...
        foreach (var partner in partners)
        {
            var partnerScript = partner.GetComponent<Animal>();//NB вот здесь могут возникнуть проблемы...
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

    protected virtual void CheckNecessities()
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

    protected virtual void UpdateNecessities(bool ready)
    {
        if (ready)
        {
            _satiety.Decrease();
            _sexNecessity.Decrease();
        }
    }


    protected virtual void HandleState(bool ready)
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
        }

    }

    public override void Update()
    {
        bool tick = TickPassed();
        CheckNecessities();
        UpdateNecessities(tick);
        HandleState(tick);
        Move();
    }

}
