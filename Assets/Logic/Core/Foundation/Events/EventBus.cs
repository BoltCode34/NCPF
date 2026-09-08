using System;
using System.Collections.Generic;
using Core.Foundation.LifeTime;
using UnityEngine;

namespace Core.Foundation.Events
{
    public abstract class EventBus<T> : ICommand where T : class
    {
        private List<WeakReference<T>> _listeners;
        public EventBus()
        {
            _listeners = new List<WeakReference<T>>();
        }
        public void AddListener(T listener)
        {
            if (listener == null) return;
            _listeners.Add(new WeakReference<T>(listener));
        }
        public void RemoveListener(T listener)
        {
            if (listener == null) return;

            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                if (_listeners[i].TryGetTarget(out var target) && ReferenceEquals(target, listener))
                {
                    _listeners.RemoveAt(i);
                }
            }
        }
        public void Execute()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                if (!_listeners[i].TryGetTarget(out T listener) || listener == null)
                {
                    _listeners.RemoveAt(i);
                }
                else
                {
                    try
                    {
                        CallItem(listener);

                    }
                    catch(Exception e) 
                    {
                        Debug.Log($"Listener: {_listeners[i]},\n Listener id: {i},\n Count: {_listeners.Count}");
                        _listeners.RemoveAt(i);
                        Debug.LogError(e);
                    }
                }
            }
        }
        protected abstract void CallItem(T item);
    }
    public class UpdateBus : EventBus<IUpdatable>
    {
        protected override void CallItem(IUpdatable item)
        {
            item.UpdateTick();
        }
    }
    public class FixedUpdateBus : EventBus<IFixedUpdatable>
    {
        protected override void CallItem(IFixedUpdatable item)
        {
            item.FixedTick();
        }
    }
}