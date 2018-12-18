using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    public int H_Value; // Heuristic - distance to target
    public int G_Value; // Move cost
    public int F_Value; // G + H
    public Node Parent; // Node used to reach this node
    public int xPos;
    public int yPos;

    public string Name { get { return xPos.ToString() + "_" + yPos.ToString(); } }

    // Overload for start node, as it does not have a parent
    public Node(int x, int y, Node target)
    {
        xPos = x;
        yPos = y;
        H_Value = FindHeuristic(this, target);
        G_Value = 0;
        F_Value = H_Value + G_Value;
    }

    // Overload for Target node
    public Node(int x, int y)
    {
        xPos = x;
        yPos = y;
        H_Value = 0;
        G_Value = 0;
        F_Value = H_Value + G_Value;
    }

    public Node(int x, int y, Node target, int moveCost, Node parent)
    {
        xPos = x;
        yPos = y;
        H_Value = FindHeuristic(this, target);
        Parent = parent;
        G_Value = parent.G_Value + moveCost;
        F_Value = H_Value + G_Value;
    }

    int FindHeuristic(Node start, Node target)
    {
        int startXIndex = start.xPos;
        int startYIndex = start.yPos;

        int targetXIndex = target.xPos;
        int targetYIndex = target.yPos;

        int H = Mathf.Abs(targetXIndex - startXIndex) + Mathf.Abs(targetYIndex - startYIndex);

        return H;
    }
}

public class AstarPathfinder : MonoBehaviour {

    // Temp variables
    int MAP_WIDTH = 10;
    int MAP_HEIGHT = 10;
    public Sprite sampleFloor;
    public Sprite sampleBlueFloor;
    public Sprite sampleGreenFloor;
    public Sprite sampleWall;
    int[,] Map;

    // pathfinder variables
    Node StartNode;
    Node TargetNode;

    bool foundTarget = false;
    int baseMoveCost = 10;


    Dictionary<string, Node> openList = new Dictionary<string, Node>();
    List<Node> closedList = new List<Node>();

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
        DrawMap();
    }

    void SetStart()
    {
        // Hard code start to tile 1, 1 for now
        Map[5, 1] = 2;

        StartNode = new Node(5, 1, TargetNode);
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
                    TargetNode = new Node(xIndex, yIndex);

                    SetStart();
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
        if (TargetNode != null)
        {
            Map[TargetNode.xPos, TargetNode.yPos] = 1;
            TargetNode = null;
        }

        isSetTarget = true;
        DrawMap();
    }

    public void DeactivateSetTarget()
    {

    }

    public void StartPathfinder()
    {
        Debug.Log("start button clicked!");

        Debug.Log("Starting H value: " + StartNode.H_Value);

        // Add Start node to ClosedList
        AddToClosedList(StartNode);

        // Add all surrounding nodes to OpenList
        Find_Check_AddSurroundingNodesToOpenList(StartNode);

        while (foundTarget == false)
        {
            // Find smallest F value on OpenList,
            int lowestF = openList.Min(s => s.Value.F_Value);
            Node lowestFNode = openList.First(s => s.Value.F_Value == lowestF).Value;
            // Add to ClosedList
            RemoveFromOpenList(lowestFNode);
            AddToClosedList(lowestFNode);
            // Add surrounding nodes to Open list
            // If node is the target, stop, we've found the end!
            // If node is already on closed list, ignore
            // If node is already on Open list, check if new F value would be lower than previous - if so update node info
            Find_Check_AddSurroundingNodesToOpenList(lowestFNode);
        }

        Debug.Log("Path found!");
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

    #region pathfinder methods
    
    void Find_Check_AddSurroundingNodesToOpenList(Node parent)
    {
        //  We are only interested in horizontal and virtical, NOT diagonal
        int newX = parent.xPos;
        int newY = parent.yPos;

        // Check Left
        ///////////////
        // Get pos
        newX = parent.xPos - 1;
        newY = parent.yPos;
        Node newNode = new Node(newX, newY, TargetNode, baseMoveCost, parent);
        if (CheckNode(newNode))
        { foundTarget = true; }
        // Check Right
        ///////////////
        // Get pos
        newX = parent.xPos + 1;
        newY = parent.yPos;
        newNode = new Node(newX, newY, TargetNode, baseMoveCost, parent);
        if (CheckNode(newNode))
        { foundTarget = true; }
        // Check Up
        ///////////////
        // Get pos
        newX = parent.xPos;
        newY = parent.yPos + 1;
        newNode = new Node(newX, newY, TargetNode, baseMoveCost, parent);
        if (CheckNode(newNode))
        { foundTarget = true; }
        // Check Down
        ///////////////
        // Get pos
        newX = parent.xPos;
        newY = parent.yPos - 1;
        newNode = new Node(newX, newY, TargetNode, baseMoveCost, parent);
        if (CheckNode(newNode))
        { foundTarget = true; }

    }

    bool CheckNode(Node newNode)
    {
        // If tile doesn't exist - Ignore
        if (newNode.xPos < 0 || newNode.xPos >= MAP_WIDTH ||
            newNode.yPos < 0 || newNode.yPos >= MAP_HEIGHT)
        { }
        // If tile is impassable - Ignore
        /// TODO...
        // If Node is the target - Break out, no need to keep looking
        else if (TargetNode.Name == newNode.Name)
        {
            //TODO...
            TargetNode = newNode;
            return true;
        }
        // If Node is in the ClosedList - Ignore, we have already checked this
        else if (CheckClosedListForNode(newNode))
        { }
        // If Node is already in the OpenList - check if info needs to be updated
        else if (CheckOpenListForNode(newNode))
        {
            if (newNode.F_Value < openList[newNode.Name].F_Value)
            {
                openList[newNode.Name] = newNode;
            }
        }
        // Add to OpenList
        else
        {
            openList.Add(newNode.Name, newNode);
        }

        // We have not yet found the target
        return false;
    }

    bool CheckForTargetNode(Node node)
    {
        if (node == TargetNode)
            return true;
        else
            return false;
    }

    bool CheckOpenListForNode(Node node)
    {
        if (openList.ContainsKey(node.Name))
            return true;
        else
            return false;
    }

    void AddToOpenList(Node node)
    {
        openList.Add(node.Name, node);
    }

    void RemoveFromOpenList(Node node)
    {
        if (openList.Remove(node.Name))
        {
        }
        else
            Debug.Log(node.Name + " Could not be found in the open list.");
    }

    bool CheckClosedListForNode(Node node)
    {
        if (closedList.Contains(node))
            return true;
        else
            return false;
    }

    void AddToClosedList(Node node)
    {
        closedList.Add(node);
    }

    void RemoveFromClosedList(Node node)
    {
        if (closedList.Remove(node))
        {
        }
        else
            Debug.Log(node.Name + " Could not be found in the closed list.");
    }

    #endregion

}
