public class SexNecessity : Necessity
{

    public SexNecessity() : base()
    {
        name = "Секс";
        _priority = (int)PRIORITY.PHISIOLOGICAL;
    }

    public SexNecessity(int min, int max) : base(min, max)
    {
        name = "Секс";
        _priority = (int)PRIORITY.PHISIOLOGICAL;
    }
}