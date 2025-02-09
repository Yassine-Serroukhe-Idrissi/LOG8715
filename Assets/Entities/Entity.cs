using System;
using System.Collections.Generic;

/// An entity is identified by a unique uint and holds a set of data-only components.

public class Entity
{
    public uint Id { get; }
    private readonly Dictionary<Type, IComponent> _components;

    public Entity(uint id)
    {
        Id = id;
        _components = new Dictionary<Type, IComponent>();
    }

    public void AddComponent(IComponent component)
    {
        _components[component.GetType()] = component;
    }

    public T GetComponent<T>() where T : class, IComponent
    {
        if (_components.TryGetValue(typeof(T), out IComponent comp))
            return comp as T;
        return null;
    }

    public bool HasComponent<T>() where T : IComponent
    {
        return _components.ContainsKey(typeof(T));
    }

    public void RemoveComponent<T>()
    {
        _components.Remove(typeof(T));
    }
}
