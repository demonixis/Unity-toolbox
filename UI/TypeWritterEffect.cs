using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public sealed class TypeWritterEffect : MonoBehaviour
{
    private Text _text;
    private StringBuilder _stringBuilder = new StringBuilder();
    private int _size = 0;
    private float _elaspedTime = 0;
    private string _contentText = string.Empty;
    private bool _done = true;
    private int _index = 0;
    public float displaySpeed = 0.75f;
    public int letterPerCycle = 3;
    public bool autoStart = true;
    public bool activeOnEnable = false;

    public bool Started { get; private set; }

    public event EventHandler<EventArgs> Completed = null;

    void OnEnable()
    {
        if (activeOnEnable)
        {
            autoStart = false;
            Start();
            Begin(Translation.Get(_text.text));
        }
    }

    void Start()
    {
        if (!Started)
        {
            Started = true;

            _text = GetComponent(typeof(Text)) as Text;

            if (autoStart)
                Begin(_text.text);
        }
    }

    void Update()
    {
        if (!_done)
        {
            _elaspedTime += Time.deltaTime;
            UpdateText();
        } 
    }

    public void Begin(string text)
    {
        Start();
        _contentText = text;
        _size = text.Length;
        _elaspedTime = displaySpeed;
        _index = 0;
        _done = false;
        _stringBuilder.Length = 0;
        _text.text = string.Empty;
        UpdateText();
    }

    public void Stop()
    {
        _elaspedTime = displaySpeed;
        _done = true;
    }

    private void UpdateText()
    {
        if (_elaspedTime >= displaySpeed)
        {
            var limit = Mathf.Min(_index + letterPerCycle, _size);

            for (int i = _index; i < limit; i++)
                _stringBuilder.Append(_contentText[i]);

            _index = limit;
            _text.text = _stringBuilder.ToString();
            _elaspedTime = 0;

            if (_index >= _size)
            {
                _done = true;

                if (Completed != null)
                    Completed(this, EventArgs.Empty);
            }
        }
    }
}