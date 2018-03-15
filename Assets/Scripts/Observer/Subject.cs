using System.Collections.Generic;

public abstract class Subject
{
    private List<IObserver> observers = new List<IObserver>();

    public void Subscribe(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Unsubscribe(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void Notify(object value)
    {
        for (int i = 0; i < observers.Count; i++)
        {
            observers[i].ValueChanged(value);
        }
    }
}