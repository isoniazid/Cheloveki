using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Plant : MonoBehaviour
{
    private string _name;
    public GameObject body;

    public Plant()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Vector3 _position
    {
        set {this.transform.Translate(value.x,value.y,0f);}
        get { return this._position;}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
