using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Net;
using UnityEngine.UIElements;
using static Config;

public class ExplosionSystem : ISystem
{

    public string Name => "ExplosionSystem";
    public void UpdateSystem()
    {
        var entities = World.GetEntitiesWith<SizeComponent, PositionComponent>();

        var entityList = new List<uint>(entities);

        for (int i = 0; i < entityList.Count; i++)
        {
            
            var entity = entityList[i];

            if (!World.HasComponent<CircleTypeComponent>(entity) ||
                !World.HasComponent<SizeComponent>(entity) ||
                !World.HasComponent<PositionComponent>(entity) ||
                !World.HasComponent<VelocityComponent>(entity))
            {
                Debug.LogWarning($"L'entité {entity} ne possède pas tous les composants requis.");
                continue;
            }

            var typeCircle = World.GetComponent<CircleTypeComponent>(entity).type;
            var size = World.GetComponent<SizeComponent>(entity);
            var position = World.GetComponent<PositionComponent>(entity);
            var velocity = World.GetComponent<VelocityComponent>(entity);


            if (typeCircle == CircleType.Static)
            {
                continue;
            }

            //taille critique
            var config = ECSController.Instance.Config;
            var explosionSize = config.explosionSize;

            if (size.radius >= explosionSize)
            {
                Debug.Log($"L entite {entity} a explose taille critique atteinte");
                World.RemoveEntity(entity);
                ECSController.Instance.DestroyShape(entity);

                //cercles resultants de l explosion de taille 1/4 du cercle d origine
                int newSize = Mathf.Max(size.radius / 4, 1);

                //directions
                Vector2[] directions =
                {
                new Vector2(1,1),
                new Vector2(-1,1),
                new Vector2(1,-1),
                new Vector2(-1,-1)
            };

                //On cree les nouveaux cercles
                foreach (var dir in directions)
                {
                    var newEntity = World.CreateEntity();
                    World.AddComponent(newEntity, new PositionComponent { position = position.position });
                    World.AddComponent(newEntity, new VelocityComponent { velocity = velocity.velocity + dir.normalized * 2 });
                    World.AddComponent(newEntity, new SizeComponent { radius = newSize });

                    World.AddComponent(newEntity, new CircleTypeComponent { type = CircleType.Dynamic });
                    //ECS
                    ECSController.Instance.CreateShape(newEntity, newSize);
                }
            }

        }
    }
}

