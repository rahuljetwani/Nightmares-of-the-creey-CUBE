using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GroundGenerator : MonoBehaviour
{
    [Header("Tilemap Setup")]
    public Tilemap tilemap;
    public TileBase groundTile;

    [Header("Chunk Settings")]
    public int chunkWidth = 10;
    public int minStepHeight = 1;
    public int maxStepHeight = 8;
    public int baseLayers = 4;

    [Header("Start Point")]
    public Vector2Int startPoint = Vector2Int.zero;

    [Header("Player")]
    public Transform player;

    [Header("Generation Settings")]
    public int chunksVisibleAhead = 5;
    public int chunksBehindToKeep = 2;

    [Header("Surface Bumps")]
    [Range(0f, 1f)]
    public float bumpChance = 0.15f;

    [Header("Spikes")]
    public GameObject spikePrefab;
    [Range(0f, 1f)]
    public float spikeChance = 0.2f;
    public float spikeYOffset = 1.0f;

    [Header("Termites")]
    public GameObject termitePrefab;
    [Range(0f, 1f)]
    public float termiteChance = 0.1f; 
    public float termiteYOffset = 0.5f;

    private int currentHeight = 2;
    private int currentChunkIndex = 0;
    private Queue<Vector2Int> activeChunks = new Queue<Vector2Int>();

    private int flatCounter = 0;
    public int maxFlatChunks = 3;

    void Start()
    {
        Vector2Int startChunk = new Vector2Int(0, currentHeight);
        activeChunks.Enqueue(startChunk);

        for (int i = 0; i < chunksVisibleAhead; i++)
        {
            GenerateChunk(currentChunkIndex);
            currentChunkIndex++;
        }
    }

    void Update()
    {
        float playerX = player.position.x;
        int playerChunkIndex = Mathf.FloorToInt((playerX - startPoint.x) / chunkWidth);

        while (currentChunkIndex < playerChunkIndex + chunksVisibleAhead)
        {
            GenerateChunk(currentChunkIndex);
            currentChunkIndex++;
        }

        while (activeChunks.Count > chunksVisibleAhead + chunksBehindToKeep)
        {
            Vector2Int oldChunk = activeChunks.Dequeue();
            ClearChunk(oldChunk);
        }
    }

    void GenerateChunk(int chunkIndex)
    {
        int heightChange = 0;
        float roll = Random.value;

        if (flatCounter >= maxFlatChunks)
        {
            heightChange = Random.value < 0.5f ? 1 : -1;
            flatCounter = 0;
        }
        else
        {
            if (roll < 0.5f)
                heightChange = Random.Range(-1, 2);
            else if (roll < 0.8f)
                heightChange = Random.Range(-2, 2);
            else
                heightChange = 0;

            if (heightChange == 0)
                flatCounter++;
            else
                flatCounter = 0;
        }

        currentHeight += heightChange;
        currentHeight = Mathf.Clamp(currentHeight, minStepHeight, maxStepHeight);

        int startX = startPoint.x + chunkIndex * chunkWidth;

        for (int x = startX; x < startX + chunkWidth; x++)
        {
            for (int y = 0; y < currentHeight; y++)
            {
                tilemap.SetTile(new Vector3Int(x, startPoint.y + y, 0), groundTile);
            }

            if (Random.value < bumpChance)
            {
                tilemap.SetTile(new Vector3Int(x, startPoint.y + currentHeight, 0), groundTile);
            }

            for (int b = -1; b >= -baseLayers; b--)
            {
                tilemap.SetTile(new Vector3Int(x, startPoint.y + b, 0), groundTile);
            }
        }

        if (spikePrefab != null && Random.value < spikeChance)
        {
            int spikeX = Random.Range(startX, startX + chunkWidth);
            int spikeSurfaceY = GetSurfaceY(spikeX);

            if (spikeSurfaceY != -100)
            {
                Vector3 spikePos = tilemap.CellToWorld(new Vector3Int(spikeX, spikeSurfaceY, 0));
                spikePos += new Vector3(0.5f, 0.5f + spikeYOffset, 0f);
                Instantiate(spikePrefab, spikePos, Quaternion.identity);
            }
        }

        if (termitePrefab != null && Random.value < termiteChance)
        {
            int termiteX = Random.Range(startX, startX + chunkWidth);
            int termiteSurfaceY = GetSurfaceY(termiteX);

            if (termiteSurfaceY != -100)
            {
                Vector3 termitePos = tilemap.CellToWorld(new Vector3Int(termiteX, termiteSurfaceY, 0));
                termitePos += new Vector3(0.5f, 0.5f + termiteYOffset, 0f);
                Instantiate(termitePrefab, termitePos, Quaternion.identity);
            }
        }

        activeChunks.Enqueue(new Vector2Int(chunkIndex, currentHeight));
    }

    int GetSurfaceY(int x)
    {
        for (int y = maxStepHeight + 10; y >= -baseLayers - 10; y--)
        {
            if (tilemap.HasTile(new Vector3Int(x, y, 0)))
            {
                return y + 1;
            }
        }
        return -100;
    }

    void ClearChunk(Vector2Int chunk)
    {
        int startX = startPoint.x + chunk.x * chunkWidth;

        for (int x = startX; x < startX + chunkWidth; x++)
        {
            for (int y = -baseLayers; y < maxStepHeight + 4; y++)
            {
                tilemap.SetTile(new Vector3Int(x, startPoint.y + y, 0), null);
            }
        }
    }
    public void MakeHole(Vector2 worldPosition, float radius)
    {
        Vector3Int centerCell = tilemap.WorldToCell(worldPosition);
        int intRadius = Mathf.CeilToInt(radius);

        for (int x = -intRadius; x <= intRadius; x++)
        {
            for (int y = -intRadius; y <= intRadius; y++)
            {
                Vector3Int cellPos = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);
                Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);

                float dist = Vector2.Distance(new Vector2(cellCenter.x, cellCenter.y), worldPosition);

                if (dist <= radius)
                {
                    tilemap.SetTile(cellPos, null);
                }
            }
        }
    }

}
