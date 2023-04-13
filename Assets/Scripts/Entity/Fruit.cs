using UnityEngine;

public class Fruit : Entity, IThing, IEdible
{
    [SerializeField] int startSatInc;
    [SerializeField] int startWeight; //NB PEREDELATb, протупил, свойства не выводятся в юнити

    public int satietyIncrement { get { return startSatInc; } set {} }
    public int weight { get {return startWeight;} set {} }
    public int price { get; set; }

    public override void Start()
    {
        base.Start();
        price = 1;
        name = "Плод";
        this.satietyIncrement = satietyIncrement;
    }

    public override void Update()
    {

    }
}