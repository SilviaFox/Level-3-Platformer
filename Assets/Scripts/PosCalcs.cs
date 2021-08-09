using UnityEngine;

public class PosCalcs
{
    public static Vector3Int Round(Vector3 input)
    {
        // Round each point of vector to int
        return new Vector3Int(Mathf.RoundToInt(input.x),Mathf.RoundToInt(input.y),Mathf.RoundToInt(input.z));
    }

    public static Vector3Int Floor(Vector3 input)
    {
        // Floor each point of vector to int
        return new Vector3Int(Mathf.FloorToInt(input.x),Mathf.FloorToInt(input.y),Mathf.FloorToInt(input.z));
    }

    public static Vector3Int Ceil(Vector3 input)
    {
        // Ceiling each point of vector to int
        return new Vector3Int(Mathf.CeilToInt(input.x),Mathf.CeilToInt(input.y),Mathf.CeilToInt(input.z));
    }

    public static Vector3 Clamp(Vector3 input, float min, float max)
    {
        return new Vector3(Mathf.Clamp(input.x, min,max), Mathf.Clamp(input.y, min, max), Mathf.Clamp(input.z, min, max));
    }

    public static float GetDistance(Vector2 posA, Vector2 posB) {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);
        
        return distance;
    }
}
