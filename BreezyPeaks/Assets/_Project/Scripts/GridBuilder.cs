using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class GridBuilder : MonoBehaviour{
    [AssetsOnly][SerializeField] Room[] rooms;
    public int gridWidth = 5;
    public int gridHeight = 5;
    float roomSpacing = 15f;
    List<List<Vector3>> gridPositions = new List<List<Vector3>>();
    List<GameObject> generatedRooms = new List<GameObject>();
    GameObject lastSpawnedElement;

    void Start(){
        GenerateGridPositions();
        Build();
    }
    void GenerateGridPositions()
    {
        gridPositions.Clear();

        // Loop through each row
        for (int y = 0; y < gridHeight; y++)
        {
            List<Vector3> row = new List<Vector3>();

            // Loop through each column in the row
            for (int x = 0; x < gridWidth; x++)
            {
                // Calculate the position of the room based on the grid coordinates
                Vector3 roomPosition = new Vector3(x * roomSpacing, y * roomSpacing, 0f);

                // Add the position to the row
                row.Add(roomPosition);
                Debug.Log(x+", "+y+" | "+roomPosition);
            }

            // Add the row to the gridPositions list
            gridPositions.Add(row);
        }
    }
    // void Build(){
    //     // Generate the initial room
    //     Vector3 currentPos = Vector3.zero;
    //     GameObject initialRoom = Instantiate(GetRandomRoom(), transform);
    //     initialRoom.transform.position = currentPos;
    //     generatedRooms.Add(initialRoom);

    //     // Expand the grid
    //     for (int x = 0; x < gridWidth; x++)
    //     {
    //         for (int y = 0; y < gridHeight; y++)
    //         {
    //             // Skip if current position already has a room
    //             if (x == 0 && y == 0) continue;

    //             // Calculate the position of the new room
    //             currentPos = new Vector3(x * roomSpacing, y * roomSpacing, 0);

    //             // Check compatibility with neighboring rooms
    //             List<GameObject> compatibleRooms = GetCompatibleRooms(currentPos);

    //             // If there are compatible rooms, randomly select one and instantiate it
    //             if (compatibleRooms.Count > 0)
    //             {
    //                 GameObject newRoom = Instantiate(compatibleRooms[UnityEngine.Random.Range(0, compatibleRooms.Count)], transform);
    //                 newRoom.transform.position = currentPos;
    //                 generatedRooms.Add(newRoom);
    //             }
    //         }
    //     }
    // }
    // void Build()
    // {
    //     // Generate the initial room
    //     Vector3 currentPos = Vector3.zero;
    //     GameObject initialRoom = Instantiate(GetRandomRoom(), transform);
    //     initialRoom.transform.position = currentPos;
    //     generatedRooms.Add(initialRoom);

    //     // Expand the maze
    //     while (true)
    //     {
    //         // Get the list of neighboring positions
    //         List<Vector3> neighborPositions = GetNeighborPositions(currentPos);

    //         // Filter out positions that are outside the grid or already have a room
    //         neighborPositions.RemoveAll(p => p.x < 0 || p.x >= gridWidth || p.y < 0 || p.y >= gridHeight || generatedRooms.Exists(room => room.transform.position == p));

    //         // If there are no valid neighbors, exit the loop
    //         if (neighborPositions.Count == 0)
    //         {
    //             break;
    //         }

    //         // Choose a random neighbor position
    //         currentPos = neighborPositions[Random.Range(0, neighborPositions.Count)];

    //         // Instantiate a random compatible room at the chosen position
    //         GameObject newRoom = Instantiate(GetRandomCompatibleRoom(currentPos), currentPos, Quaternion.identity, transform);
    //         generatedRooms.Add(newRoom);
    //     }
    // }
    void Build()
    {
        // Generate the initial room
        Vector3 initialPos = Vector3.zero;
        GameObject initialRoom = Instantiate(GetRandomRoom(), transform);
        initialRoom.transform.position = initialPos;
        generatedRooms.Add(initialRoom);

        // Initialize a queue to track positions to visit
        Queue<Vector3> positionsToVisit = new Queue<Vector3>();
        positionsToVisit.Enqueue(initialPos);

        // Continue generating rooms until all positions are visited
        while (positionsToVisit.Count > 0)
        {
            // Dequeue the current position to visit
            Vector3 currentPosition = positionsToVisit.Dequeue();

            // Get neighboring positions
            List<Vector3> neighborPositions = GetNeighborPositions(currentPosition);

            // Iterate through neighboring positions
            foreach (Vector3 neighborPos in neighborPositions)
            {
                // Skip if position has already been visited
                if (generatedRooms.Any(room => room.transform.position == neighborPos))
                    continue;

                // Check compatibility with neighboring rooms
                List<GameObject> compatibleRooms = GetCompatibleRooms(neighborPos);

                // If there are compatible rooms, randomly select one and instantiate it
                if (compatibleRooms.Count > 0)
                {
                    GameObject newRoom = Instantiate(compatibleRooms[UnityEngine.Random.Range(0, compatibleRooms.Count)], transform);
                    newRoom.transform.position = neighborPos;
                    generatedRooms.Add(newRoom);

                    // Add the newly generated room's position to the queue for further exploration
                    positionsToVisit.Enqueue(neighborPos);
                }
            }
        }
    }



    [Button("Rebuild")]
    public void Rebuild(){
        generatedRooms = new List<GameObject>();
        lastSpawnedElement = null;
        for(var i = transform.childCount-1; i>0; i--){
            Destroy(transform.GetChild(i).gameObject);
        }
        Build();
    }
    GameObject GetRandomCompatibleRoom(Vector2 position){
        List<GameObject> compatibleRooms = GetCompatibleRooms(position);
        return compatibleRooms[UnityEngine.Random.Range(0, compatibleRooms.Count)];
    }
    List<GameObject> GetCompatibleRooms(Vector2 position)
    {
        List<GameObject> compatibleRooms = new List<GameObject>();

        // Get neighboring rooms
        Room topRoom = GetRoomAtPosition(position + Vector2.up * roomSpacing);
        Room bottomRoom = GetRoomAtPosition(position + Vector2.down * roomSpacing);
        Room leftRoom = GetRoomAtPosition(position + Vector2.left * roomSpacing);
        Room rightRoom = GetRoomAtPosition(position + Vector2.right * roomSpacing);

        // Check if any neighboring rooms are present
        bool hasTopRoom = topRoom != null;
        bool hasBottomRoom = bottomRoom != null;
        bool hasLeftRoom = leftRoom != null;
        bool hasRightRoom = rightRoom != null;

        // Loop through room types and check compatibility
        foreach (Room room in rooms)
        {
            // Check if the room type has at least one open side
            if (room.openTop || room.openBottom || room.openLeft || room.openRight)
            {
                // Check compatibility with neighboring rooms
                if ((!hasTopRoom || (hasTopRoom && room.openTop)) &&
                    (!hasBottomRoom || (hasBottomRoom && room.openBottom)) &&
                    (!hasLeftRoom || (hasLeftRoom && room.openLeft)) &&
                    (!hasRightRoom || (hasRightRoom && room.openRight)))
                {
                    compatibleRooms.Add(room.gameObject);
                }
            }
        }

        return compatibleRooms;
    }

    Room GetRoomAtPosition(Vector2 position)
    {
        // Perform a raycast to check for a room at the given position
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null)
        {
            Room roomComponent = hit.collider.GetComponent<Room>();
            if (roomComponent != null)
            {
                return roomComponent;
            }
        }
        return null;
    }
    List<Vector3> GetNeighborPositions(Vector3 currentPosition)
    {
        List<Vector3> neighborPositions = new List<Vector3>();

        // Define the four cardinal directions
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

        // Loop through each direction to calculate neighboring positions
        foreach (Vector3 direction in directions)
        {
            // Calculate the neighboring position
            Vector3 neighborPosition = currentPosition + direction * roomSpacing;

            // Add the neighboring position to the list if it falls within the grid bounds
            if (IsWithinGridBounds(neighborPosition))
            {
                neighborPositions.Add(neighborPosition);
            }
        }

        Debug.Log(neighborPositions.Count);
        return neighborPositions;
    }

    bool IsWithinGridBounds(Vector3 position)
    {
        // Calculate the maximum coordinates based on grid width and height
        float maxX = (gridWidth - 1) * roomSpacing / 2;
        float minX = -maxX;
        float maxY = (gridHeight - 1) * roomSpacing / 2;
        float minY = -maxY;

        // Check if the position falls within the grid bounds
        return position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY;
    }

    GameObject GetRandomRoom(){
        return rooms[UnityEngine.Random.Range(0, rooms.Length)].gameObject;
    }
}