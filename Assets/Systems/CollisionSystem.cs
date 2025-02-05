using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Net;
/// <summary>
/// identifier les collisions entre les cercles (si le rayon de deux cercles se touchent -> collision detectee)
/// gestion des rebonds apres collision (avec calculateCollision)
/// applique les maj des composants velocity et position
/// on supp ou on transforme les entites si certaines conditions de collision sont remplies 
/// </summary>
public class CollisionSystem :ISystem
{
    public string Name => "CollisionSystem";

    public void UpdateSystem()
    {
        //On recup les entites qui ont des composants Position et size
        var entities = World.GetEntitiesWith<PositionComponent, SizeComponent>();

        //on va convertir ces les entites en liste pour eviter les modifs pendant l ite 
        var entityList = new List<uint>(entities);

        // On parcourt chaque paire d entites pour detecter les collisions
        for(int i = 0; i<entityList.Count; i++)
        {
            for(int j=i+1; j < entityList.Count; j++)
            {
                var entityA = entityList[i];
                var entityB = entityList[j];

                if (!World.HasEntity(entityA) || !World.HasEntity(entityB))
                {
                    Debug.LogWarning($"Une ou les deux entités n'existent plus : entityA = {entityA}, entityB = {entityB}");
                    continue;
                }

                var positionA = World.GetComponent<PositionComponent>(entityA);
                var sizeA = World.GetComponent<SizeComponent>(entityA);
                var positionB = World.GetComponent<PositionComponent>(entityB);
                var sizeB = World.GetComponent<SizeComponent>(entityB);
                //on verif si les cercles se touchent
                float distance = Vector2.Distance(positionA.position, positionB.position);

                if (distance <= sizeA.radius + sizeB.radius)
                {
                    HandleCollision(entityA, entityB, positionA, positionB, sizeA, sizeB);
                }


            }
        }

    }

    private void HandleCollision(uint entityA, uint entityB, PositionComponent positionA,PositionComponent positionB, SizeComponent sizeA, SizeComponent sizeB)
    {
        var config = ECSController.Instance.Config;
        var protectionSize = config.protectionSize;
        var protectionCollisionCount = config.protectionCollisionCount;
        
        //verification de l existence des entite et des components
        if (!World.HasEntity(entityA) || !World.HasEntity(entityB))
        {
            Debug.LogWarning($"Une des entités n'existe plus avant HandleCollision : entityA = {entityA}, entityB = {entityB}");
            return;
        }
        //On verif dabord si les entites ont le composant Velocity
        if (!World.HasComponent<VelocityComponent>(entityA) || !World.HasComponent<VelocityComponent>(entityB))
        {
            Debug.LogWarning($"Une entite na pas de velocityComponent : entityA = {entityA}, entityB = {entityB}");
            return;
        }

        if (!World.HasComponent<CircleTypeComponent>(entityA) || !World.HasComponent<VelocityComponent>(entityB))
        {
            Debug.LogWarning($"Une entite na pas de TypeComponent : entityA = {entityA}, entityB = {entityB}");
            return;
        }

        //Recuperation des components
        //On recup le type du cercle
        var typeA = World.GetComponent<CircleTypeComponent>(entityA).type;
        var typeB = World.GetComponent<CircleTypeComponent>(entityB).type;

        //on recup la velocity des cercles 
        var velocityA = World.GetComponent<VelocityComponent>(entityA);
        var velocityB = World.GetComponent<VelocityComponent>(entityB);

        var result = CollisionUtility.CalculateCollision(positionA.position, velocityA.velocity, sizeA.radius, positionB.position, velocityB.velocity, sizeB.radius);

        if (result == null)
        {
            Debug.LogWarning($"Pas de collision détectée entre les entités {entityA} et {entityB}.");
            return;
        }

        //maj des vitesse et des position
        velocityA.velocity = result.velocity1;
        velocityB.velocity = result.velocity2;

        positionA.position = result.position1;
        positionB.position = result.position2;
        
        
        //on enregistre les modifs dans le world
        if (World.HasEntity(entityA))
        {
            World.SetComponent(entityA, velocityA);
            World.SetComponent(entityA, positionA);

            ECSController.Instance.UpdateShapePosition(entityA, positionA.position);
        }

        if (World.HasEntity(entityB))
        {
            World.SetComponent(entityB, velocityB);
            World.SetComponent(entityB, positionB);

            ECSController.Instance.UpdateShapePosition(entityB, positionB.position);
        }

        //AJOUTER DYNAMIQUE VS STATIQUE Si les deux sont colles  
        if (typeA == CircleType.Static || typeB == CircleType.Static)
        {
            return;
        }
        else //si les deux sont dyn : On chnage de taille apres la collision (si ils sont de meme taille on change pas)
        {
           
            //TAILLE 

            if (sizeA.radius > sizeB.radius)
            {
                sizeA.radius += 1;
                sizeB.radius -= 1;
            }
            else if (sizeB.radius > sizeA.radius)
            {
                sizeA.radius -= 1;
                sizeB.radius += 1;
            }

            //On supprime les cercles qui ont une taille de 0
            if (sizeA.radius <= 0)
            {
                Debug.Log($"l entite {entityA} supp car taille = 0");
                World.RemoveEntity(entityA);
                ECSController.Instance.DestroyShape(entityA);

            }
            else if (World.HasEntity(entityA))
            {
                World.SetComponent(entityA, sizeA);
                ECSController.Instance.UpdateShapeSize(entityA, sizeA.radius);
            }

            if (sizeB.radius <= 0)
            {
                Debug.Log($"Entite{entityB} supp car taille = 0");
                World.RemoveEntity(entityB);
                ECSController.Instance.DestroyShape(entityB);

            }
            else if (World.HasEntity(entityB))
            {
                World.SetComponent(entityB, sizeB);
                ECSController.Instance.UpdateShapeSize(entityB, sizeB.radius);
            }

            
        }
        
      
    }

}
