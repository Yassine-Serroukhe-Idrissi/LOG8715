using System.Collections.Generic;

public static class EntityManager
{
    private static readonly Dictionary<uint, Entity> _entities = new Dictionary<uint, Entity>();
    private static uint _nextId = 1;

    public static Entity CreateEntity()
    {
        uint id = _nextId++;
        var entity = new Entity(id);
        _entities[id] = entity;
        return entity;
    }

    public static Entity GetEntity(uint id)
    {
        _entities.TryGetValue(id, out var entity);
        return entity;
    }

    public static IEnumerable<Entity> GetAllEntities()
    {
        return _entities.Values;
    }

    // Overload for one type parameter.
    public static IEnumerable<Entity> GetEntitiesWith<T1>() where T1 : IComponent
    {
        foreach (var entity in _entities.Values)
        {
            if (entity.HasComponent<T1>())
                yield return entity;
        }
    }

    // Overload for two type parameters.
    public static IEnumerable<Entity> GetEntitiesWith<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent
    {
        foreach (var entity in _entities.Values)
        {
            if (entity.HasComponent<T1>() && entity.HasComponent<T2>())
                yield return entity;
        }
    }

    // Overload for three type parameters.
    public static IEnumerable<Entity> GetEntitiesWith<T1, T2, T3>()
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
    {
        foreach (var entity in _entities.Values)
        {
            if (entity.HasComponent<T1>() && entity.HasComponent<T2>() && entity.HasComponent<T3>())
                yield return entity;
        }
    }

    // Overload for four type parameters.
    public static IEnumerable<Entity> GetEntitiesWith<T1, T2, T3, T4>()
        where T1 : IComponent
        where T2 : IComponent
        where T3 : IComponent
        where T4 : IComponent
    {
        foreach (var entity in _entities.Values)
        {
            if (entity.HasComponent<T1>() && entity.HasComponent<T2>() &&
                entity.HasComponent<T3>() && entity.HasComponent<T4>())
                yield return entity;
        }
    }

    public static void RemoveEntity(uint id)
    {
        _entities.Remove(id);
    }
}
