using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TranslateText : MonoBehaviour
{
    public string key;

    void Start()
    {
        if (key != string.Empty)
        {
            var text = GetComponent(typeof(Text)) as Text;
            text.text = Translation.Get(key);   
        }

        Destroy(this);
    }
}

