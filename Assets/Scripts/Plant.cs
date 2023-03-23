using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Plant : Entity
{

    public override void OnMouseDown() {
        Debug.Log("Это растение:");
        
    }
   
}
