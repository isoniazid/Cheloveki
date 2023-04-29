public class HomeNecessity: Necessity
{

    public HomeNecessity()
    {
        name = "Наличие дома";
        _thresholdLevel = 0;
        _maxLevel = 1;
        currentState = _maxLevel;
    }
    
}