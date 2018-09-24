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

    const float MAP_WIDTH = 10.0f;
    const float MAP_HEIGHT = 10.0f;

    const int DIVIDE_COUNT = 3;

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
        List<List<List<Segment>>> Map = new List<List<List<Segment>>>();
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
                        newLevel.Add(SplitSegmentHorizontal(segment));
                    }
                    
                }
            }
            else
            {
                foreach (var segmentList in lastMapLevel)
                {
                    foreach (var segment in segmentList)
                    {
                        newLevel.Add(SplitSegmentVertical(segment));
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



    private List<Segment> SplitSegmentVertical(Segment input)
    {
        List<Segment> output = new List<Segment>();

        Segment segment1 = new Segment()
        {
            xPos = input.xPos - (input.width / 4),
            yPos = input.yPos,
            width = input.width / 2,
            height = input.height
        };
        output.Add(segment1);
        Segment segment2 = new Segment()
        {
            xPos = input.xPos + (input.width / 4),
            yPos = input.yPos,
            width = input.width / 2,
            height = input.height
        };
        output.Add(segment2);

        return output;
    }

    private List<Segment> SplitSegmentHorizontal(Segment input)
    {
        List<Segment> output = new List<Segment>();

        Segment segment1 = new Segment()
        {
            xPos = input.xPos,
            yPos = input.yPos - (input.height / 4),
            width = input.width,
            height = input.height / 2
        };
        output.Add(segment1);
        Segment segment2 = new Segment()
        {
            xPos = input.xPos,
            yPos = input.yPos + (input.height / 4),
            width = input.width,
            height = input.height / 2
        };
        output.Add(segment2);

        return output;
    }
}
