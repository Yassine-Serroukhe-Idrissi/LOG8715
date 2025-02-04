using UnityEngine;

/// <summary>
/// Reads the configuration and creates the initial entities (circles) for the simulation.
/// This version always adds a VelocityComponent (using a zero vector for static circles),
/// so that both dynamic and static circles are processed by the collision system.
/// </summary>
public class InitializationSystem : ISystem
{
    public string Name => "InitializationSystem";
    private bool _initialized = false;

    public void UpdateSystem()
    {
        if (_initialized)
            return;

        var config = ECSController.Instance.Config;
        foreach (var shapeConfig in config.circleInstancesToSpawn)
        {
            // Create a new entity.
            Entity entity = EntityManager.CreateEntity();

            // Add the common components.
            entity.AddComponent(new PositionComponent(shapeConfig.initialPosition));
            entity.AddComponent(new SizeComponent(shapeConfig.initialSize));
            entity.AddComponent(new ProtectionComponent());

            // Always add a VelocityComponent.
            // For static circles, this will be a zero vector.
            entity.AddComponent(new VelocityComponent(shapeConfig.initialVelocity));

            // Add the CircleTypeComponent based on the initial velocity.
            if (shapeConfig.initialVelocity == Vector2.zero)
            {
                entity.AddComponent(new CircleTypeComponent(CircleType.Static));
            }
            else
            {
                entity.AddComponent(new CircleTypeComponent(CircleType.Dynamic));
            }

            // Create a visual representation.
            ECSController.Instance.CreateShape(entity.Id, shapeConfig.initialSize);
            ECSController.Instance.UpdateShapePosition(entity.Id, shapeConfig.initialPosition);
        }
        _initialized = true;
    }
}
