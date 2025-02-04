using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects collisions between circles, applies collision responses via CollisionUtility,
/// adjusts circle sizes, and removes circles that shrink to size 0.
/// </summary>
public class CollisionSystem : ISystem
{
    public string Name => "CollisionSystem";

    public void UpdateSystem()
    {
        // Get all entities that have the required components.
        List<Entity> entities = new List<Entity>(EntityManager.GetEntitiesWith<PositionComponent, VelocityComponent, SizeComponent, CircleTypeComponent>());
        int count = entities.Count;

        for (int i = 0; i < count; i++)
        {
            for (int j = i + 1; j < count; j++)
            {
                var entityA = entities[i];
                var entityB = entities[j];

                var posA = entityA.GetComponent<PositionComponent>();
                var velA = entityA.GetComponent<VelocityComponent>();
                var sizeA = entityA.GetComponent<SizeComponent>();
                var typeA = entityA.GetComponent<CircleTypeComponent>();

                var posB = entityB.GetComponent<PositionComponent>();
                var velB = entityB.GetComponent<VelocityComponent>();
                var sizeB = entityB.GetComponent<SizeComponent>();
                var typeB = entityB.GetComponent<CircleTypeComponent>();

                float radiusA = sizeA.Size / 2f;
                float radiusB = sizeB.Size / 2f;
                float minDistance = radiusA + radiusB;
                if (Vector2.Distance(posA.Position, posB.Position) > minDistance)
                    continue;

                // Calculate collision response.
                CollisionResult result = CollisionUtility.CalculateCollision(
                    posA.Position, velA.Velocity, sizeA.Size,
                    posB.Position, velB.Velocity, sizeB.Size);

                if (result == null)
                    continue;

                // Update positions and velocities.
                posA.Position = result.position1;
                velA.Velocity = result.velocity1;
                posB.Position = result.position2;
                velB.Velocity = result.velocity2;

                ECSController.Instance.UpdateShapePosition(entityA.Id, posA.Position);
                ECSController.Instance.UpdateShapePosition(entityB.Id, posB.Position);

                // Only dynamic circles change size on collision.
                if (typeA.Type == CircleType.Static || typeB.Type == CircleType.Static)
                    continue;

                if (sizeA.Size != sizeB.Size)
                {
                    if (sizeA.Size > sizeB.Size)
                    {
                        sizeA.Size += 1;
                        sizeB.Size = Mathf.Max(0, sizeB.Size - 1);
                    }
                    else
                    {
                        sizeB.Size += 1;
                        sizeA.Size = Mathf.Max(0, sizeA.Size - 1);
                    }
                    ECSController.Instance.UpdateShapeSize(entityA.Id, sizeA.Size);
                    ECSController.Instance.UpdateShapeSize(entityB.Id, sizeB.Size);
                }

                // Increment collision counters for potential protection.
                var protA = entityA.GetComponent<ProtectionComponent>();
                if (protA != null && sizeA.Size <= ECSController.Instance.Config.protectionSize)
                    protA.CollisionCount++;

                var protB = entityB.GetComponent<ProtectionComponent>();
                if (protB != null && sizeB.Size <= ECSController.Instance.Config.protectionSize)
                    protB.CollisionCount++;
            }
        }

        // Remove entities that have shrunk to size 0.
        List<Entity> toRemove = new List<Entity>();
        foreach (var entity in EntityManager.GetAllEntities())
        {
            var sizeComp = entity.GetComponent<SizeComponent>();
            if (sizeComp != null && sizeComp.Size <= 0)
            {
                ECSController.Instance.DestroyShape(entity.Id);
                toRemove.Add(entity);
            }
        }
        foreach (var entity in toRemove)
            EntityManager.RemoveEntity(entity.Id);
    }
}
