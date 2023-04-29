using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : Entity
{

    //public SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;

    public CORPSE_STATE currentState = CORPSE_STATE.DEAD;
    private Period _updatePeriod = new Period(TIME_LEN.DAYNIGHT_LEN);
    public override void Start()
    {
        base.Start();
        spriteRenderer.sprite = spriteArray[(int)currentState];
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case CORPSE_STATE.DEAD:
                currentState = CORPSE_STATE.ROTTEN;
                spriteRenderer.sprite = spriteArray[(int)currentState];
                break;
            case CORPSE_STATE.ROTTEN:
                currentState = CORPSE_STATE.BONES;
                spriteRenderer.sprite = spriteArray[(int)currentState];
                break;
            case CORPSE_STATE.BONES:
                break;
        }
    }

    protected override void OnMouseDown()
    {
        string message = "";
        message +=$"Это труп: {name}";
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
