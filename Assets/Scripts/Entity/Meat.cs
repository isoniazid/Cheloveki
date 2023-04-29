using UnityEngine;

public class Meat : Entity, IThing, IEdible
{

    [field: SerializeField]
    public int satietyIncrement { get; set; }

    [field: SerializeField]
    public int weight { get; set; }
    public int price { get; set; }

    [SerializeField] public string meatType;

    public override void Start()
    {
        base.Start();
        price = 1;
        name = $"Мясо: {meatType}";
    }

    public override void Update()
    {

    }
}
