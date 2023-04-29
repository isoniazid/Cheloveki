using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Entity : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Color startColor;
    [SerializeField] public List<GameObject> Inventory = new List<GameObject>();
    protected float _timerStart;
    protected float _timeThreshold = TIME_LEN.DEFAULT_TICK;
    public new string name;
    [SerializeField]

    /*NB я еще пока учусь разбираться с делегатами, но думаю, что запихнуть их сюда будет в тему*/
    protected delegate void Send(string text);
    protected Send SendText;

    protected string messageText;

    protected bool TickPassed()
    {
        var currentTime = Time.time;
        if (currentTime - _timerStart >= _timeThreshold)
        {
            _timerStart = currentTime;
            return true;
        }
        return false;
    }

    protected virtual void OnMouseDown()
    {
        messageText = "";
        messageText += $"Это сущность: {name}";
        SendText(messageText);
        //Debug.Log("Это сущность");


    }

    protected void Toggle()
    { //NB возможно, я найду более хороший способ скрыть объект, чтобы отъедалось меньше ресурсов
       spriteRenderer.color = new Color(0,0,0,0);
    }

    protected void Untoggle()
    {
        spriteRenderer.color = startColor;
    }


    // Start is called before the first frame update
    public virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        SendText = GameObject.FindGameObjectWithTag("MainUI").GetComponent<InfoText>().ChangeText;
        _timerStart = Time.time;
    }


    // Update is called once per frame
    public abstract void Update();

}
