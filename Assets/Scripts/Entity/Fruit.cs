public class Fruit : Entity, IThing, IEdible
{
    public int satietyIncrement {get;set;}
    public int weight { get; set; }
    public int price { get; set; }

    public override void Start()
    {
        base.Start();
        weight = 1;
        price = 1;
        name = "Плод";
        satietyIncrement = 1;
        this.satietyIncrement = satietyIncrement;
    }

    public override void Update()
    {

    }
}