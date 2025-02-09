using UnityEngine;

public class ColorComponent : IComponent
{
    public Color Color;

    public ColorComponent(Color color)
    {
        Color = color;
    }
}
