using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap{ get; private set;} //public getter and private setter, because other sripts may want to use getter but shouldn't use setter

    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlagged;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;
   
    private void Awake() // built in function whenever our script initialize
    {
        tilemap = GetComponent<Tilemap>();
    }
    public void Draw(Cell[,] state) // , in [] means that it is 2d
    {
        int width = state.GetLength(0); // 0 and 1 determines which dimension length we are getting
        int height = state.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
               tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }
   private Tile GetTile(Cell cell)
    {
        if(cell.revealed)
        {
            return GetRevealedTile(cell);   
        }
        else if (cell.flagged)
        {
            return tileFlagged;
        } 
        else
        {
            return tileUnknown;
        }
    }
    private Tile GetRevealedTile(Cell cell)
    {
        switch(cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? tileExploded : tileMine;
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }
    private Tile GetNumberTile(Cell cell)
    {
        switch(cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }

    }
}
