using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class TIME_LEN
{
    /*Я по умолчанию выставил у всех юнитов тикер в 1 игровой час*/
    public const float GAME_MINUTE = 0.001f;
    public const float DEFAULT_TICK = 0.6f;
    public const int DAYNIGHT_LEN = 24;
    public const int WEEK_LEN = 7 * 24;
    public const int MONTH_LEN = 7 * 4 * 24;
    public const int YEAR_LEN = 7 * 4 * 24 * 365;
}


public class WorldTime : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _worldTimeTextLabel;
    private float _timerStart;
    private float _timeThreshold = TIME_LEN.GAME_MINUTE; //Игровая минута

    private Period _hour = new Period(60); //Игровой час

    const int _dayLen = 24;
    const int _weekLen = 7;
    const int _monthLen = 4;
    const int _yearLen = 365;

    private int _currentHour = 1;
    private int _currentDay = 1;
    private int _currentWeek = 1;
    private int _currentMonth = 1;
    private int _currentYear = 1;

    //private Period _dayNight = new Period(60*24); // Игровые сутки

    //private Period _month = new Period(60*24*30); //Игровой месяц

    private void Compute()
    {
        if (_currentHour > _dayLen)
        {
            _currentHour = 1;
            _currentDay++;
        }

        if (_currentDay > _weekLen)
        {
            _currentDay = 1;
            _currentWeek++;
        }

        if (_currentWeek > _monthLen)
        {
            _currentWeek = 1;
            _currentMonth++;
        }

        if (_currentMonth > _yearLen)
        {
            _currentMonth = 1;
            _currentYear++;
        }

    }

    private void setText()
    {

        _worldTimeTextLabel.text = "";
        _worldTimeTextLabel.text += $"Год: {_currentYear}\n";
        _worldTimeTextLabel.text += $"Месяц: {_currentMonth}\n";
        _worldTimeTextLabel.text += $"Неделя: {_currentWeek}\n";
        _worldTimeTextLabel.text += $"День: {_currentDay}\n";
        _worldTimeTextLabel.text += $"Час: {_currentHour}\n";
    }

    private bool TickPassed()
    {
        var currentTime = Time.time;
        if (currentTime - _timerStart >= _timeThreshold)
        {
            _timerStart = currentTime;
            return true;
        }
        return false;
    }
    // Start is called before the first frame update
    void Start()
    {
        _timerStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (_hour.isPassed(TickPassed()))
        {
            _currentHour++;
            Compute();
            setText();
        }
    }
}
