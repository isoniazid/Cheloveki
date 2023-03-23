using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Entity : MonoBehaviour
{

    protected float _timerStart = 0f;
    protected float _timeThreshold = 5f;
    public string _name;
    [SerializeField]
    

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

    public virtual void OnMouseDown()
    {
        Debug.Log("Это сущность");
    }

    
    // Start is called before the first frame update
    public abstract void Start();


    // Update is called once per frame
    public abstract void Update();
   
}
