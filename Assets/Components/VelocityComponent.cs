using UnityEngine;

public class VelocityComponent : IComponent 
{
    public Vector2 Velocity;

    //Constructer

    public VelocityComponent(Vector2 v)
    {
        Velocity = v;
    }
}