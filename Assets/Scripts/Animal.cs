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
    private float _step_size = 0.5f;

    private string _currentAnimation = "goat_walk_forward";
    private Animator _animator;
    private float _timerStart = 0f;
    private float _timeThreshold = 1f;
    private DIRECTIONS _currentDir = DIRECTIONS.FORWARD;
    // Start is called before the first frame update

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

    void Wobble()
    {
        if(TickPassed())
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

    // Update is called once per frame
    void Update()
    {
        Wobble();
        Move();
    }
}