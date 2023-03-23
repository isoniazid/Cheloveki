using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : Entity
{

    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;
    

    enum STATE {DEAD, ROTTEN, BONES};
    private STATE _currentState = STATE.DEAD;
    // Start is called before the first frame update

    public override void Start() 
    {
        spriteRenderer.sprite = spriteArray[(int)_currentState];
        _timeThreshold = 10f;
    }

    private void HandleState()
    {
        switch(_currentState)
        {
            case STATE.DEAD:
            _currentState = STATE.ROTTEN;
            spriteRenderer.sprite = spriteArray[(int)_currentState];
            break;
            case STATE.ROTTEN:
            _currentState = STATE.BONES;
            spriteRenderer.sprite = spriteArray[(int)_currentState];
            break;
            case STATE.BONES:
            break;
        }
    }

    public override void OnMouseDown()
    {
        Debug.Log($"Hello There\n I'm a {_name}");
    }

    // Update is called once per frame
    public override void Update()
    {
        bool tick = TickPassed();
        if(tick)
        {
            HandleState();
        }
    }
}
