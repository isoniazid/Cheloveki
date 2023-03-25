using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Entity : MonoBehaviour
{

    protected float _timerStart;
    protected float _timeThreshold = 5f;
    public string _name;
    [SerializeField]

    /*NB я еще пока учусь разбираться с делегатами, но думаю, что запихнуть их сюда будет в тему*/
    protected delegate void Send(string text);
    protected Send SendText;
    
    protected string messageText;

     protected bool TickPassed()
    {
        var currentTime = Time.time;
        if(currentTime - _timerStart >= _timeThreshold)
        {
            _timerStart = currentTime;
            return true;
        }
        return false;
    }

    protected virtual void OnMouseDown()
    {
        messageText = "";
        messageText += "Это сущность";
        SendText(messageText);
        //Debug.Log("Это сущность");

        
    }


    
    // Start is called before the first frame update
    public virtual void Start()
    {
        SendText = GameObject.FindGameObjectWithTag("MainUI").GetComponent<InfoText>().ChangeText;
        _timerStart = Time.time;
    }


    // Update is called once per frame
    public abstract void Update();
   
}
