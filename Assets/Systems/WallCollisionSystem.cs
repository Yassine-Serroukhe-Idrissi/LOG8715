using UnityEngine;

/// <summary>
/// Checks if circles have hit the screen boundaries and reverses their velocity accordingly.
/// </summary>
public class WallCollisionSystem : ISystem
{
    public string Name => "WallCollisionSystem";

    public void UpdateSystem()
    {
        // Convert screen corners to world coordinates.
        Vector2 minBounds = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        Vector2 maxBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        foreach (var entity in EntityManager.GetEntitiesWith<PositionComponent, VelocityComponent, SizeComponent, CircleTypeComponent>())
        {
            var typeComp = entity.GetComponent<CircleTypeComponent>();
            if (typeComp.Type == CircleType.Static)
                continue;

            var posComp = entity.GetComponent<PositionComponent>();
            var velComp = entity.GetComponent<VelocityComponent>();
            var sizeComp = entity.GetComponent<SizeComponent>();

            float radius = sizeComp.Size / 2f;

            // Left/right boundaries.
            if (posComp.Position.x - radius < minBounds.x && velComp.Velocity.x < 0)
                velComp.Velocity = new Vector2(-velComp.Velocity.x, velComp.Velocity.y);
            else if (posComp.Position.x + radius > maxBounds.x && velComp.Velocity.x > 0)
                velComp.Velocity = new Vector2(-velComp.Velocity.x, velComp.Velocity.y);

            // Top/bottom boundaries.
            if (posComp.Position.y - radius < minBounds.y && velComp.Velocity.y < 0)
                velComp.Velocity = new Vector2(velComp.Velocity.x, -velComp.Velocity.y);
            else if (posComp.Position.y + radius > maxBounds.y && velComp.Velocity.y > 0)
                velComp.Velocity = new Vector2(velComp.Velocity.x, -velComp.Velocity.y);
        }
    }
}
