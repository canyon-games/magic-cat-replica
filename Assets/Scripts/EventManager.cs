using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager 
{
    public static UnityEvent onGamePlay = new UnityEvent();
    public static UnityEvent OnLevelComplete = new UnityEvent();
    public static UnityEvent OnLevelFail = new UnityEvent();
}
