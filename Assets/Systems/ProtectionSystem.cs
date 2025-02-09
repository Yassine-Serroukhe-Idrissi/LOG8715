using UnityEngine;

/// <summary>
/// Manages protection for dynamic circles. When a circle (below a threshold size)
/// experiences enough collisions, it becomes protected for a duration, then enters cooldown.
/// </summary>
public class ProtectionSystem : ISystem
{
    public string Name => "ProtectionSystem";

    public void UpdateSystem()
    {
        foreach (var entity in EntityManager.GetEntitiesWith<ProtectionComponent, SizeComponent, CircleTypeComponent>())
        {
            var protComp = entity.GetComponent<ProtectionComponent>();
            var sizeComp = entity.GetComponent<SizeComponent>();
            var typeComp = entity.GetComponent<CircleTypeComponent>();

            if (typeComp.Type == CircleType.Static)
                continue;

            if (protComp.State == ProtectionState.Protected)
            {
                protComp.ProtectionTimer -= Time.deltaTime;
                if (protComp.ProtectionTimer <= 0)
                {
                    protComp.State = ProtectionState.Cooldown;
                    protComp.CooldownTimer = ECSController.Instance.Config.protectionCooldown;
                    ECSController.Instance.UpdateShapeColor(entity.Id, Color.yellow);
                    entity.AddComponent(new ColorComponent(Color.yellow));
                }
                else
                {
                    ECSController.Instance.UpdateShapeColor(entity.Id, Color.white);
                    entity.AddComponent(new ColorComponent(Color.white));
                }
            }
            else if (protComp.State == ProtectionState.Cooldown)
            {
                protComp.CooldownTimer -= Time.deltaTime;
                if (protComp.CooldownTimer <= 0)
                {
                    protComp.State = ProtectionState.None;
                    protComp.CollisionCount = 0;
                    ECSController.Instance.UpdateShapeColor(entity.Id, new Color(0.5f, 0.5f, 1f));
                    entity.AddComponent(new ColorComponent(new Color(0.5f, 0.5f, 1f)));
                }
                else
                {
                    ECSController.Instance.UpdateShapeColor(entity.Id, Color.yellow);
                    entity.AddComponent(new ColorComponent(Color.yellow));
                }
            }
            else if (protComp.State == ProtectionState.None)
            {
                if (sizeComp.Size <= ECSController.Instance.Config.protectionSize)
                {
                    if (protComp.CollisionCount >= ECSController.Instance.Config.protectionCollisionCount)
                    {
                        protComp.State = ProtectionState.Protected;
                        protComp.ProtectionTimer = ECSController.Instance.Config.protectionDuration;
                        ECSController.Instance.UpdateShapeColor(entity.Id, Color.white);
                        entity.AddComponent(new ColorComponent(Color.white));
                    }
                    else
                    {
                        ECSController.Instance.UpdateShapeColor(entity.Id, new Color(0.5f, 0.5f, 1f));
                        entity.AddComponent(new ColorComponent(new Color(0.5f, 0.5f, 1f)));

                    }
                }
                else
                {
                    ECSController.Instance.UpdateShapeColor(entity.Id, new Color(0f, 0f, 0.5f));
                    entity.AddComponent(new ColorComponent(new Color(0f, 0f, 0.5f)));
                }
            }
        }
    }
}
