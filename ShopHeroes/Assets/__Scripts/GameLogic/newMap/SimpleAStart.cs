using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int cellX;
    public int cellY;
    public float x;
    public float y;
    public Vector3Int WorldPos;
    public int G, H;//公式 F =G+H   F:通过路径总代价 G:起始点到当前点代价 H：当前点到目标点击代价
    public int F { get { return G + H; } }
    public PathNode parent = null;
    public bool isObstacle = false;
    public PathNode(int cx, int cy, int worldposx, int worldposy, bool obstacle)
    {
        this.cellX = cx;
        this.cellY = cy;
        WorldPos = new Vector3Int(worldposx, worldposy, 0);
        // centrepos = pos;
        isObstacle = obstacle;
    }
    public void setState(bool obstacle)
    {
        isObstacle = obstacle;
    }
}
public class SimpleAStart : TSingletonHotfix<SimpleAStart>
{
    PathNode m_StartNode = null;
    PathNode m_EndNode = null;
    List<PathNode> _openList = new List<PathNode>();
    HashSet<PathNode> _closeList = new HashSet<PathNode>();

    PathNode[,] mapGrid;
    public Stack<PathNode> FindPath(PathNode[,] map, int startX, int startY, int endX, int endY)
    {
        if (map == null || map[startX, startY] == null || map[endX, endY] == null) return new Stack<PathNode>();
        mapGrid = map;
        _openList.Clear();
        _closeList.Clear();
        m_StartNode = map[startX, startY];
        m_EndNode = map[endX, endY];


        _openList.Add(m_StartNode);

        while (_openList.Count > 0)
        {
            PathNode currentNode = _openList[0];

            for (int i = 0; i < _openList.Count; i++)
            {
                if (_openList[i].F < currentNode.F && _openList[i].H < currentNode.H)
                {
                    currentNode = _openList[i];
                }
            }

            _openList.Remove(currentNode);
            _closeList.Add(currentNode);

            if (currentNode == m_EndNode)
            {
                //找到终点
                return GeneratePath(m_StartNode, m_EndNode);
            }

            List<PathNode> aroundCells = GetNeibourhood(currentNode);
            foreach (var node in aroundCells)
            {
                if (node.isObstacle || _closeList.Contains(node))
                {
                    continue;
                }
                int newCost = currentNode.G + GetNodesDistance(node, currentNode);
                if (newCost < node.G || !_openList.Contains(node))
                {
                    node.G = newCost;
                    node.H = GetNodesDistance(node, m_EndNode);
                    node.parent = currentNode;
                    if (!_openList.Contains(node))
                    {
                        _openList.Add(node);
                    }
                }
            }
        }
        return new Stack<PathNode>();
    }

    private Stack<PathNode> GeneratePath(PathNode start, PathNode end)
    {
        Stack<PathNode> path = new Stack<PathNode>();
        PathNode node = end;
        while (node != start)
        {
            path.Push(node);
            node = node.parent;
        }
        return path;
    }
    public int GetNodesDistance(PathNode a, PathNode b)
    {
        int countX = Mathf.Abs(a.cellX - b.cellX);
        int countY = Mathf.Abs(a.cellY - b.cellY);
        int Dis;
        if (countX > countY)
        {
            Dis = countY * 14 + (countX - countY) * 10;
        }
        else
        {
            Dis = countX * 14 + (countY - countX) * 10;
        }
        return Dis;
    }
    //获取节点周围的节点
    public List<PathNode> GetNeibourhood(PathNode node)
    {
        List<PathNode> neribourhood = new List<PathNode>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                // if (i == 0 || j == 0)
                {
                    int tempX = node.cellX + j;
                    int tempY = node.cellY + i;
                    if (tempX >= 0 && tempY >= 0 && tempX < mapGrid.GetLength(0) && tempY < mapGrid.GetLength(1))
                    {
                        if (!mapGrid[tempX, tempY].isObstacle)
                        {
                            neribourhood.Add(mapGrid[tempX, tempY]);
                        }
                    }
                }
            }
        }
        return neribourhood;
    }
}
