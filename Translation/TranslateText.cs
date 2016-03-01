using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public sealed class TranslateText : MonoBehaviour
{
    public string key;

    void Start()
    {
        var text = GetComponent(typeof(Text)) as Text;
        text.text = Translation.Get(key != string.Empty ? key : text.text);

        Destroy(this);
    }
}

