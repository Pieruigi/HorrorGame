using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int size = 20; // Mappa quadrata (20x20)
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    public float cellSize = 1.5f;

    private int[,] grid; // 0 = muro, 1 = corridoio

    void Start()
    {
        Generate();
        Build();
    }

    void Generate()
    {
        grid = new int[size, size];

        // 1) GENERA STRUTTURA BASE PAC-MAN (griglia di corridoi sempre collegati)
        for (int y = 1; y < size - 1; y++)
        {
            for (int x = 1; x < size - 1; x++)
            {
                // Alterna corridoi e muri per formare rete tipo pacman
                bool corridor = (x % 2 == 1) || (y % 2 == 1);
                grid[x, y] = corridor ? 1 : 0;
            }
        }

        // 2) RIPULISCI VICOLI CIECHI SE SI CREANO
        RemoveDeadEnds();

        // 3) ASSICURA almeno un muro lungo ≥ 5
        EnsureLongWall(5);
    }

    void Build()
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, y * cellSize);

                if (grid[x, y] == 1)
                    Instantiate(floorPrefab, pos, Quaternion.identity, transform);
                else
                    Instantiate(wallPrefab, pos, Quaternion.identity, transform);
            }
        }
    }

    // -------------------------------------------------------------------------
    //  RIMOZIONE VICOLI CIECHI
    // -------------------------------------------------------------------------
    void RemoveDeadEnds()
    {
        bool changed = true;

        while (changed)
        {
            changed = false;

            for (int y = 1; y < size - 1; y++)
            {
                for (int x = 1; x < size - 1; x++)
                {
                    if (grid[x, y] == 1) // Corridoio
                    {
                        int count =
                            (grid[x + 1, y] == 1 ? 1 : 0) +
                            (grid[x - 1, y] == 1 ? 1 : 0) +
                            (grid[x, y + 1] == 1 ? 1 : 0) +
                            (grid[x, y - 1] == 1 ? 1 : 0);

                        // Vicolo cieco → lo chiudo (diventa muro)
                        if (count <= 1)
                        {
                            grid[x, y] = 0;
                            changed = true;
                        }
                    }
                }
            }
        }
    }

    // -------------------------------------------------------------------------
    //  ASSICURARE ALMENO UN MURO LUNGO
    // -------------------------------------------------------------------------
    void EnsureLongWall(int length)
    {
        // Cerca muri orizzontali lunghi enough
        for (int y = 0; y < size; y++)
        {
            int run = 0;
            for (int x = 0; x < size; x++)
            {
                if (grid[x, y] == 0) run++;
                else run = 0;

                if (run >= length) return; // Già presente
            }
        }

        // Se non trovato → crea un muro lungo manualmente
        int row = Random.Range(2, size - 2);
        int start = Random.Range(1, size - length - 1);

        for (int i = 0; i < length; i++)
            grid[start + i, row] = 0;

        // ripulisci eventuali corridoi isolati
        RemoveDeadEnds();
    }

    // -------------------------------------------------------------------------
    //  UTILITY: Trova tutti i segmenti di muro lunghi
    // -------------------------------------------------------------------------
    public List<List<Vector2Int>> GetLongWalls(int minLength)
    {
        var result = new List<List<Vector2Int>>();

        // Only horizontal walls for now
        for (int y = 0; y < size; y++)
        {
            List<Vector2Int> segment = new List<Vector2Int>();

            for (int x = 0; x < size; x++)
            {
                if (grid[x, y] == 0)
                {
                    segment.Add(new Vector2Int(x, y));
                }
                else
                {
                    if (segment.Count >= minLength)
                        result.Add(new List<Vector2Int>(segment));

                    segment.Clear();
                }
            }

            if (segment.Count >= minLength)
                result.Add(segment);
        }

        return result;
    }
}
