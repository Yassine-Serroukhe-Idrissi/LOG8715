using UnityEngine;

public class InitializationSystem : ISystem
{
    public string Name => "InitializationSystem";
    private bool _initialized = false;

    public void UpdateSystem()
    {
        if (_initialized) return;

        var config = ECSController.Instance.Config;

        foreach(var shapeConfig in config.circleInstancesToSpawn)
        {
            var entity = World.CreateEntity();

            World.AddComponent(entity, new PositionComponent { position = shapeConfig.initialPosition });
            World.AddComponent(entity, new VelocityComponent { velocity = shapeConfig.initialVelocity });
            World.AddComponent(entity, new SizeComponent { radius = shapeConfig.initialSize });
            World.AddComponent(entity, new StateComponent { state = State.None });
            //Ajout du protection component 

            //Ajout du tye de cercle
            var isStatic = shapeConfig.initialVelocity == Vector2.zero;
            World.AddComponent(entity, new CircleTypeComponent { type = isStatic ? CircleType.Static : CircleType.Dynamic });


            //lien avec unity visuel
            ECSController.Instance.CreateShape(entity, shapeConfig.initialSize);
            ECSController.Instance.UpdateShapePosition(entity, shapeConfig.initialPosition);
        }
        _initialized = true;
    }
    
}
