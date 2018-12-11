using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarPathfinder : MonoBehaviour {

    struct Tile
    {
        public int xPos;
        public int yPos;
    }

    // Temp variables
    int MAP_WIDTH = 10;
    int MAP_HEIGHT = 10;
    public Sprite sampleFloor;
    public Sprite sampleBlueFloor;
    public Sprite sampleGreenFloor;
    public Sprite sampleWall;
    int[,] Map;

    // pathfinder variables
    Tile StartTile;
    Tile TargetTile;

    // Control variables
    bool isSetTarget = false;

	// Use this for initialization
	void Start () {
        // Reset Map
        Map = new int[(int)MAP_WIDTH, (int)MAP_HEIGHT];
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                Map[x, y] = 1;
            }
        }

        SetStart();
        DrawMap();
    }

    void SetStart()
    {
        // Hard code start to tile 1, 1 for now
        Map[1, 1] = 2;

        StartTile = new Tile();
        StartTile.xPos = 1;
        StartTile.yPos = 1;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
            OnTouch();
    }

    void OnTouch()
    {
        Debug.Log("Mouse Clicked!");
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit)
        {
            Debug.Log("Clicked on: " + hit.transform.name);
            if (hit.transform.name.Contains("Tile"))
            {
                int xIndex;
                int yIndex;

                string x = hit.transform.name.Split('_')[1];
                string y = hit.transform.name.Split('_')[2];
                if (int.TryParse(hit.transform.name.Split('_')[1], out xIndex) == false)
                    Debug.Log("tile hit is not named as expected");
                if (int.TryParse(hit.transform.name.Split('_')[2], out yIndex) == false)
                    Debug.Log("tile hit is not named as expected");

                if (isSetTarget)
                {
                    Map[xIndex, yIndex] = 2;
                    // Assign new target tile
                    TargetTile.xPos = xIndex;
                    TargetTile.yPos = yIndex;

                    isSetTarget = false;
                }
                else
                    Map[xIndex, yIndex] = 0;


                DrawMap();
            }
        }

    }

    public void ActivateSetTarget()
    {
        Debug.Log("Set target button clicked!");

        // Reset current target
        Map[TargetTile.xPos, TargetTile.yPos] = 1;

        isSetTarget = true;
        DrawMap();
    }

    public void DeactivateSetTaget()
    {

    }

    public void StartPathfinder()
    {
        Debug.Log("start button clicked!");
    }

    private void DrawMap()
    {
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                GameObject tile;
                string name = "Tile_" + x + "_" + y;
                if (GameObject.Find(name) == null)
                {
                    tile = new GameObject();
                    tile.name = name;
                    tile.transform.position = new Vector3(x, y);
                    tile.AddComponent<BoxCollider2D>().offset = new Vector2 (0.5f, 0.5f);
                    if (Map[x, y] == 0)
                        tile.AddComponent<SpriteRenderer>().sprite = sampleWall;
                    if (Map[x, y] == 1)
                        tile.AddComponent<SpriteRenderer>().sprite = sampleFloor;
                    if (Map[x, y] == 2)
                        tile.AddComponent<SpriteRenderer>().sprite = sampleBlueFloor;
                    if (Map[x, y] == 3)
                        tile.AddComponent<SpriteRenderer>().sprite = sampleGreenFloor;
                }
                else
                {
                    tile = GameObject.Find(name);
                    if (Map[x, y] == 0)
                        tile.GetComponent<SpriteRenderer>().sprite = sampleWall;
                    if (Map[x, y] == 1)
                        tile.GetComponent<SpriteRenderer>().sprite = sampleFloor;
                    if (Map[x, y] == 2)
                        tile.GetComponent<SpriteRenderer>().sprite = sampleBlueFloor;
                    if (Map[x, y] == 3)
                        tile.GetComponent<SpriteRenderer>().sprite = sampleGreenFloor;
                }
            }
        }
    }

}
