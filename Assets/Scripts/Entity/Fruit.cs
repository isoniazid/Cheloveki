using UnityEngine;

public class Fruit : Entity, IThing, IEdible
{

    [field: SerializeField]
    public int satietyIncrement { get; set; }

    [field: SerializeField]
    public int weight { get; set; }
    public int price { get; set; }

    public override void Start()
    {
        base.Start();
        price = 1;
        name = "Плод";
    }

    public override void Update()
    {

    }
}