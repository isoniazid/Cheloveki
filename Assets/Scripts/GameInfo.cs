using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*Сюда я буду пихать системную инфу, чтобы потом над ней думать*/

public class GameInfo : MonoBehaviour
{
    [SerializeField] public int delay;

    IEnumerator EntityLogger()
    {
        yield return new WaitForSeconds(delay);
        var numOfGoats = GameObject.FindGameObjectsWithTag("Goat").Length;
        var numOfBushes = GameObject.FindGameObjectsWithTag("Bush").Length;
        Debug.Log($"Всего сущностей: {numOfGoats + numOfBushes}. Козы: {numOfGoats}, растения (кусты): {numOfBushes}");
        Start();
    }



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EntityLogger());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
