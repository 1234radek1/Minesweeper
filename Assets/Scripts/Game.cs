using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int width = 16; //default values
    public int height = 16;
    public int mineCount = 32;

    private bool gameover;
    private Board board;
    private Cell[,] state;

    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }
    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
    }
    private void NewGame()
    {
        state = new Cell[width, height];
        gameover = false;
        GenerateCells();
        GenerateMines();
        GenerateNumbers();
        board.Draw(state);
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
    }

    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0); // ideally vector 2d int but tilemap expects vector3d
                cell.type = Cell.Type.Empty;
                state[x,y] = cell;
            }
        }
      
    }
    private void GenerateMines()
    {
        for(int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            while(state[x, y].type == Cell.Type.Mine)
            {
                x++;
                if(x>= width) // so we dont get over the edge of the board
                {
                    x = 0;
                    y++;
                    if(y >= height)
                    {
                        y = 0;
                    }
                }
            }
           
            state[x, y].type = Cell.Type.Mine;

        }
    }
    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    continue;
                }
                cell.number = CountMines(x, y);
                if(cell.number > 0 )
                {
                    cell.type = Cell.Type.Number;
                }
             
                state[x, y] = cell;
            }
        }
    }
    private int CountMines(int cellX, int cellY)
    {
        int count = 0;
        for (int adjacentX = -1; adjacentX <= 1; adjacentX++) // loop goes through adjecent cells to verify if there is a mine so we can get to know what number should be displayed
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }
                int x = cellX + adjacentX;
                int y = cellY + adjacentY;
              
                if (GetCell(x,y).type == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            NewGame(); // restart
        }
        if (!gameover)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Flag();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }
        }


    }

    private void Flag()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // conversion from screen space to world space
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
        if(cell.type == Cell.Type.Invalid || cell.revealed)
        {
            return;
        }
        cell.flagged = !cell.flagged; // it sets the opposite
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);
    }
    private void Reveal()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // conversion from screen space to world space
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)
        {
            return;
        }
        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell); break;
            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            default:
                cell.revealed = true;
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }

        if(cell.type == Cell.Type.Empty)
        {
            Flood(cell);
        }
        cell.revealed = true;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);

    }
    private void Explode(Cell cell)
    {
        Debug.Log("Game Over");
        gameover = true;
        cell.revealed = true;
        cell.exploded = true;
        state[cell.position.x, cell.position.y] = cell;

        for (int x = 0; x < width; x++)
        { 
            for (int y = 0; y < height; y++)
            {
                cell = state[x,y];
                if(cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x,y] = cell;
                }
            }
        }

    }
    private void Flood(Cell cell)
    {
        if (cell.revealed) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;
        if (cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
        }
    }

    private Cell GetCell(int x, int y)
    {
        if(IsValid(x,y))
        {
            return state[x,y];
        }
        else
        {
            return new Cell(); 
        }
    }
    private void CheckWinCondition()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if(cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return;
                }
            }
        }
        Debug.Log("Winner");
        gameover = true; // so we can no longer press anything if we won

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }
            }
        }

    }
    private bool IsValid(int x , int y)
    {
        return x >= 0 && x < width && y >=0 && y < height;
       
    }

}
