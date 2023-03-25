using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Plant : Entity
{

    protected override void OnMouseDown() {
        messageText = "";
        messageText += "Это растение.";
        //SendText("Это растение.");
        
    }
   
}
