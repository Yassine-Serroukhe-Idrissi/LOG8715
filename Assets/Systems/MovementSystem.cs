using UnityEngine;
using UnityEngine.LightTransport;

/// <summary>
/// Récupère toutes les entités qui ont PositionComponent et VelocityComponent. 
/// MAJ de leur position en fonction de leur vitesse
/// Applique les nouvelles positions dans le ECSController.UpdateShapePosition() pour la MAJ de l'affichage
/// </summary>


public class MovementSystem : ISystem
{
    public string Name => "MovementSystem";

    public void UpdateSystem()
    {

        float screenTop = Camera.main.orthographicSize;
        float screenBottom = -Camera.main.orthographicSize;
        float screenRight = Camera.main.orthographicSize * Camera.main.aspect;
        float screenLeft = -screenRight;

        foreach (var entity in World.GetEntitiesWith<PositionComponent, VelocityComponent>())
        {
            if (!World.HasEntity(entity))
            {
                Debug.LogWarning($"L'entité {entity} n'existe plus dans le World.");
                continue;
            }

            var position = World.GetComponent<PositionComponent>(entity);
            var velocity = World.GetComponent<VelocityComponent>(entity);
            var size = World.GetComponent<SizeComponent>(entity);

            position.position += velocity.velocity * Time.deltaTime; //maj de la pos avec velocity

            float radius = size.radius;

            //collision sur les bords verticaux de l ecran
            if(position.position.x - (radius/2) <=screenLeft || position.position.x+ (radius / 2) >= screenRight)
            {
                velocity.velocity = new Vector2(-velocity.velocity.x, velocity.velocity.y);
                position.position = new Vector2(Mathf.Clamp(position.position.x, screenLeft, screenRight), position.position.y);
            }

            //collisions des bords horizontaux 
            if(position.position.y- (radius / 2) <= screenBottom || position.position.y+ (radius / 2) >= screenTop)
            {
                velocity.velocity = new Vector2(velocity.velocity.x, -velocity.velocity.y);
                position.position = new Vector2(position.position.x, Mathf.Clamp(position.position.y, screenBottom, screenTop));
            }

            //collision sur les bords verticaux de l ecran
            if (position.position.x <= screenLeft || position.position.x >= screenRight)
            {
                velocity.velocity = new Vector2(-velocity.velocity.x, velocity.velocity.y);
                position.position = new Vector2(Mathf.Clamp(position.position.x, screenLeft, screenRight), position.position.y);
            }

            //collisions des bords horizontaux 
            if (position.position.y  <= screenBottom || position.position.y >= screenTop)
            {
                velocity.velocity = new Vector2(velocity.velocity.x, -velocity.velocity.y);
                position.position = new Vector2(position.position.x, Mathf.Clamp(position.position.y, screenBottom, screenTop));
            }

            if (World.HasEntity(entity))
            {
                World.SetComponent(entity, position); //Enregistrement de la nouvelle pos
                World.SetComponent(entity, velocity);

                if (World.HasEntity(entity))
                {
                    ECSController.Instance.UpdateShapePosition(entity, position.position); //MAJ UNITY
                }
               
            };
            
        }
    }
}
