using System;
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
    }

    List<List<List<Segment>>> BSPMap = new List<List<List<Segment>>>();
    const float MAP_WIDTH = 50.0f;
    const float MAP_HEIGHT = 30.0f;
    const float MAX_DIVIDE_RATIO = 0.70f;
    const float MIN_DIVIDE_RATION = 0.30f;

    const int DIVIDE_COUNT = 4;

    // Map
    public Sprite sampleFloor;
    public Sprite sampleWall;
    int[,] Map;

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
                height = MAP_HEIGHT
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
            height = input.height * seg1Ratio
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

        foreach (var segment in BSPMap[0][0])
        {
            //BuildRoomInSegment(segment);
        }

        DrawMap();
    }

    private void BuildRoomInSegment(Segment segment)
    {
        int left = (int)segment.xPos - (int)segment.width / 2;
        int top = (int)segment.yPos + (int)segment.height / 2;

        // max size of the room is width-1 x height-1
        // This is because there must be enouigh space to have the wall all around the edge

        // Min size of the room can be fixed to an arbituary 2x2, 
        // Just to prevent tiny pointless rooms

        // Not used yet
        int roomWidth = UnityEngine.Random.Range(2, (int)segment.width - 1);
        int roomHeight = UnityEngine.Random.Range(2, (int)segment.height - 1);

        // Temp - Just makes a room 1 smaller than the full segment size each time.
        // Adjust tile map array to include new room
        for (int x = left+1; x < left + segment.width; x++)
        {
            for (int y = top-1; y > top - segment.height; y--)
            {
                Map[x + ((int)MAP_WIDTH/2), y + ((int)MAP_HEIGHT/2)] = 1;
            }
        }

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
                if (Map[x,y] == 1)
                    tile.AddComponent<SpriteRenderer>().sprite = sampleFloor;
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
