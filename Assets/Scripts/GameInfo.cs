using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*Сюда я буду пихать системную инфу, чтобы потом над ней думать*/

public class GameInfo : MonoBehaviour
{
    [SerializeField] public int delay;
    protected delegate void Send(string text);
    protected Send SendText;

    IEnumerator EntityLogger()
    {
        yield return new WaitForSeconds(delay);
        var numOfGoats = GameObject.FindGameObjectsWithTag("Goat").Length;
        var numOfWolves = GameObject.FindGameObjectsWithTag("Wolf").Length;
        var numOfBushes = GameObject.FindGameObjectsWithTag("Bush").Length;
        SendText($"Всего сущностей: {numOfGoats + numOfBushes+numOfWolves}.\n Волки: {numOfWolves}.\n Козы: {numOfGoats},\n кусты: {numOfBushes}");
        //Debug.Log($"Всего сущностей: {numOfGoats + numOfBushes+numOfWolves}. Волки: {numOfWolves}. Козы: {numOfGoats}, кусты: {numOfBushes}");
        Start();
    }



    // Start is called before the first frame update
    void Start()
    {
        SendText = GameObject.FindGameObjectWithTag("MainUI").GetComponent<InfoText>().ChangeText;
        SendText+=Debug.Log;
        StartCoroutine(EntityLogger());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
