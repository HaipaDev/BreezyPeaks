using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GridBuilder : MonoBehaviour{
    // [AssetsOnly][SerializeField] GameObject prefab_TL;
    // [AssetsOnly][SerializeField] GameObject prefab_TM;
    // [AssetsOnly][SerializeField] GameObject prefab_TR;
    // [AssetsOnly][SerializeField] GameObject prefab_ML;
    // [AssetsOnly][SerializeField] GameObject prefab_MM;
    // [AssetsOnly][SerializeField] GameObject prefab_MR;
    // [AssetsOnly][SerializeField] GameObject prefab_BL;
    // [AssetsOnly][SerializeField] GameObject prefab_BM;
    // [AssetsOnly][SerializeField] GameObject prefab_BR;
    [AssetsOnly][SerializeField] Room[] rooms;
    public int gridWidth = 5;
    public int gridHeight = 5;
    float roomSpacing = 15f;

    private List<GameObject> generatedRooms = new List<GameObject>();
    bool[,] compatibilityMatrix;
    GameObject lastSpawnedElement;

    void Start(){
        GenerateCompatibilityMatrix();
        Build();
    }
    void Build(){
        // Generate the initial room
        Vector3 currentPos = Vector3.zero;
        GameObject initialRoom = Instantiate(GetRandomRoom(), transform);
        initialRoom.transform.position = currentPos;
        generatedRooms.Add(initialRoom);

        // Expand the grid
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Skip if current position already has a room
                if (x == 0 && y == 0) continue;

                // Calculate the position of the new room
                currentPos = new Vector3(x * roomSpacing, y * roomSpacing, 0);

                // Check compatibility with neighboring rooms
                List<GameObject> compatibleRooms = GetCompatibleRooms(currentPos);

                // If there are compatible rooms, randomly select one and instantiate it
                if (compatibleRooms.Count > 0)
                {
                    GameObject newRoom = Instantiate(compatibleRooms[UnityEngine.Random.Range(0, compatibleRooms.Count)], transform);
                    newRoom.transform.position = currentPos;
                    generatedRooms.Add(newRoom);
                }
            }
        }
    }

    [Button("Rebuild")]
    public void Rebuild(){
        lastSpawnedElement = null;
        for(var i = transform.childCount-1; i>0; i--){
            Destroy(transform.GetChild(i).gameObject);
        }
        Build();
    }

    // List<GameObject> GetCompatibleRooms(Vector2 position)
    // {
    //     List<GameObject> compatibleRooms = new List<GameObject>();

    //     // Get neighboring rooms
    //     Room topRoomType = GetRoomAtPosition(position + Vector2.up * roomSpacing);
    //     Room bottomRoomType = GetRoomAtPosition(position + Vector2.down * roomSpacing);
    //     Room leftRoomType = GetRoomAtPosition(position + Vector2.left * roomSpacing);
    //     Room rightRoomType = GetRoomAtPosition(position + Vector2.right * roomSpacing);

    //     // Loop through room types and check compatibility
    //     foreach (Room roomType in rooms)
    //     {
    //         // Check compatibility with neighboring rooms
    //         if ((topRoomType == null || (topRoomType.openBottom && roomType.openTop)) &&
    //             (bottomRoomType == null || (bottomRoomType.openTop && roomType.openBottom)) &&
    //             (leftRoomType == null || (leftRoomType.openRight && roomType.openLeft)) &&
    //             (rightRoomType == null || (rightRoomType.openLeft && roomType.openRight)))
    //         {
    //             compatibleRooms.Add(roomType.gameObject);
    //         }
    //     }

    //     return compatibleRooms;
    // }
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


    bool IsCompatibleWithNeighbors(Vector2Int position, GameObject roomPrefab){
        // Check compatibility with neighboring rooms based on the compatibility matrix
        // You'll need to implement this method based on your compatibility rules
        // For example, you can use the compatibility matrix defined earlier

        // For demonstration purposes, always return true in this method
        return true;
    }

    GameObject GetRandomRoom(){
        return rooms[UnityEngine.Random.Range(0, rooms.Length)].gameObject;
    }
    void GenerateCompatibilityMatrix(){
        // compatibilityMatrix = new bool[rooms.Length, rooms.Length];
        // RoomType TL = new(false, true, true, false);
        // RoomType TM = new(false, true, false, false);
        // RoomType TR = new(false, true, false, true);
        // RoomType ML = new(true, true, true, false);
        // RoomType MM = new(true, true, true, true);
        // RoomType MR = new(true, true, false, true);
        // RoomType BL = new(true, false, true, false);
        // RoomType BM = new(true, false, false, false);
        // RoomType BR = new(true, false, false, true);

        // Compatibility matrix
        compatibilityMatrix = new bool[9, 9]
        {
            //       TL     TM     TR     ML     MM     MR     BL     BM     BR
            /*TL*/{ true, false, false, false, false, false, false, false, false },
            /*TM*/{ false, true, false, false, false, false, false, false, false },
            /*TR*/{ false, false, true, false, false, false, false, false, false },
            /*ML*/{ false, false, false, true, true, false, true, false, false },
            /*MM*/{ false, false, false, true, true, true, true, true, true },
            /*MR*/{ false, false, false, false, true, true, false, false, true },
            /*BL*/{ false, false, false, true, true, false, true, false, false },
            /*BM*/{ false, false, false, false, true, false, false, true, false },
            /*BR*/{ false, false, false, false, true, true, false, false, true }
        };
    }
}