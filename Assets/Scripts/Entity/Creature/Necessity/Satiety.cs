public class Satiety : Necessity
{
     
    //На старте максимально сыт
    public Satiety() : base()
    {
        name = "Сытость";
        _priority = (int)PRIORITY.PHISIOLOGICAL;
    }

    public Satiety(int min, int max) : base(min, max)
    {
        name = "Сытость";
        _priority = (int)PRIORITY.PHISIOLOGICAL;

    }
}