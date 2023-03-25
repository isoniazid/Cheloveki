using UnityEngine;
using System.Collections.Generic;
using System;

///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
/*Там, где есть какие-то недоделанные моменты, плохо сделанные места или просто
участки кода, на которые стоит обратить внимание, комментарий начинается с NB*/
///////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////

public enum DIRECTIONS : int {
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



public class Animal : MonoBehaviour
{
    const bool MALE = true;
    const bool FEMALE = false;
    //6,5rad ~= 360deg
    public bool gender;

    [SerializeField] GameObject child;
    [SerializeField] GameObject corpse; //Объект, создающийся, когда животное умирает
    private Vector3 _step_size = new Vector3(0.5f,0.5f,0f); //размер шага
    private Satiety _satiety = new Satiety(); //сытость
    private SexNecessity _sexNecessity = new SexNecessity();
    private string _currentAnimation = "goat_walk_forward"; //текущая анимация
    private Animator _animator; //для проигрывания анимаций
    private float _timerStart; //стартовая точка таймера (меняется)
    private float _timeThreshold = 1f; //размер тика
    //private DIRECTIONS _currentDir = DIRECTIONS.FORWARD; //Текущее направление движения
    private Vector3 _currentStep = new Vector3(0.5f,0.5f,0f);
    public STATE _currentState = STATE.CHILL; //Текущее состояние. По умолчанию - бродить без дела

    /*NB я еще пока учусь разбираться с делегатами, но думаю, что запихнуть их сюда будет в тему*/
    delegate void Send(string text);
    Send SendText;

    public enum STATE : int 
    {
        CHILL,
        SEEK_FOR_FOOD,
        SEEK_FOR_PARTNER
    };


////////////////////////////////////
//Конструктор и взаимодействие с пользователем
///////////////////////////////////

    void Start()
    {
        SendText = GameObject.FindGameObjectWithTag("MainUI").GetComponent<InfoText>().ChangeText;
        System.Random rnd = new System.Random();
         gender = (rnd.Next(2)==0);
        _timerStart = Time.time;
        _animator = GetComponent<Animator>();
        
    }

    private void OnMouseDown() 
    {
        string message = "";
        message+="Это животное.\n";
        message+=$"Пол: {GetGenderStr()}\n";
        message+=$"Позиция: {transform.position}\n";
        message+=$"Сытость: {_satiety.CurrentStatePercent()}%\n";
        message+=$"Порог сытости: {_satiety.ThresholdPercent()}%\n";
        message+=$"Секс: {_sexNecessity.CurrentStatePercent()}%\n";
        message+=$"Порог для поиска партнера: {_sexNecessity.ThresholdPercent()}%\n";
        SendText(message);

    /* Debug.Log($"Это животное.");
    Debug.Log($"Позиция: {transform.position}");
    Debug.Log($"Сытость: {_satiety.CurrentStatePercent()}%");
    Debug.Log($"Порог сытости: {_satiety.ThresholdPercent()}%");
    Debug.Log($"Секс: {_sexNecessity.CurrentStatePercent()}%");
    Debug.Log($"Порог для поиска партнера: {_sexNecessity.ThresholdPercent()}");
    Debug.Log($"Пол: {GetGenderStr()}"); */
    }

    private string GetGenderStr()
    {
        return gender ? "Самец" : "Самка";
    }

////////////////////////////////////
//Низкоуровневые методы перемещения, смерти и тд
///////////////////////////////////

