using System;
using System.Text;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    private float _elapsedTime = 0.0f;
    private float _minutes = 0.0f;
    private float _seconds = 0.0f;
    private float _miliseconds = 0.0f;
    private bool _done = false;
    private bool _timeAttackMode = true;
    private StringBuilder _builder = new StringBuilder();
    public int minutes = 5;
    public int seconds = 0;
    public int milliseconds = 0;

    public bool TimeAttackMode
    {
        get { return _timeAttackMode; }
        set { _timeAttackMode = value; }
    }

    void Start()
    {
        _minutes = minutes;
        _seconds = seconds;
        _miliseconds = milliseconds;
    }

    void Update()
    {
        if (!_done)
            _elapsedTime += Time.deltaTime;

        if (_timeAttackMode && !_done && _minutes <= 0 && _minutes <= 0 && _seconds <= 0)
        {
            _done = true;
            _minutes = 0.0f;
            _seconds = 0.0f;
            _miliseconds = 0.0f;
            Messenger.Notify("time.over");
        }

#if UNITY_EDITOR
        if (_timeAttackMode && Input.GetKeyDown(KeyCode.F12))
            Messenger.Notify("time.over");
#endif
    }

    public float GetMaxTime()
    {
        return (float)(minutes * 60 + seconds);
    }

    public float GetElapsedTime()
    {
        return _elapsedTime;
    }

    public string GetElaspsedTime()
    {
        var time = TimeSpan.FromSeconds(_elapsedTime);
        var dMins = time.Minutes.ToString();
        var dSecs = time.Seconds.ToString();
        var dMs = time.Milliseconds.ToString();

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

    public string GetTime()
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