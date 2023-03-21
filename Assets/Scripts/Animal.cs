using UnityEngine;
using System.Collections.Generic;
using System;
enum DIRECTIONS {FORWARD,BACKWARD,LEFT,RIGHT};

public class Animal : MonoBehaviour
{

   private Dictionary<int,DIRECTIONS> DirDictionary = new Dictionary<int,DIRECTIONS>()
    {
        {0, DIRECTIONS.FORWARD},
        {1, DIRECTIONS.BACKWARD},
        {2, DIRECTIONS.LEFT},
        {3, DIRECTIONS.RIGHT}
    };
//

    private float _step_size = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void ChangePosition(Vector3 newCoords)
    {
        
        transform.Translate(newCoords*Time.deltaTime);
    }

    void Wobble()
    {
        var rnd = new System.Random();
        int randomIndex = rnd.Next(DirDictionary.Count);

        switch(DirDictionary[randomIndex])
        {
            case DIRECTIONS.FORWARD:
            ChangePosition(new Vector3(0.0f,_step_size,0f));
            break;
            case DIRECTIONS.BACKWARD:
            ChangePosition(new Vector3(0.0f,-1f*_step_size,0f));
            break;
            case DIRECTIONS.LEFT:
            ChangePosition(new Vector3(-1f*_step_size,0.0f,0f));
            break;
            case DIRECTIONS.RIGHT:
            ChangePosition(new Vector3(_step_size,0.0f,0f));
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Wobble();
    }
}
