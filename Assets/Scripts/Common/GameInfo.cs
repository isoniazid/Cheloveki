using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*Сюда я буду пихать системную инфу, чтобы потом над ней думать*/

public class GameInfo : MonoBehaviour
{
    [SerializeField] public int delay;
    [SerializeField] public GameObject[] objsToCount;
    [SerializeField] public string[] objNames;
    protected delegate void Send(string text);
    protected Send SendText;

    IEnumerator EntityLogger()
    {
        yield return new WaitForSeconds(delay);
        int totalNum = 0;
        string text = "";
        for (int i = 0; i < objsToCount.Length; ++i)
        {
            var numOfObjects = GameObject.FindGameObjectsWithTag(objsToCount[i].tag).Length;
            totalNum += numOfObjects;
            text += $"{objNames[i]}: {numOfObjects}\n";
        }
        text += $"Всего сущностей: {totalNum}";
        SendText(text);
        Start();
    }



    // Start is called before the first frame update
    void Start()
    {
        SendText = GameObject.FindGameObjectWithTag("MainUI").GetComponent<InfoText>().ChangeText;
        SendText += Debug.Log;
        StartCoroutine(EntityLogger());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
