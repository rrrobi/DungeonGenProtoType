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

    List<List<List<Segment>>> Map = new List<List<List<Segment>>>();
    const float MAP_WIDTH = 10.0f;
    const float MAP_HEIGHT = 10.0f;
    const float MAX_DIVIDE_RATIO = 0.70f;
    const float MIN_DIVIDE_RATION = 0.30f;

    const int DIVIDE_COUNT = 5;

    float drawCounter = 0;
    float counterTimeOut = 2.0f;
    int drawIndex = 0;

    // Use this for initialization
    void Start ()
    {
        MapGen();
	}
	
    void MapGen()
    {
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
//        List<List<List<Segment>>> Map = new List<List<List<Segment>>>();
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
        Map.Add(startLevel);


        #region Step 1 - Build List structure
        for (int i = 0; i < DIVIDE_COUNT; i++)
        {
            // Take last element of List
            var lastMapLevel = Map[Map.Count - 1];

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
            Map.Add(newLevel);
        }


        #endregion

        #region Step 2 - Replace individual large segments with list of all small segments that space contains

        #endregion
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
            xPos = input.xPos - (input.width / 2) + (input.width * (seg1Ratio / 2)),
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
            xPos = input.xPos + (input.width / 2) - (input.width * (seg2Ratio / 2)),
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
            xPos = input.xPos,
            yPos = input.yPos + (input.height / 2) - (input.height * (seg1Ratio / 2)),
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
            xPos = input.xPos,
            yPos = input.yPos - (input.height / 2) + (input.height * (seg2Ratio / 2)),
            width = input.width,
            height = input.height * seg2Ratio
        };
        output.Add(new List<Segment>() { segment2 });

        return output;
    }

    private float RandomRatio()
    {
        return Random.Range(MIN_DIVIDE_RATION, MAX_DIVIDE_RATIO);
    }

    void Update()
    {
        
        drawCounter += Time.deltaTime;
        if (drawCounter >= counterTimeOut)
        {
            drawIndex++;
            drawCounter = 0;
            if (drawIndex >= Map.Count)
            {
                drawIndex = 0;
                MapGen();
            }
        }


        foreach (var segList in Map[drawIndex])
        {
            foreach (var segment in segList)
            {
                DrawSegment(segment);

            }
        }
    }

    private void DrawSegment(Segment input)
    {
        Vector3 topLeft = new Vector3(input.xPos - (input.width/2), input.yPos + (input.height/2), 0.0f);
        Vector3 topRight = new Vector3(input.xPos + (input.width / 2), input.yPos + (input.height / 2), 0.0f);
        Vector3 bottomLeft = new Vector3(input.xPos - (input.width / 2), input.yPos - (input.height / 2), 0.0f);
        Vector3 bottomRight = new Vector3(input.xPos + (input.width / 2), input.yPos - (input.height / 2), 0.0f);

        Debug.DrawLine(topLeft, topRight);
        Debug.DrawLine(topRight, bottomRight);
        Debug.DrawLine(bottomRight, bottomLeft);
        Debug.DrawLine(bottomLeft, topLeft);
    }
}
