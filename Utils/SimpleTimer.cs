using System;
using UnityEngine;

/// <summary>
/// An event driven timer.
/// </summary>
public sealed class SimpleTimer : MonoBehaviour
{
    [SerializeField]
    private float _interval = 0;

    [SerializeField]
    private bool _repeat = false;

    [SerializeField]
    private bool _ignoreTimeScale = false;

    private float _elapsedTime = 0;
    private bool _enabled = false;

    public bool IsActive
    {
        get { return _enabled; }
    }

	public float Duration
	{
		get { return _interval; }
		set { _interval = value; }
	}

	public bool Repeat
	{
		get { return _repeat; }
		set { _repeat = value; }
	}

    public bool IgnoreTimeScale
    {
        get { return _ignoreTimeScale; }
        set { _ignoreTimeScale = value; }
    }

    public float ElapsedTime
    {
        get { return _elapsedTime; }
        set { _elapsedTime = value; }
    }

	public event EventHandler<EventArgs> Completed = null;

    public void Begin()
    {
        _elapsedTime = 0;
        _enabled = true;
    }

	public void Stop()
    {
        _enabled = false;
		_elapsedTime = 0;
	}

	void Update()
	{
        if (_enabled)
        {
            _elapsedTime += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

            if (_elapsedTime >= _interval)
            {
                _enabled = _repeat;

                if (Completed != null)
                    Completed(this, EventArgs.Empty);
            }
        }
	}

	public float GetTimeRemaining()
	{
		return Duration - _elapsedTime;
	}

	public float GetPrecisePercent()
	{
		return (float)(_elapsedTime * 100.0f) / (float)_interval;
	}
	
	public int GetPercent()
	{
		return Mathf.Min(Mathf.RoundToInt(GetPrecisePercent()), 100);
	}

	public int GetPercentStep()
	{
		int percent = GetPercent();
		return percent - (percent % 10);
	}
}
