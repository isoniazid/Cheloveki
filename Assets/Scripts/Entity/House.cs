using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Entity
{
    public List<Human> inhabitors = new List<Human>();

    protected override void OnMouseDown()
    {
        messageText = "";
        messageText += $"Это: {name}\n";
        messageText += $"Жильцы:\n";
        foreach (var human in inhabitors) messageText += $"{human.firstName} {human.lastName}";
        SendText(messageText);
    }

    public override void Start()
    {
        base.Start();
        name = "дом";
    }


    public override void Update()
    {

    }
}