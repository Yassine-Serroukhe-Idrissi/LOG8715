//Enumeration de type de cercle
public enum CircleType
{
    Static,
    Dynamic
}

public class CircleTypeComponent : IComponent
{
    public CircleType Type;

    //Constructeur
    public CircleTypeComponent(CircleType t)
    {
        Type = t;
    }
}
