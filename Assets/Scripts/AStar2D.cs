using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStar2D
{
    private const int STRAIGHT_COST = 10;
    private const int DIAGONAL_COST = 14;

    public class Node
    {
        public Vector2Int position;

        public int gCost;
        public int hCost; // Heuristic.
        public int fCost { get { return gCost + hCost; } }

        public Node previousNode;

        public Node(Vector2Int position)
        {
            this.gCost = int.MaxValue;
            this.hCost = 0;

            this.position = position;
        }

        public Node(Vector2Int position, int gCost, int hCost)
        {
            this.position = position;

            this.gCost = gCost;
            this.hCost = hCost;
        }
    }

    public static List<(Vector2Int, int)> FindPath(Vector2Int startPosition, Vector2Int endPosition, Dictionary<Vector2Int, Node> nodesMap)
    {
        if (nodesMap.TryGetValue(startPosition, out Node startNode) && nodesMap.TryGetValue(endPosition, out Node endNode))
        {
            List<Node> openNodes = new List<Node> { startNode };
            List<Node> processedNodes = new List<Node>();

            while (openNodes.Count > 0)
            {
                Node currentNode = GetLowestFCostNode(openNodes);

                if (currentNode == endNode)
                {
                    return CalculatePathFromNode(endNode);
                }

                openNodes.Remove(currentNode);
                processedNodes.Add(currentNode);

                foreach (Node neighbourNode in GetNeighbourNodes(currentNode, nodesMap))
                {
                    if (processedNodes.Contains(neighbourNode))
                    {
                        continue;
                    }

                    int tentativeGCost = currentNode.gCost + CalculateDistanceBetweenNodes(currentNode, neighbourNode);

                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.previousNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceBetweenNodes(neighbourNode, endNode);

                        if (!openNodes.Contains(neighbourNode))
                        {
                            openNodes.Add(neighbourNode);
                        }
                    }
                }
            }
        }

        return new List<(Vector2Int, int)>();
    }

    private static Node GetLowestFCostNode(List<Node> nodesList)
    {
        Node lowestFCostNode = nodesList[0];

        for (int i = 1; i < nodesList.Count; i++)
        {
            if (nodesList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = nodesList[i];
            }
        }

        return lowestFCostNode;
    }

    private static List<Node> GetNeighbourNodes(Node currentNode, Dictionary<Vector2Int, Node> nodesMap)
    {
        List<Node> neighbourNodes = new List<Node>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i != 0 || j != 0)
                {
                    Vector2Int neighbourPosition = currentNode.position + new Vector2Int(i, j);

                    if (nodesMap.TryGetValue(neighbourPosition, out Node neighbourNode))
                    {
                        neighbourNodes.Add(neighbourNode);
                    }
                }
            }
        }

        return neighbourNodes;
    }

    private static int CalculateDistanceBetweenNodes(Node aNode, Node bNode)
    {
        int xDistance = Mathf.Abs(aNode.position.x - bNode.position.x);
        int yDistance = Mathf.Abs(aNode.position.y - bNode.position.y);
        int rDistance = Mathf.Abs(xDistance - yDistance);

        return DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * rDistance;
    }

    private static List<(Vector2Int, int)> CalculatePathFromNode(Node endNode)
    {
        Node currentNode = endNode;
        List<(Vector2Int, int)> path = new List<(Vector2Int, int)>();

        while (currentNode.previousNode != null)
        {
            int costToReachNode = CalculateDistanceBetweenNodes(currentNode, currentNode.previousNode);

            path.Add((currentNode.position, costToReachNode));

            currentNode = currentNode.previousNode;
        }

        path.Reverse();

        return path;
    }
}
