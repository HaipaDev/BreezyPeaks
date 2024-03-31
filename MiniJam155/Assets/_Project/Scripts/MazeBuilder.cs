using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float roomSpacing = 15f;
    public GameObject[] roomPrefabs; // Array of room prefabs

    private HashSet<Vector2Int> visitedRooms = new HashSet<Vector2Int>();

    void Start()
    {
        GenerateMaze(new Vector2Int(0, 0));
    }

    void GenerateMaze(Vector2Int currentPosition)
    {
        // Mark the current room as visited
        visitedRooms.Add(currentPosition);

        // Instantiate the room prefab at the specified position
        InstantiateRoom(currentPosition);

        // Get unvisited neighbors
        List<Vector2Int> unvisitedNeighbors = GetUnvisitedNeighbors(currentPosition);

        // Explore each unvisited neighbor recursively
        foreach (Vector2Int neighbor in unvisitedNeighbors)
        {
            GenerateMaze(neighbor);
        }
    }

    void InstantiateRoom(Vector2Int position)
    {
        // Randomly choose a room prefab from the array
        GameObject roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

        // Instantiate the room prefab at the specified position
        GameObject roomObject = Instantiate(roomPrefab, GetWorldPosition(position), Quaternion.identity, transform);
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int currentPosition)
    {
        List<Vector2Int> unvisitedNeighbors = new List<Vector2Int>();

        // Define all possible directions
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPosition = currentPosition + direction;

            // Check if the neighbor is within bounds and not visited
            if (IsWithinBounds(neighborPosition) && !IsRoomVisited(neighborPosition))
            {
                unvisitedNeighbors.Add(neighborPosition);
            }
        }

        return unvisitedNeighbors;
    }

    bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridWidth && position.y >= 0 && position.y < gridHeight;
    }

    bool IsRoomVisited(Vector2Int position)
    {
        return visitedRooms.Contains(position);
    }

    Vector3 GetWorldPosition(Vector2Int position)
    {
        return new Vector3(position.x * roomSpacing, position.y * roomSpacing, 0);
    }
}
