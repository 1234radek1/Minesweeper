using UnityEngine;

public struct Cell
{
    public enum Type
    {
        Invalid,
        Empty,
        Mine,
        Number,
    }
    public Vector3Int position; // Vector3 is a float,  because we are using tilemap we should use just Inteeger
    public Type type;
    public int number;
    public bool revealed;
    public bool flagged;
    public bool exploded;


}
