using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace OmmyShapeML
{
public static class EventManager 
{
    public static UnityEvent OnLevelComplete = new UnityEvent();
    public static UnityEvent OnLevelFail = new UnityEvent();
}
}