using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// When a dynamic circle’s size reaches or exceeds the explosion threshold,
/// it explodes into four smaller circles that move in diagonal directions.
/// </summary>
public class ExplosionSystem : ISystem
{
    public string Name => "ExplosionSystem";

    public void UpdateSystem()
    {
        List<Entity> toExplode = new List<Entity>();
        foreach (var entity in EntityManager.GetEntitiesWith<SizeComponent, CircleTypeComponent, PositionComponent, VelocityComponent>())
        {
            var sizeComp = entity.GetComponent<SizeComponent>();
            var typeComp = entity.GetComponent<CircleTypeComponent>();
            if (typeComp.Type == CircleType.Static)
                continue;

            if (sizeComp.Size >= ECSController.Instance.Config.explosionSize)
                toExplode.Add(entity);
        }

        foreach (var entity in toExplode)
        {
            var posComp = entity.GetComponent<PositionComponent>();
            var sizeComp = entity.GetComponent<SizeComponent>();
            var velComp = entity.GetComponent<VelocityComponent>();
            int originalSize = sizeComp.Size;
            int newSize = Mathf.Max(1, originalSize / 4);

            // Define four diagonal directions.
            Vector2[] directions = new Vector2[]
            {
                new Vector2(1, 1).normalized,
                new Vector2(-1, 1).normalized,
                new Vector2(1, -1).normalized,
                new Vector2(-1, -1).normalized
            };

            foreach (var dir in directions)
            {
                Entity newEntity = EntityManager.CreateEntity();
                newEntity.AddComponent(new PositionComponent(posComp.Position));
                newEntity.AddComponent(new VelocityComponent(dir * velComp.Velocity.magnitude));
                newEntity.AddComponent(new SizeComponent(newSize));
                newEntity.AddComponent(new CircleTypeComponent(CircleType.Dynamic));
                newEntity.AddComponent(new ProtectionComponent());

                ECSController.Instance.CreateShape(newEntity.Id, newSize);
                ECSController.Instance.UpdateShapePosition(newEntity.Id, posComp.Position);
                // Set initial color to pink.
                ECSController.Instance.UpdateShapeColor(newEntity.Id, new Color(1f, 0.75f, 0.8f));
            }

            ECSController.Instance.DestroyShape(entity.Id);
            EntityManager.RemoveEntity(entity.Id);
        }
    }
}
