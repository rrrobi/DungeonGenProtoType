﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSP_MapGen : MonoBehaviour {

    public struct Segment
    {
        // Pos = centre
        public float xPos;
        public float yPos;
        public float width;
        public float height;

        public string segmentKey;
        public Room segmentRoom;
    }

    public struct Room
    {
        public int roomWidth;
        public int roomHeight;
        public int roomLeft;
        public int roomBottom;

        public List<Vector2Int> roomDoors;
    }

    List<List<List<Segment>>> BSPMap = new List<List<List<Segment>>>();
    const float MAP_WIDTH = 50.0f;
    const float MAP_HEIGHT = 30.0f;
    const float MAX_DIVIDE_RATIO = 0.70f;
    const float MIN_DIVIDE_RATION = 0.30f;

    const int DIVIDE_COUNT = 4;

    // Map
    public Sprite sampleFloor;
    public Sprite sampleEntranceTile;
    public Sprite sampleExitTile;
    public Sprite sampleDoor;
    public Sprite sampleWall;
    int[,] Map;

    // Entrance/Exit
    Vector2Int Entrance;
    Vector2Int Exit;

    float drawCounter = 0;
    float counterTimeOut = 2.0f;
  //  int drawIndex = 0;

    // Use this for initialization
    void Start ()
    {
        MapGen();
	}
	
    void MapGen()
    {
        DateTime before = DateTime.Now;

        // Map
        //      ________________
        //      |              |
        //      |              |
        //      |              |
        //      |              |
        //      |______________|
        //
        //      _________________
        //      |A      |B      |
        //      |       |       |
        //      |       |       |
        //      |       |       |
        //      |_______|_______|
        //  
        //      _________________
        //      |A1     |B1     |
        //      |       |       |
        //      |_______|_______|
        //      |A2     |B2     |
        //      |_______|_______|
        //
        //  List<Segment> - contains all the segments in the map
        //  List<Segment> A, List<Segment> B - contains all the segments split into 2 lists By the first divide, as shown above
        //  List<Segment> A1, List<Segment> A2, List<Segment> B1, List<Segment> B2 - contains all the segments split into 4 lists By the first 2 divides, as shown above
        //  ... and so on....
        
        // TODO.. Rethink this
        BSPMap = new List<List<List<Segment>>>();
             List<List<Segment>> startLevel = new List<List<Segment>>();
                  List<Segment> root = new List<Segment>
        {
            new Segment
            {
                xPos = 0.0f,
                yPos = 0.0f,
                width = MAP_WIDTH,
                height = MAP_HEIGHT,

                segmentKey = "BASE"
            }
        };
        startLevel.Add(root);
        BSPMap.Add(startLevel);


        #region Step 1 - Build List structure
        for (int i = 0; i < DIVIDE_COUNT; i++)
        {
            // Take last element of List
            var lastMapLevel = BSPMap[BSPMap.Count - 1];

            List<List<Segment>> newLevel = new List<List<Segment>>();
            // Decide if cut virtically or horizontaly
            if (i % 2 == 1)
            {
                foreach (var segmentList in lastMapLevel)
                {
                    foreach (var segment in segmentList)
                    {
                        newLevel.AddRange(SplitSegmentHorizontal(segment));
                    }
                    
                }
            }
            else
            {
                foreach (var segmentList in lastMapLevel)
                {
                    foreach (var segment in segmentList)
                    {
                        newLevel.AddRange(SplitSegmentVertical(segment));
                    }
                    
                }
            }

            // Add New Level to the Map
            BSPMap.Add(newLevel);
        }


        #endregion

        #region Step 2 - Replace individual large segments with list of all small segments that space contains

        foreach (var level in BSPMap)
        {
            foreach (var segmentList in level)
            {
                var temp = FillSegmentList(segmentList[0]);
                segmentList.RemoveAt(0);
                segmentList.AddRange(temp);
            }
        }

        #endregion

        // Remove last Level in the List
        // We don't need x lists of 1, 
        // we already have 1 list of x in the first level
        BSPMap.RemoveAt(BSPMap.Count - 1);


        BuildTileMap();


        DateTime after = DateTime.Now;
        TimeSpan duration = after.Subtract(before);
        Debug.Log("Time taken to generate BSP map (ms): " + duration);
    }

    private string GetNewKey(string inputKey)
    {
        string output = string.Empty;

        // If input is base, 


        return output;
    }

    private List<List<Segment>> SplitSegmentVertical(Segment input)
    {
        List<List<Segment>> output = new List<List<Segment>>();

        float seg1Ratio = RandomRatio();
        float seg2Ratio = 1.0f - seg1Ratio;

        Segment segment1 = new Segment()
        {
            // center of new Left segment = 
            // original center  
            // -Half Width  
            // +Half Ratio of original Full Width
            //xPos = input.xPos - (input.width / 2) + (input.width * (seg1Ratio / 2)),
            //yPos = input.yPos,

            // Bottom left of new Left Segment = 
            // xPos stays the same
            // yPos stays the same
            xPos = input.xPos,
            yPos = input.yPos,
            width = input.width * seg1Ratio,
            height = input.height
        };
        output.Add(new List<Segment>() { segment1 });
        Segment segment2 = new Segment()
        {
            // center of new Right segment = 
            // original center  
            // +Half Width  
            // -Half Ratio of original Full Width
            ////xPos = input.xPos + (input.width / 2) - (input.width * (seg2Ratio / 2)),
            ////yPos = input.yPos,

            // Bottom left of new Right Segment = 
            // xPos = + width of left segment
            // yPos stays the same
            xPos = input.xPos + input.width * seg1Ratio,
            yPos = input.yPos,
            width = input.width * seg2Ratio,
            height = input.height
        };
        output.Add(new List<Segment>() { segment2 });

        return output;
    }

    private List<List<Segment>> SplitSegmentHorizontal(Segment input)
    {
        List<List<Segment>> output = new List<List<Segment>>();

        float seg1Ratio = RandomRatio();
        float seg2Ratio = 1.0f - seg1Ratio;

        Segment segment1 = new Segment()
        {
            // center of new Top segment = 
            // original center  
            // +Half Hieght  
            // -Half Ratio of original Full Hieght
            //xPos = input.xPos,
            //yPos = input.yPos + (input.height / 2) - (input.height * (seg1Ratio / 2)),

            // Bottom left of top segment = 
            // xPos stays the same
            // ypos = +height of BOTTOM segement
            xPos = input.xPos,
            yPos = input.yPos + input.height * seg2Ratio,
            width = input.width,
            height = input.height * seg1Ratio,

        };
        output.Add(new List<Segment>() { segment1 });
        Segment segment2 = new Segment()
        {
            // center of new Bottom segment = 
            // original center  
            // -Half Hieght  
            // +Half Ratio of original Full Hieght
            //xPos = input.xPos,
            //yPos = input.yPos - (input.height / 2) + (input.height * (seg2Ratio / 2)),

            // Bottom Left of Bottom segment = 
            // xPos stays the same
            // yPos stays the same
            xPos = input.xPos,
            yPos = input.yPos,
            width = input.width,
            height = input.height * seg2Ratio
        };
        output.Add(new List<Segment>() { segment2 });

        return output;
    }

    private float RandomRatio()
    {
        return UnityEngine.Random.Range(MIN_DIVIDE_RATION, MAX_DIVIDE_RATIO);
    }

    private List<Segment> FillSegmentList(Segment region)
    {
        List<Segment> output = new List<Segment>();
        float regionLeft = region.xPos;// - region.width / 2;
        float regionRight = region.xPos + region.width;// / 2;
        float regionTop = region.yPos + region.height;// / 2;
        float regionBottom = region.yPos;// - region.height / 2;

        // For each segment created in the map
        
        //foreach (var level in Map)
        //{
            foreach (var segmentList in BSPMap[BSPMap.Count-1])
            {
                foreach (var segment in segmentList)
                {
                float centerXPos = segment.xPos + segment.width / 2;
                float centerYPos = segment.yPos + segment.height / 2;
                    // if the centre pos, is inside the given region
                    // Add to the List
                    if (centerXPos > regionLeft &&
                        centerXPos < regionRight &&
                        centerYPos < regionTop &&
                        centerYPos > regionBottom)
                    {
                        output.Add(segment);
                    }
                }
            }
        //}
        return output;
    }



    #region I dont like this at all - Redo it
    private void BuildTileMap()
    {
        // Reset Map
        Map = new int[(int)MAP_WIDTH, (int)MAP_HEIGHT];
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                Map[x, y] = 0;
            }
        }

        // For each segment, make a randomly sized room (which fits within that segment)
        // Update the Map with the new rooms
        for (int i = 0; i < BSPMap[0][0].Count; i++)
        {
            // have to 'replace' segment in question with a copy iincluding the room information, 
            // unsure why I was unable to simply amend the 'Room' property
            BSPMap[0][0][i] = BuildRoomInSegment(BSPMap[0][0][i]);            
        }

        // Take subset of total rooms (even number)
        // Pair them up, find pathbetween them
        // Weight tiles, 1 floor - 4ish wall, will prefer to cut through existing rooms, but still can cut new tunnels
        FirstPassCorridorCreation();

        // Place Entrance in on random tile in random room
        // Check it has a path to every other room
        // this time weight something like 1 - 100 weights, so MUCH more likly to use existing paths where possible
        SecondPassCorridorCreation(); //<-- Still working on this

        DrawMap();
    }

    private Segment BuildRoomInSegment(Segment segment)
    {
        int left = (int)segment.xPos;
        int bottom = (int)segment.yPos;

        // max size of the room is width-1 x height-1
        // This is because there must be enouigh space to have the wall all around the edge

        // Min size of the room can be fixed to an arbituary 2x2, 
        // Just to prevent tiny pointless rooms

        // Get random room size
        Room room = new Room();
        room.roomWidth = UnityEngine.Random.Range(2, (int)segment.width - 1);
        room.roomHeight = UnityEngine.Random.Range(2, (int)segment.height - 1);
        // Get random room start pos - 
        room.roomLeft = UnityEngine.Random.Range(left + 1, left + (int)segment.width - (room.roomWidth + 1));
        room.roomBottom = UnityEngine.Random.Range(bottom + 1, bottom + (int)segment.height - (room.roomHeight + 1));
        segment.segmentRoom = room;

        // Adjust tile map array to include new room
        for (int x = room.roomLeft; x < room.roomLeft + room.roomWidth; x++)
        {
            for (int y = room.roomBottom; y < room.roomBottom + room.roomHeight; y++)
            {
                Map[x, y] = 1;
            }
        }

        return segment;
    }

    private void FirstPassCorridorCreation()
    {
        // Get List of Random indices for segments to use
        // Roughly half the total
        // Make sure it is a even number
        List<int> segmentIndices = new List<int>();
        int maxIndex = BSPMap[0][0].Count;
        while (segmentIndices.Count < (BSPMap[0][0].Count) ||
            segmentIndices.Count % 2 != 0)
        {
            int index = UnityEngine.Random.Range(0, maxIndex);
            if (!segmentIndices.Contains(index))
            {
                segmentIndices.Add(index);
            }
        }

        List<int> impassableList = new List<int>();
        Dictionary<int, int> costModList = new Dictionary<int, int>();
        costModList.Add(0, 40);
        costModList.Add(1, 10);
        costModList.Add(2, 10);
        costModList.Add(3, 10);
        costModList.Add(4, 10);
        AstarPathfinder pathfinder = new AstarPathfinder(Map, costModList, impassableList);
        // loop through randomly selected segments
        for (int i = 0; i < segmentIndices.Count; i += 2)
        {
            // Start node will be in BSPMap[0][0][i]
            Vector2Int startNode = GetRandomPointInRoom(BSPMap[0][0][segmentIndices[i]]);
            // Target node will be in BSPMap[0][0][i + 1]
            Vector2Int targetNode = GetRandomPointInRoom(BSPMap[0][0][segmentIndices[i+1]]);            
            
            List<Node> path = pathfinder.StartPathfinder(startNode, targetNode);
            foreach (var node in path)
            {
                Map[node.xPos, node.yPos] = 1;
            }

        }
    }

    private void SecondPassCorridorCreation()
    {
        List<int> impassableList = new List<int>();
        Dictionary<int, int> costModList = new Dictionary<int, int>();
        costModList.Add(0, 1000);
        costModList.Add(1, 10);
        costModList.Add(2, 10);
        costModList.Add(3, 10);
        costModList.Add(4, 10);
        AstarPathfinder pathfinder = new AstarPathfinder(Map, costModList, impassableList);

        // Pick random Room
        // Pick Random tile in that room
        Entrance = GetRandomPointInRoom(BSPMap[0][0][UnityEngine.Random.Range(0, BSPMap[0][0].Count)]);
        // Assign 'Entrance' to that tile
        Map[Entrance.x, Entrance.y] = 3;

        int longestPathLength = 0;
        // loop through all Segments
        for (int i = 0; i < BSPMap[0][0].Count; i++)
        {
            // Ensure there is a path from the entrance to each room
            Vector2Int startNode = Entrance;
            Vector2Int targetNode = GetRandomPointInRoom(BSPMap[0][0][i]);                
                
            List<Node> path = pathfinder.StartPathfinder(startNode, targetNode);
            foreach (var node in path)
            {
                if (Map[node.xPos, node.yPos] == 0)
                    Map[node.xPos, node.yPos] = 1;
            }
            if (path.Count > longestPathLength)
            {
                longestPathLength = path.Count;
                // The Exit tile is replaced with the new longestPath target - temp level exit placement  
                Exit = new Vector2Int(targetNode.x, targetNode.y);
            }
        }

        // Use longest path from 2nd pass to place Exit stairs
        Map[Exit.x, Exit.y] = 4;
    }

    private Vector2Int GetRandomPointInRoom(Segment segment)
    {
        Vector2Int output = new Vector2Int();
        output.x = UnityEngine.Random.Range(segment.segmentRoom.roomLeft, segment.segmentRoom.roomLeft + segment.segmentRoom.roomWidth);
        output.y = UnityEngine.Random.Range(segment.segmentRoom.roomBottom, segment.segmentRoom.roomBottom + segment.segmentRoom.roomHeight);

        return output;
    }

    private void DrawMap()
    {
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                GameObject tile = new GameObject();
                tile.name = "Tile_" + x + "_" + y;
                tile.transform.position = new Vector3(x, y);
                if (Map[x, y] == 0)
                    tile.AddComponent<SpriteRenderer>().sprite = sampleWall;
                else if (Map[x,y] == 1)
                    tile.AddComponent<SpriteRenderer>().sprite = sampleFloor;
                else if (Map[x, y] == 2)
                    tile.AddComponent<SpriteRenderer>().sprite = sampleDoor;
                else if (Map[x, y] == 3)
                    tile.AddComponent<SpriteRenderer>().sprite = sampleEntranceTile;
                else if (Map[x, y] == 4)
                    tile.AddComponent<SpriteRenderer>().sprite = sampleExitTile;
            }
        }
    }
    #endregion

    void Update()
    {
        
        drawCounter += Time.deltaTime;
        if (drawCounter >= counterTimeOut)
        {
            //drawIndex++;
            drawCounter = 0;
            //if (drawIndex >= Map.Count)
            //{
            //    drawIndex = 0;
            //    MapGen();
            //}
        }


        foreach (var segList in BSPMap[0])
        {
            foreach (var segment in segList)
            {
                DrawSegment(segment);

            }
        }
    }

    private void DrawSegment(Segment input)
    {
        Vector3 topLeft = new Vector3(input.xPos, input.yPos + input.height, 0.0f);
        Vector3 topRight = new Vector3(input.xPos + input.width, input.yPos + input.height, 0.0f);
        Vector3 bottomLeft = new Vector3(input.xPos, input.yPos, 0.0f);
        Vector3 bottomRight = new Vector3(input.xPos + input.width, input.yPos, 0.0f);

        Debug.DrawLine(topLeft, topRight);
        Debug.DrawLine(topRight, bottomRight);
        Debug.DrawLine(bottomRight, bottomLeft);
        Debug.DrawLine(bottomLeft, topLeft);
    }
}
