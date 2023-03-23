using UnityEngine;
using System.Collections.Generic;
using System;

public enum DIRECTIONS : int {
    FORWARD = 0,
    BACKWARD = 1,
    LEFT = 2,
    RIGHT = 3,
    LEN
    };

public class Animal : MonoBehaviour
{
    [SerializeField] GameObject corpse; //Объект, создающийся, когда животное умирает
    private float _step_size = 0.5f; //размер шага
    private Satiety _satiety = new Satiety(); //сытость
    private string _currentAnimation = "goat_walk_forward"; //текущая анимация
    private Animator _animator; //для проигрывания анимаций
    private float _timerStart = 0f; //стартовая точка таймера (меняется)
    private float _timeThreshold = 5f; //размер тика
    private DIRECTIONS _currentDir = DIRECTIONS.FORWARD; //Текущее направление движения

    private STATE _currentState = STATE.CHILL; //Текущее состояние. По умолчанию - бродить без дела

    private enum STATE : int 
    {
        CHILL,
        SEEK_FOR_FOOD
    };

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

    void Start()
    {
        _animator = GetComponent<Animator>();
        
    }

    void ChangePosition(Vector3 newCoords)
    {
        
        transform.Translate(newCoords*Time.deltaTime);
    }

    void ChangeAnimation(string aanimation)
    {
        if(_currentAnimation!=aanimation)
        {
            _animator.Play(aanimation);
            _currentAnimation = aanimation;
        }
    }


    void Move()
    {
         switch(_currentDir)
            {
                case DIRECTIONS.FORWARD:
                    ChangeAnimation("goat_walk_forward");
                    ChangePosition(new Vector3(0.0f,-1f*_step_size,0f));
                    break;

                case DIRECTIONS.BACKWARD:
                    ChangeAnimation("goat_walk_backward");
                    ChangePosition(new Vector3(0.0f,_step_size,0f));
                    break;

                case DIRECTIONS.LEFT:
                    ChangeAnimation("goat_walk_left");
                    ChangePosition(new Vector3(-1f*_step_size,0.0f,0f));
                    break;

                case DIRECTIONS.RIGHT:
                    ChangeAnimation("goat_walk_right");
                    ChangePosition(new Vector3(_step_size,0.0f,0f));
                    break;
            }
    }

    void Wobble(bool ready)
    {
        if(ready)
        {
            var rnd = new System.Random();
            int randomIndex = rnd.Next((int)DIRECTIONS.LEN);

            switch(randomIndex)
            {
                case (int)DIRECTIONS.FORWARD:
                    _currentDir = DIRECTIONS.FORWARD;
                    break;

                case (int)DIRECTIONS.BACKWARD:
                    _currentDir = DIRECTIONS.BACKWARD;
                    break;

                case (int)DIRECTIONS.LEFT:
                    _currentDir = DIRECTIONS.LEFT;
                    break;

                case (int)DIRECTIONS.RIGHT:
                    _currentDir = DIRECTIONS.RIGHT;
                    break;
            }
        }
    }
    void FindFood()
    {
        GameObject[] bushes;
        bushes = GameObject.FindGameObjectsWithTag("Bush");
        if(bushes != null)
        {
            foreach(GameObject entity in bushes)
            {
                Debug.Log($"Found bush with the coordinates {entity.transform.position}");
            }
        } 
    }

    void Die() 
    {
        Instantiate(corpse, transform.position , Quaternion.identity);
        Destroy(gameObject);
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

        _currentState = STATE.CHILL;
    }

    void UpdateNecessities(bool ready)
    {
        if(ready)
        {
            _satiety.Decrease(1);
        }
    }

    private void OnMouseDown() 
    {
    Debug.Log($"Это животное.\n Позиция: {transform.position} \nСытость: {_satiety.currentState}");
    }


    void Update()
    {
        bool tick = TickPassed();
        UpdateNecessities(tick);
        Wobble(tick);
        CheckNecessities();
        Move();
    }
}