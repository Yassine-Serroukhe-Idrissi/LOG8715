// CollisionSystem.cs
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem
{
    private readonly ComponentManager componentManager;

    public CollisionSystem(ComponentManager cm)
    {
        componentManager = cm;
    }

    public void Update()
    {
        var dynamicEntities = new List<int>(componentManager.GetEntitiesWithComponents(
            typeof(PositionComponent), typeof(VelocityComponent), typeof(SizeComponent)));

        var staticEntities = new List<int>(componentManager.GetEntitiesWithComponents(
            typeof(PositionComponent), typeof(SizeComponent), typeof(IsStatic)));

        CheckCollisions(dynamicEntities, dynamicEntities); // Dynamic vs Dynamic
        CheckCollisions(dynamicEntities, staticEntities);  // Dynamic vs Static
    }

    private void CheckCollisions(List<int> groupA, List<int> groupB)
    {
        for (int i = 0; i < groupA.Count; i++)
        {
            for (int j = (groupA == groupB ? i + 1 : 0); j < groupB.Count; j++)
            {
                int entityA = groupA[i];
                int entityB = groupB[j];

                if (entityA == entityB) continue;
                ResolveCollision(entityA, entityB);
            }
        }
    }

    private void ResolveCollision(int entityA, int entityB)
    {
        bool isStaticA = componentManager.HasComponent<IsStatic>(entityA);
        bool isStaticB = componentManager.HasComponent<IsStatic>(entityB);

        // Récupération des composants
        var posA = componentManager.GetComponent<PositionComponent>(entityA);
        var posB = componentManager.GetComponent<PositionComponent>(entityB);

        var velA = isStaticA ? Vector2.zero : new Vector2(
            componentManager.GetComponent<VelocityComponent>(entityA).dx,
            componentManager.GetComponent<VelocityComponent>(entityA).dy
        );

        var velB = isStaticB ? Vector2.zero : new Vector2(
            componentManager.GetComponent<VelocityComponent>(entityB).dx,
            componentManager.GetComponent<VelocityComponent>(entityB).dy
        );

        var sizeA = componentManager.GetComponent<SizeComponent>(entityA).radius * 2;
        var sizeB = componentManager.GetComponent<SizeComponent>(entityB).radius * 2;

        // Calcul de la collision
        var result = CollisionUtility.CalculateCollision(
            new Vector2(posA.x, posA.y), velA, sizeA,
            new Vector2(posB.x, posB.y), velB, sizeB
        );

        if (result == null) return;

        // Mise à jour des positions et vitesses
        if (!isStaticA)
        {
            componentManager.AddComponent(entityA, new PositionComponent
            {
                x = result.position1.x,
                y = result.position1.y
            });

            componentManager.AddComponent(entityA, new VelocityComponent
            {
                dx = result.velocity1.x,
                dy = result.velocity1.y
            });
        }

        if (!isStaticB)
        {
            componentManager.AddComponent(entityB, new PositionComponent
            {
                x = result.position2.x,
                y = result.position2.y
            });

            componentManager.AddComponent(entityB, new VelocityComponent
            {
                dx = result.velocity2.x,
                dy = result.velocity2.y
            });
        }

        // Ajustement des tailles (uniquement entre dynamiques)
        if (!isStaticA && !isStaticB)
        {
            var sizeCompA = componentManager.GetComponent<SizeComponent>(entityA);
            var sizeCompB = componentManager.GetComponent<SizeComponent>(entityB);

            if (sizeCompA.radius != sizeCompB.radius)
            {
                if (sizeCompA.radius > sizeCompB.radius)
                {
                    sizeCompA.radius++;
                    sizeCompB.radius--;
                }
                else
                {
                    sizeCompA.radius--;
                    sizeCompB.radius++;
                }

                componentManager.AddComponent(entityA, sizeCompA);
                componentManager.AddComponent(entityB, sizeCompB);
            }
        }
    }
}