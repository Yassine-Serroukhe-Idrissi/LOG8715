using UnityEngine;
using System;
using System.Collections.Generic;
/// <summary>
/// entities : stocke les entités et leurs composants 
/// AddComponent : ajoute un compo a une entité 
/// GetComponent : recup un copo d une entité 
/// SetCo,ponent : MAJ d une entité
/// GetEntitiesWith : Recup les entités qui ont un certain type de composants 
/// 
/// </summary>
public static class World
{
    //On stocke les composants par entite 
    private static readonly Dictionary<uint, Dictionary<Type, IComponent>> entities = new(); //dictionnaire : Type -> Composant

    private static uint nextEntityId = 1;

    public static uint CreateEntity()
    {
        var entityId = nextEntityId++;
        entities[entityId] = new Dictionary<Type, IComponent>();
        return entityId;
    }

    // ajoute un composant a une entite
    public static void AddComponent<T>(uint entityId, T component ) where T : IComponent
    {
        if (!entities.ContainsKey(entityId))
        {
            entities[entityId] = new Dictionary<Type, IComponent>();
        }
        entities[entityId][typeof(T)] = component;
    }

    //recup un composant a une entite
    public static T GetComponent<T>(uint entityId) where T : IComponent
    {
        
         return (T)entities[entityId][typeof(T)];
       
    }

    //def un composant pour une entite 
    public static void SetComponent<T>(uint entityId, T component) where T : IComponent
    {
        
            entities[entityId][typeof(T)] = component;
        
    }

    //Recup toutes les entites avec des composants spe
    public static IEnumerable<uint> GetEntitiesWith<T1, T2>() where T1 : IComponent where T2 : IComponent
    {
        foreach (var entity in entities)
        {
            if(entity.Value.ContainsKey(typeof(T1)) && entity.Value.ContainsKey(typeof(T2)))
            {
                yield return entity.Key;
            }

        }
    }

    //signifie si l entite a le component
    public static bool HasComponent<T>(uint entityId) where T : IComponent
    {
        if (!entities.ContainsKey(entityId))
        {
            return false;
        }
        return entities[entityId].ContainsKey(typeof(T));
    }

    public static void RemoveEntity (uint entityId)
    {
        entities.Remove(entityId);
    }

    public static bool HasEntity(uint entityId)
    {
        return entities.ContainsKey(entityId);
    }

    public static void RemoveComponent<T>(uint entityId)  where T : IComponent
    {
        if(entities.ContainsKey(entityId) && entities[entityId].ContainsKey(typeof(T)))
        {
            entities[entityId].Remove(typeof(T));

            // si l entite n aplus de composant on le supp completement 

            if (entities[entityId].Count == 0)
            {
                entities.Remove(entityId);
            }
        }
    }
}
