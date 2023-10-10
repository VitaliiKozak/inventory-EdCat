using System.Collections.Generic;


public class SimpleEvent
{
    private List<System.Action> _eventListeners = new List<System.Action>();
    private List<System.Action> _eventListenersSafe = new List<System.Action>();

    public void AddListener(System.Action listener)
    {
        _eventListeners.Add(listener);
    }

    public void RemoveListener(System.Action listener)
    {
        _eventListeners.Remove(listener);
    }

    public void RemoveAllListeners()
    {
        _eventListeners.Clear();
    }

    public void Notify()
    {
        _eventListenersSafe.Clear();
        _eventListenersSafe.AddRange(_eventListeners);

        foreach (var action in _eventListenersSafe)
        {
            action?.Invoke();
        }


    }
}

public class SimpleEvent<T>
{
    private List<System.Action<T>> _eventListeners = new List<System.Action<T>>();
    private List<System.Action<T>> _eventListenersSafe = new List<System.Action<T>>();
    private T _value;

    public void AddListener(System.Action<T> listener)
    {
        _eventListeners.Add(listener);
    }

    public void SetValue(T value)
    {
        _value = value;
    }

    public void RemoveListener(System.Action<T> listener)
    {
        _eventListeners.Remove(listener);
    }

    public void RemoveAllListeners()
    {
        _eventListeners.Clear();
    }

    public void Notify(T value)
    {
        _eventListenersSafe.Clear();
        _eventListenersSafe.AddRange(_eventListeners);

        foreach (var action in _eventListenersSafe)
        {
            action?.Invoke(value);
        }
    }
}
