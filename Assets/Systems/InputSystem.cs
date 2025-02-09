using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles user input:
/// - Pressing Space triggers a rewind (if not on cooldown).
/// - Mouse clicks on a dynamic circle either trigger an explosion (if size ≥ 4) or destroy the circle (if size < 4).
/// </summary>
public class InputSystem : ISystem
{
    public string Name => "InputSystem";
    private float _rewindCooldownTimer = 0f;
    private const float RewindCooldownDuration = 3f;

    public void UpdateSystem()
    {
        // Handle Space key for rewinding the simulation.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_rewindCooldownTimer <= 0f)
            {
                StateHistorySystem.Instance.Rewind(3f);
                _rewindCooldownTimer = RewindCooldownDuration;
            }
            else
            {
                Debug.Log("Rewind is on cooldown");
            }
        }
        if (_rewindCooldownTimer > 0f)
            _rewindCooldownTimer -= Time.deltaTime;

        // Handle mouse click input.
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (var entity in EntityManager.GetEntitiesWith<PositionComponent, SizeComponent, CircleTypeComponent, VelocityComponent>())
            {
                var posComp = entity.GetComponent<PositionComponent>();
                var sizeComp = entity.GetComponent<SizeComponent>();
                var typeComp = entity.GetComponent<CircleTypeComponent>();

                if (typeComp.Type == CircleType.Static)
                    continue;

                float radius = sizeComp.Size / 2f;
                if (Vector2.Distance(mousePos, posComp.Position) <= radius)
                {
                    // If size ≥ 4, explode the circle; otherwise, destroy it.
                    if (sizeComp.Size >= 4)
                        ExplodeEntity(entity);
                    else
                    {
                        ECSController.Instance.DestroyShape(entity.Id);
                        EntityManager.RemoveEntity(entity.Id);
                    }
                    break;
                }
            }
        }
    }

    private void ExplodeEntity(Entity entity)
    {
        var posComp = entity.GetComponent<PositionComponent>();
        var sizeComp = entity.GetComponent<SizeComponent>();
        var velComp = entity.GetComponent<VelocityComponent>();
        int originalSize = sizeComp.Size;
        int newSize = Mathf.Max(1, originalSize / 4);

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

           newEntity.AddComponent(new ColorComponent(Color.magenta));

            ECSController.Instance.CreateShape(newEntity.Id, newSize);
            ECSController.Instance.UpdateShapePosition(newEntity.Id, posComp.Position);
            ECSController.Instance.UpdateShapeColor(newEntity.Id, Color.magenta); // pink color
        }
        ECSController.Instance.DestroyShape(entity.Id);
        EntityManager.RemoveEntity(entity.Id);
    }
}
