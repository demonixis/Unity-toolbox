# A static messenger system

This is a static messenger system. You can easily send an event with ```Messenger.Notify()``` method. This method can be used with or without parameters.

- ```Messenger.Notify(string eventName, string message)```
- ```Messenger.Notify(string eventName, BasicMessage message)```
- ```Messenger.Notify(string eventName, GenericMessage<T> message)```

A class can be notified by a message. You have to register a callback function for this type of message with the following method 
- ```Messenger.Register(string eventName, BasicMessage callback)```

Obviously, you need to unregister your callback function when the object is destroyed. For that you must use the following method
- ```Messenger.Unregister(string eventName, string message)```

## Basic usage
_Player.cs_
```csharp
void Update()
{
    if (Input.GetKeyDown(Keys.Space))
    {
        Shoot();
        // Notify to all registered objects that a bullet has been fired.
        // The GenericMessage contains the number of the remaining bullets.
        Messenger.Notify("Player.Shooted", new GenericMessage<int>(this.bullets.Count));
    }
    
    if (Input.GetKeyDown(Keys.F))
    {
        flashlight.enabled = !flashlight.enabled;
        // Send a simple message
        Messenger.Notify("Player.Use.Flashlight", flashLight.enabled ? "On" : "Off");
    }
}
```

_UIManager.cs_
```csharp
void Start()
{
    // Register a callback function for this type of event.
    Messenger.Register("Player.Shooted", UpdatePlayerBullets);
}

void Destroy()
{   
    // Don't forget to unregister when the script is destroyed to prevent memory leaks.
    Messenger.Unregister("Player.Shooted", UpdatePlayerBullets);
}

private void UpdatePlayerBullets(BasicMessage m)
{
    var intMessage = m as GenericMessage<int>();
    _playerBullet.text = intMessage.Value.toString();
}
```
