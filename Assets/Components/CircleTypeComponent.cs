using UnityEngine;

public enum CircleType
{
    Static,
    Dynamic
}
public struct CircleTypeComponent : IComponent
{
    public CircleType type;
}