    private void OnTriggerEnter2D(Collider2D collided) 
    /*NB В будущем надо будет отвязать события потребностей от коллайдера*/
    {
        if(collided.tag == "Bush" && _currentState == STATE.SEEK_FOR_FOOD)
        {
            _satiety.Increase();
            Destroy(collided.gameObject);
            //Debug.Log("Покушал");
        }

        if(collided.tag == tag && _currentState == STATE.SEEK_FOR_PARTNER)
        {
            //Debug.Log("Вот вот начнется...");
            var partnerScript = collided.GetComponent<Animal>();

            if(partnerScript.gender != gender && partnerScript._currentState == STATE.SEEK_FOR_PARTNER) 
            {
            /*NB обратите внимание, что я обращаюсь к скрипту партнера, а тут могут быть проблемы, о которых
            я уже писал в FindPartners.
            
            Более того, я не проверяю, что это именно тот партнер, который был в getPartners, возможно,
            одна хорни коза столкнулась с другой хорни козой, которые шли к третьей, но это, я думаю, неважно, главное, что все счастливы
            И да, НИКАКОГО ХАРАССМЕНТА! Партнер тоже должен хотеть близости*/
            
                //Debug.Log("Ура давай ебаться!");
                _sexNecessity.Increase();
                if(gender == MALE)
                {
                /*NB Спавн детей происходит только если объект мужского пола.
                Связано это с тем, что коллизия происходит и  у самки, и у самца, и если этого условия не будет,
                то сгенерируются два ребенка. Возможно, это костыль*/
                Instantiate(child, transform.position,Quaternion.identity);
                }
            }
        }  
    }

    Vector3 FindNearest(GameObject [] array)
    {
        GameObject nearest = array[0];
        var nearestDistance = Vector3.Distance(nearest.transform.position, transform.position);
        foreach(GameObject currentObj in array)
        {
            var currentDistance = Vector3.Distance(currentObj.transform.position, transform.position);
            if(currentDistance<nearestDistance)
            {
                nearestDistance = currentDistance;
                nearest = currentObj;
            }
        }
        //Debug.Log($"Nearest bush {nearest.transform.position}");
        return nearest.transform.position;
    }
    void Taxis(Vector3 taxisTarget)
    /*Вектор шага поворачивается в сторону объекта, к которому надо идти...*/
    {
        if(taxisTarget == null) return;
        //6.5f - Чуть больше 360 градусов в радианах.
        // 0f - это что размер вектора нельзя менять.
        _currentStep = Vector3.RotateTowards(_currentStep,taxisTarget-transform.position,7f,0f);
    }

    void Taxis(GameObject taxisTarget)
    {
        if(taxisTarget == null) return;
        _currentStep = Vector3.RotateTowards(_currentStep,taxisTarget.transform.position-transform.position,7f,0f); 
    }

    void ChangeAnimation(string aanimation)
    {
        if(_currentAnimation!=aanimation)
        {
            _animator.Play(aanimation);
            _currentAnimation = aanimation;
        }
    }

    void ChangePosition(Vector3 newCoords)
    {
        
        transform.Translate(newCoords*Time.deltaTime);
    }

    DIRECTIONS HandleDirections(Vector3 vectorToHandle)
    /*Здесь есть магические числа*/
    {
        if(Math.Abs(vectorToHandle.x) < 0.5f ) //Сначала вырожденные ситуации, потом 
        //Варианты - либо вверх, либо вниз
        {
            if(vectorToHandle.y >=0f) return DIRECTIONS.BACKWARD;
            else return DIRECTIONS.FORWARD;

        }

        else if(Math.Abs(vectorToHandle.y) < 0.5f)
        {//Либо влево, либо вправо

            if(vectorToHandle.x >=0f) return DIRECTIONS.RIGHT;
            else return DIRECTIONS.LEFT;

        }

        else //если оба вектора ненулевые...
        {
            if(vectorToHandle.x > 0f)
            {
                if(vectorToHandle.y >0f) return DIRECTIONS.RIGHT_BACKWARD;
                else return DIRECTIONS.RIGHT_FORWARD;
            }

            else
            {
                if(vectorToHandle.y >0f) return DIRECTIONS.LEFT_BACKWARD;
                else return DIRECTIONS.LEFT_FORWARD;

            }

        }



    }

