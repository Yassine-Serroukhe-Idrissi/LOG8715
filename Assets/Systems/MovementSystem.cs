using UnityEngine;

/// <summary>
/// Updates the position of dynamic circles by adding their velocity.
/// </summary>
public class MovementSystem : ISystem
{
    public string Name => "MovementSystem";

    public void UpdateSystem()
    {
        foreach (var entity in EntityManager.GetEntitiesWith<PositionComponent, VelocityComponent>())
        {
            var typeComp = entity.GetComponent<CircleTypeComponent>();
            // Only update dynamic circles.
            if (typeComp != null && typeComp.Type == CircleType.Static)
                continue;

            var posComp = entity.GetComponent<PositionComponent>();
            var velComp = entity.GetComponent<VelocityComponent>();

            posComp.Position += velComp.Velocity * Time.deltaTime;
            ECSController.Instance.UpdateShapePosition(entity.Id, posComp.Position);
        }
    }
}
