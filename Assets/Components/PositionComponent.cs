using UnityEngine;

public class PositionComponent : IComponent
{
    public Vector2 Position;

    //Constructor
    public PositionComponent(Vector2 p)
    {
        Position = p;
    }

}
