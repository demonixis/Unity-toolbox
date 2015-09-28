using System;
using UnityEngine;

public class CheatCode : MonoBehaviour
{
    private int _index = 0;
    public string[] Code { get; set; }
    public string Description { get; set; }
    public Action SuccessCallback { get; set; }
    public bool DestroyAfterUnlock = true;

    void Update()
    {
        if (_index > -1)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(Code[_index].ToLower()))
                    _index++;
                else
                    _index = 0;
            }

            if (_index == Code.Length)
            {
                SuccessCallback();

                if (DestroyAfterUnlock)
                    Destroy(this);
                else
                    _index = 0;
            }
        }
    }
}