    void Move()
    {
                switch(HandleDirections(_currentStep))
                {
                    /*пока нет анимаций для некоторых направлений!*/
                    case DIRECTIONS.FORWARD:
                    ChangeAnimation("goat_walk_forward");
                    break;

                    case DIRECTIONS.LEFT_FORWARD:
                    ChangeAnimation("goat_walk_left");
                    break;

                    case DIRECTIONS.RIGHT_FORWARD:
                    ChangeAnimation("goat_walk_right");
                    break;

                    case DIRECTIONS.BACKWARD:
                    ChangeAnimation("goat_walk_backward");
                    break;

                    case DIRECTIONS.RIGHT_BACKWARD:
                    ChangeAnimation("goat_walk_right"); 
                    break;

                    case DIRECTIONS.LEFT_BACKWARD:
                    ChangeAnimation("goat_walk_left");
                    break;

                    case DIRECTIONS.LEFT:
                    ChangeAnimation("goat_walk_left");
                    break;

                    case DIRECTIONS.RIGHT:
                    ChangeAnimation("goat_walk_right");
                    break;
                }

                ChangePosition(_currentStep);
    }

    void Die() 
    {
        Instantiate(corpse, transform.position , Quaternion.identity);
        Destroy(gameObject);
    }

    private bool TickPassed()
    {
        var currentTime = Time.time;
        if(currentTime - _timerStart >= _timeThreshold)
        {
            _timerStart = currentTime;
            return true;
        }
        return false;
    }

////////////////////////////////////
//Высокоуровневая логика и поведение
///////////////////////////////////


    void Wobble()
    {
            float randomX = UnityEngine.Random.Range(-10f,10f);
            float randomY = UnityEngine.Random.Range(-10f,10f);
            Vector3 randomPoint = transform.position + new Vector3(randomX,randomY, 0f);
            Taxis(randomPoint);
    }
    GameObject[] FindFood()
    {
        GameObject[] bushes = GameObject.FindGameObjectsWithTag("Bush");
        if (bushes.Length<1) return null;
        return bushes;
    }
    
    GameObject[] FindPartners()
    {
        GameObject[] partners = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> tmp = new List<GameObject>(); //Временный список...
        foreach(var partner in partners)
        {
            var partnerScript = partner.GetComponent<Animal>();//NB вот здесь могут возникнуть проблемы...
            //...Если я чето в названии скрипта, или сделаю наследование, надо будет поменять Анимал на Human или Creature
            if(partnerScript.gender != gender)//Если пол партнера не сопадает с твоим....
            { 
                tmp.Add(partner);//Ура!
            }
        }
        partners = tmp.ToArray();
        if(partners.Length<1) return null;
        return partners;
    }
    
    void CheckNecessities()
    /*Проверка потребностей. Если есть неудовлетворенные потребности, состояние животного изменится, и оно начнет их удовлетворять*/
    {
        if(!_satiety.isSatisfied())
        {
            if(!_satiety.IsCritical())
            {
            _currentState = STATE.SEEK_FOR_FOOD;
            return;
            }
            else
            {
                Die();
            }
        }

        if(!_sexNecessity.isSatisfied())
        {
            if(!_satiety.IsCritical())
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
    
    void UpdateNecessities(bool ready)
    {
        if(ready)
        {
            _satiety.Decrease();
            _sexNecessity.Decrease();
        }
    }


    void HandleState(bool ready)
    { /*В зависимости от состояния изменяется логика, по которой существо куда-то идет*/
        if(!ready) return;

        switch(_currentState)
        {
            case STATE.CHILL:
            Wobble();
            break;

            case STATE.SEEK_FOR_FOOD:
            //Debug.Log("I am hungry!");
            var food = FindFood();
            if(food != null) Taxis(FindNearest(food));
            else _currentState = STATE.CHILL;
            break;

            case STATE.SEEK_FOR_PARTNER:
            var partners = FindPartners();
            if(partners != null) Taxis(FindNearest(partners));
            break;
        }

    }

    void Update()
    {
        bool tick = TickPassed();
        CheckNecessities();
        UpdateNecessities(tick);
        HandleState(tick);
        Move();
    }
}