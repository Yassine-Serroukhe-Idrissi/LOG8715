using UnityEngine;

/// <summary>
/// In the left half of the screen, the simulation is updated 4× faster.
/// This system applies extra movement updates for circles located in that region.
/// </summary>
public class TimeAccelerationSystem : ISystem
{
    public string Name => "TimeAccelerationSystem";

    public void UpdateSystem()
    {
        // Determine the world coordinate for the screen's midpoint.
        Vector2 midPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, 0));
        float midX = midPoint.x;

        foreach (var entity in EntityManager.GetEntitiesWith<PositionComponent, VelocityComponent, CircleTypeComponent>())
        {
            var posComp = entity.GetComponent<PositionComponent>();
            var velComp = entity.GetComponent<VelocityComponent>();
            var typeComp = entity.GetComponent<CircleTypeComponent>();

            if (typeComp.Type == CircleType.Static)
                continue;

            if (posComp.Position.x < midX)
            {
                // Apply three additional updates (total 4× update).
                for (int i = 0; i < 3; i++)
                {
                    posComp.Position += velComp.Velocity * Time.deltaTime;
                }
                ECSController.Instance.UpdateShapePosition(entity.Id, posComp.Position);
            }
        }
    }
}
