using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSP_MapGen : MonoBehaviour {

    public struct Segment
    {
        public float xPos;
        public float yPos;
        public float width;
        public float height;
    }

    const float MAP_WIDTH = 10.0f;
    const float MAP_HEIGHT = 10.0f;

	// Use this for initialization
	void Start ()
    {

		
	}
	
    void MapGen()
    {
        List<List<List<Segment>>> Map = new List<List<List<Segment>>>();
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


    }
}
