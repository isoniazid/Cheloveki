using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Entity
{
    public List<Human> inhabitors = new List<Human>();
    public List<GameObject> peopleAtHome = new List<GameObject>();

    protected override void OnMouseDown()
    {
        messageText = "";
        messageText += $"Это: {name}\n";
        messageText += $"Жильцы:\n";
        foreach (var human in inhabitors) messageText += $"{human.firstName} {human.lastName}\n";
        SendText(messageText);
    }

    public void AddInhabitor(Human human)
    {
        inhabitors.Add(human);
    }

    public void RemoveInhabitor(Human human)
    {
        inhabitors.Remove(human);
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