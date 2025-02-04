using UnityEngine;

/// <summary>
/// Sets the color of circles based on their state:
/// - Static circles are rendered in red.
/// - Dynamic circles are rendered in a default dark blue,
///   with other systems (like protection or collision) overriding this as needed.
/// </summary>
public class RenderSystem : ISystem
{
    public string Name => "RenderSystem";

    public void UpdateSystem()
    {
        foreach (var entity in EntityManager.GetEntitiesWith<CircleTypeComponent, SizeComponent>())
        {
            var typeComp = entity.GetComponent<CircleTypeComponent>();

            if (typeComp.Type == CircleType.Static)
            {
                ECSController.Instance.UpdateShapeColor(entity.Id, Color.red);
            }
            else
            {
                ECSController.Instance.UpdateShapeColor(entity.Id, new Color(0f, 0f, 0.5f));
            }
        }
    }
}
