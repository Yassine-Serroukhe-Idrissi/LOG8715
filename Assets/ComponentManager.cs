using System;
using System.Collections.Generic;

public class ComponentManager
{
    private Dictionary<Type, Dictionary<int, object>> components = new Dictionary<Type, Dictionary<int, object>>();
    private int nextEntityId = 1;

    public int CreateEntity() => nextEntityId++;

    public void AddComponent<T>(int entityId, T component)
    {
        Type type = typeof(T);
        if (!components.ContainsKey(type))
            components[type] = new Dictionary<int, object>();
        components[type][entityId] = component;
    }

    public T GetComponent<T>(int entityId)
    {
        if (components.TryGetValue(typeof(T), out var entityComponents))
            if (entityComponents.TryGetValue(entityId, out var component))
                return (T)component;
        return default;
    }

    
    public bool HasComponent<T>(int entityId)
    {
        Type componentType = typeof(T);
        return components.ContainsKey(componentType)
            && components[componentType].ContainsKey(entityId);
    }

    public IEnumerable<int> GetEntitiesWithComponents(params Type[] types)
    {
        foreach (var entity in GetAllEntities())
        {
            bool valid = true;
            foreach (var type in types)
                if (!components.ContainsKey(type) || !components[type].ContainsKey(entity))
                    valid = false;
            if (valid) yield return entity;
        }
    }

    private IEnumerable<int> GetAllEntities()
    {
        HashSet<int> entities = new HashSet<int>();
        foreach (var dict in components.Values)
            foreach (int id in dict.Keys)
                entities.Add(id);
        return entities;
    }
}