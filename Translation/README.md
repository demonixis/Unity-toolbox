# Translation system

### Translation system

By default, you must create the following hierarchie in your asset folder:

* Resources/Translations/Texts.EN.txt
* Resources/Translations/Texts.FR.txt

Of course you can change that by adapting it to your needs. Don't forget to change the value of the `AvailableLanguages` constante.

Using it
========

```csharp
var hello = Translation.Get("Hello");
```

### Translate UI elements

The `TranslateText` script must be used on a GameObject that has a `UnityEngine.UI.Text` component.
You just have to specify the desired key. If the field is empty, the component will try to translate the value of the text contained by the `text` property.