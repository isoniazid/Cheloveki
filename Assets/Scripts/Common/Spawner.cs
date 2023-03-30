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
    [SerializeField] int[] amounts;

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(delay);


        for (int i = 0; i < currentObjects.Length; ++i)
        {
            for (int j = 0; j < amounts[i]; ++j)
            {
                Vector3 pos = spawnPosition + new Vector3(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), 0f);
                Instantiate(currentObjects[i], pos, Quaternion.identity);
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
