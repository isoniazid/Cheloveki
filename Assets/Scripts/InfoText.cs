using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI infoText;
    // Start is called before the first frame update
    void Start()
    {
        infoText.text = "";
    }

    public void ChangeText(string text)
    {
        infoText.text = text;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
