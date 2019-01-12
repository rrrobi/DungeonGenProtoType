using System;
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

public class AstarPathfinder
{

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
    List<int> ImpassableList;
    Dictionary<int, int> CostModList;

    bool foundTarget = false;
    //int baseMoveCost = 10;


    Dictionary<string, Node> openList = new Dictionary<string, Node>();
    List<Node> closedList = new List<Node>();
    List<Node> path = new List<Node>();


    // Control variables
    bool isSetTarget = false;

    public AstarPathfinder(int[,] map, 
        Dictionary<int, int> costModList, List<int> impassable)
    {
        Map = map;
        MAP_WIDTH = map.GetLength(0);
        MAP_HEIGHT = map.GetLength(1);
        CostModList = costModList;
        ImpassableList = impassable;
    }

    public List<Node> StartPathfinder(Vector2Int startIndex, Vector2Int targetIndex)
    {
        // Reset pathfinder
        foundTarget = false;
        closedList = new List<Node>();
        openList = new Dictionary<string, Node>();
        TargetNode = new Node(targetIndex.x, targetIndex.y);
        StartNode = new Node(startIndex.x, startIndex.y, TargetNode);

        Debug.Log("Pathfinder has begun, Starting H value: " + StartNode.H_Value);
        DateTime beforePath = DateTime.Now;
        // reset current path
        path = new List<Node>();

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
        BuildPath();
        DateTime afterPathTime = DateTime.Now;
        TimeSpan durationPath = afterPathTime.Subtract(beforePath);
        Debug.Log("Time taken to generate Path: " + durationPath + ". Path Length: " + path.Count);


        return path;
    }

    #region pathfinder methods

    void Find_Check_AddSurroundingNodesToOpenList(Node parent)
    {
        //  We are only interested in horizontal and virtical, NOT diagonal
        int newX = parent.xPos;
        int newY = parent.yPos;

        Node newNode;
        // Check Left
        ///////////////
        // Get pos
        newX = parent.xPos - 1;
        newY = parent.yPos;
        if (IsNodeInMap(newX, newY))
        {
            newNode = new Node(newX, newY, TargetNode, CostModList[Map[newX, newY]], parent);
            if (CheckNode(newNode))
            {
                foundTarget = true;
            }
        }
        // Check Right
        ///////////////
        // Get pos
        newX = parent.xPos + 1;
        newY = parent.yPos;
        if (IsNodeInMap(newX, newY))
        {
            newNode = new Node(newX, newY, TargetNode, CostModList[Map[newX, newY]], parent);
            if (CheckNode(newNode))
            { foundTarget = true; }
        }        
        // Check Up
        ///////////////
        // Get pos
        newX = parent.xPos;
        newY = parent.yPos + 1;
        if (IsNodeInMap(newX, newY))
        {
            newNode = new Node(newX, newY, TargetNode, CostModList[Map[newX, newY]], parent);
            if (CheckNode(newNode))
            { foundTarget = true; }
        }
        // Check Down
        ///////////////
        // Get pos
        newX = parent.xPos;
        newY = parent.yPos - 1;
        if (IsNodeInMap(newX, newY))
        {
            newNode = new Node(newX, newY, TargetNode, CostModList[Map[newX, newY]], parent);
            if (CheckNode(newNode))
            { foundTarget = true; }
        }
    }

    bool IsNodeInMap(int x, int y)
    {
        if (x < 0 || x >= MAP_WIDTH ||
            y < 0 || y >= MAP_HEIGHT)
            return false;

        return true;
    }

    bool CheckNode(Node newNode)
    {
        // If tile is impassable - Ignore
        if (ImpassableList.Contains(Map[newNode.xPos, newNode.yPos]))
        { }
        // If Node is the target - Break out, no need to keep looking
        else if (TargetNode.Name == newNode.Name)
        {
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

    void BuildPath()
    {
        Node currentNode = TargetNode;
        while (currentNode.Name != StartNode.Name)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;

            // should never hit this
            if (currentNode.Parent == null)
                break;
        }
        // we have reached the start node
        path.Add(StartNode);

        path.Reverse();
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
            Debug.LogError(node.Name + " Could not be found in the open list.");
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
            Debug.LogError(node.Name + " Could not be found in the closed list.");
    }

    #endregion

}