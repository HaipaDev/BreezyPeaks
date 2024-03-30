using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GridBuilder : MonoBehaviour{
    [AssetsOnly][SerializeField] Room[] rooms;
    public int gridWidth = 5;
    public int gridHeight = 5;
    float roomSpacing = 15f;

    private List<GameObject> generatedRooms = new List<GameObject>();
    GameObject lastSpawnedElement;

    void Start(){
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

    GameObject GetRandomRoom(){
        return rooms[UnityEngine.Random.Range(0, rooms.Length)].gameObject;
    }
}