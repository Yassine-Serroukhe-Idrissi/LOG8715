using System.Collections.Generic;


public class MovementSystem 
{
    private readonly ComponentManager componentManager;

    public MovementSystem(ComponentManager cm) => componentManager = cm;

    public void UpdateSystem()
    {
        var entities = componentManager.GetEntitiesWithComponents(typeof(PositionComponent), typeof(VelocityComponent));
        foreach (var entity in entities)
        {
            var pos = componentManager.GetComponent<PositionComponent>(entity);
            var vel = componentManager.GetComponent<VelocityComponent>(entity);

            pos.x += vel.dx * Time.deltaTime;
            pos.y += vel.dy * Time.deltaTime;

            componentManager.AddComponent(entity, pos);
        }
    }

    public string Name => "Movement System";
}