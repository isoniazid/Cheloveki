using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldTime : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _worldTimeTextLabel;
    private float _timerStart;
    private float _timeThreshold = 0.01f; //Игровая секунда

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
        if(_currentHour > _dayLen)
        {
            _currentHour = 0;
            _currentDay++;
        }

        if(_currentDay > _weekLen)
        {
            _currentDay = 0;
            _currentWeek++;
        }

        if(_currentWeek > _monthLen)
        {
            _currentWeek = 0;
            _currentMonth++;
        }

        if(_currentMonth > _yearLen)
        {
            _currentMonth = 0;
            _currentYear++;
        }

    }

    private void setText()
    {

        _worldTimeTextLabel.text = "";
        _worldTimeTextLabel.text+= $"Год: {_currentYear}\n";
        _worldTimeTextLabel.text+= $"Месяц: {_currentMonth}\n";
        _worldTimeTextLabel.text+= $"Неделя: {_currentWeek}\n";
        _worldTimeTextLabel.text+= $"День: {_currentDay}\n";
        _worldTimeTextLabel.text+= $"Час: {_currentHour}\n";
    }

    private bool TickPassed()
    {
        var currentTime = Time.time;
        if(currentTime - _timerStart >= _timeThreshold)
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
        if(_hour.isPassed(TickPassed()))
        {
            _currentHour++;
            Compute();
            setText();
        }
    }
}