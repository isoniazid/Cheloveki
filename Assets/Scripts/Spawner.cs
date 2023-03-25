using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public float delay;
    [Range(0, 100f)]

    Vector3 spawnPosition;
    [SerializeField] Vector2 range;
    [SerializeField] GameObject[] currentObjects;

    IEnumerator Spawn()
    {
        for(int i =0; i<300; ++i) 
        {
            yield return new WaitForSeconds(delay);
            Vector3 pos = spawnPosition + new Vector3(Random.Range(-range.x,range.x), Random.Range(-range.y,range.y), 0f);
            foreach(GameObject obj in currentObjects) 
            {
                Instantiate(obj, pos, Quaternion.identity);
            }
        }
    }

    void Start()
    {
        spawnPosition = transform.position;
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
