using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : Entity
{

    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;


    enum STATE { DEAD, ROTTEN, BONES };
    private STATE _currentState = STATE.DEAD;
    private Period _updatePeriod = new Period(TIME_LEN.DAYNIGHT_LEN);
    public override void Start()
    {
        base.Start();
        spriteRenderer.sprite = spriteArray[(int)_currentState];
    }

    private void HandleState()
    {
        switch (_currentState)
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

    protected override void OnMouseDown()
    {
        string message = "";
        message +=$"Это труп: {_name}";
        SendText(message);
    }

    // Update is called once per frame
    public override void Update()
    {
        if (_updatePeriod.isPassed(TickPassed()))
        {
            HandleState();
        }
    }
}
