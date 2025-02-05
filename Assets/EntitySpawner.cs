using UnityEngine;

public class EntitySpawner :MonoBehaviour
{

    
    void Start()
    {
        

        for (int i = 0; i < ECSController.Instance.Config.circleInstancesToSpawn.Count; i++)
        {
            var entity = World.CreateEntity();
            World.AddComponent(entity, new PositionComponent { position = new Vector2(i * 2, 0) });
            World.AddComponent(entity, new VelocityComponent { velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) });
            World.AddComponent(entity, new SizeComponent { radius = Random.Range(8, 12) }); // Taille proche de l'explosionSize
            ECSController.Instance.CreateShape(entity, Random.Range(8, 12));
            }


            //var entity = World.CreateEntity();

            //on ajoute les compo a l entite 
            //World.AddComponent(entity, new PositionComponent { position = new Vector2(0, 0) });
            // World.AddComponent(entity, new VelocityComponent { velocity = new Vector2(1, 0) });
            // World.AddComponent(entity, new SizeComponent { radius = 5 });
            //World.AddComponent(entity, new ColorComponent { color = Color.green });

            //cree la forme dans unity avec ECSController
            //ECSController.Instance.CreateShape(entity, 5);
            //ECSController.Instance.UpdateShapeColor(entity, Color.green);


        }
    }
