using Unity.Entities;
using UnityEditor.Experimental.GraphView;
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
        // Iterate over entities that have CircleTypeComponent and SizeComponent.
        foreach (var entity in EntityManager.GetEntitiesWith<CircleTypeComponent, SizeComponent>())
        {
            var typeComp = entity.GetComponent<CircleTypeComponent>();
            var colorComp = entity.GetComponent<ColorComponent>();
            var sizeComp = entity.GetComponent<SizeComponent>();

            Color newColor;
            if (colorComp != null)
            {
                // Use the stored color (e.g., pink for exploded fragments).
                newColor = colorComp.Color;
            }
            else
            {
                // Assign a default color based on the circle type.
                if (typeComp.Type == CircleType.Static)
                    newColor = Color.red;
                else if (typeComp.Type == CircleType.Dynamic)
                    if (sizeComp.Size == (ECSController.Instance.Config.explosionSize - 1))
                        newColor = new Color(1.0f, 0.64f, 0.0f);
                    else
                        newColor = new Color(0f, 0f, 0.5f); // Default dark blue.
                else
                    newColor = Color.white;
            }

            ECSController.Instance.UpdateShapeColor(entity.Id, newColor);
            entity.AddComponent(new ColorComponent(newColor));
        }
    }

}
