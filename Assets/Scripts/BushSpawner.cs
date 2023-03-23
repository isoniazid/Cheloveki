using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushSpawner : MonoBehaviour
{

    public Transform spawnPosition;
    [SerializeField] Vector2 range;
    [SerializeField] GameObject currentObject;

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(0.1f);
        Vector3 pos = spawnPosition.position + new Vector3(Random.Range(-range.x,range.x), Random.Range(-range.y,range.y), 0f);
        Instantiate(currentObject, pos, Quaternion.identity);
        
        Start();
    }

    void Start()
    {
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
