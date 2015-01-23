using System;
using System.Text;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    private DateTime _startTime = DateTime.Now;
    private float _minutes = 0;
    private float _seconds = 0;
    private float _miliseconds = 0;
    private StringBuilder _builder = new StringBuilder();
    public int minutes = 5;
    public int seconds = 0;
    public int milliseconds = 0;

    void Start()
    {
        _minutes = minutes;
        _seconds = seconds;
        _miliseconds = milliseconds;
    }

    void Update()
    {
        if (_minutes == 0 && _minutes == 0 && _seconds == 0)
            Messenger.Notify("player.died");
    }

    public string GetElaspsedTime()
    {
        var diff = DateTime.Now - _startTime;
        var dMins = diff.Minutes.ToString();
        var dSecs = diff.Seconds.ToString();
        var dMs = diff.Milliseconds.ToString();

        if (dMins.Length == 1)
            dMins = string.Concat("0", dMins);

        if (dSecs.Length == 1)
            dSecs = string.Concat("0", dSecs);
        else if (dSecs.Length > 2)
            dSecs = string.Concat(dSecs[0], dSecs[1]);

        if (dMs.Length == 1)
            dMs = string.Concat("0", dMs);
        else if (dMs.Length > 2)
            dMs = string.Concat(dMs[0], dMs[1]);
        
        return string.Concat(dMins, ":", dSecs, ":", dMs);
    }

    public string UpdateTime()
    {
        _builder.Length = 0;

        if (_miliseconds < 0)
        {
            if (_seconds <= 0)
            {
                _minutes--;
                _seconds = 59;
            }
            else if (_seconds >= 0)
                _seconds--;

            _miliseconds = 100;
        }
        else if (_miliseconds >= 100)
        {
            if (_seconds >= 60)
            {
                _minutes++;
                _seconds = 0;
            }
            else if (_seconds <= 60)
                _seconds++;

            _miliseconds = 0;
        }

        _miliseconds -= Time.deltaTime * 100;

        if (_minutes < 10)
            _builder.Append("0");

        _builder.Append(_minutes);
        _builder.Append(":");

        if (_seconds < 10)
            _builder.Append("0");

        _builder.Append(_seconds);
        _builder.Append(":");

        if ((int)_miliseconds < 10)
            _builder.Append("0");

        _builder.Append((int)_miliseconds);

        return _builder.ToString();
    }
}