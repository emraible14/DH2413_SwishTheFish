using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEvent
{
    public EventManager.EventType type { get; private set; }
    public object data { get; private set; }

    public CustomEvent(EventManager.EventType type, object data)
    {
        this.type = type;
        this.data = data;
    }
}
