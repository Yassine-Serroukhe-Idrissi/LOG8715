using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


public class SizeSystem :ISystem
{
    public string Name => "MovementSystem";

    public void UpdateSystem()
    {
        var entities = World.GetEntitiesWith<SizeComponent, CircleTypeComponent>();
        var entityList = new List<uint>(entities);

        for (int i =0; i<entityList.Count; i++)
        {
            var entity = entityList[i];
            if (!World.HasEntity(entity))
            {
                Debug.LogWarning($"l entite = {entity} n existe plus");
                continue;
            }
            if (!World.HasComponent<CollisionDataComponent>(entity))
            {
                Debug.LogWarning($"l entite = {entity} n existe plus");
                continue;
            }
            var collisionData = World.GetComponent<CollisionDataComponent>(entity);
            if (!World.HasEntity(collisionData.otherEntity))
            {
                Debug.LogWarning($"l entite = {collisionData.otherEntity} n existe plus");
                continue;
            }
            var size = World.GetComponent<SizeComponent>(entity);
            var type = World.GetComponent< CircleTypeComponent > (entity).type;
            var otherType = World.GetComponent<CircleTypeComponent>(collisionData.otherEntity).type;
            var otherSize = World.GetComponent<SizeComponent>(collisionData.otherEntity);

            if(type == CircleType.Static || otherType == CircleType.Static)
            {
                continue;
            }
            else 
            {
                //TAILLE 

                if (size.radius > otherSize.radius)
                {
                    size.radius += 1;
                    otherSize.radius -= 1;
                }
                else if (otherSize.radius > size.radius)
                {
                    size.radius -= 1;
                    otherSize.radius += 1;
                }

                //On supprime les cercles qui ont une taille de 0
                if (size.radius <= 0)
                {
                    Debug.Log($"l entite {entity} supp car taille = 0");
                    World.RemoveEntity(entity);
                    ECSController.Instance.DestroyShape(entity);

                }
                else if (World.HasEntity(entity))
                {
                    World.SetComponent(entity, size);
                    ECSController.Instance.UpdateShapeSize(entity, size.radius);
                }

                if (otherSize.radius <= 0)
                {
                    Debug.Log($"Entite{collisionData.otherEntity} supp car taille = 0");
                    World.RemoveEntity(collisionData.otherEntity);
                    ECSController.Instance.DestroyShape(collisionData.otherEntity);

                }
                else if (World.HasEntity(collisionData.otherEntity))
                {
                    World.SetComponent(collisionData.otherEntity, otherSize);
                    ECSController.Instance.UpdateShapeSize(collisionData.otherEntity, otherSize.radius);
                }
            }

        }
        // Supprimer le composant temporaire après traitement
        foreach (var entity in entities)
        {
            World.RemoveComponent<CollisionDataComponent>(entity);
        }
    }
}
