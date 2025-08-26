using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Observer : Singleton<Observer>
{
    Dictionary<ObserverKey, List<Action<object>>> observes = new Dictionary<ObserverKey, List<Action<object>>>();

    public void AddObserver(ObserverKey key, Action<object> observerCallback)
    {
        var observerList = GetObserverList(key);
        observerList.Add(observerCallback);
    }

    public void RemoveObserver(ObserverKey key, Action<object> observerCallback)
    {
        var observerList = GetObserverList(key);
        observerList.Remove(observerCallback);
    }

    public void Notify(ObserverKey key, object data)
    {
        var observerList = GetObserverList(key);
        for(int i  = observerList.Count - 1; i >= 0; i--)
        {
            observerList[i](data);
        }
    }

    public void Notify(ObserverKey key)
    {
        Notify(key, null);
    }

    public List<Action<object>> GetObserverList(ObserverKey key)
    {
        if (!observes.ContainsKey(key))
        {
            observes.Add(key, new List<Action<object>>());
        }
        return observes[key];
    }

}