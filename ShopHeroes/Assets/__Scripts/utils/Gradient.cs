using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum DirectionType
{
    horizontal,
    vertical
}
[AddComponentMenu("UI/Effect/Gradient")]
public class Gradient : BaseMeshEffect
{
    [SerializeField]
    private Color32 firstColor = Color.black;
    [SerializeField]
    private Color32 secondColor = Color.black;
    [SerializeField]
    private DirectionType direction;
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        var vertextList = new List<UIVertex>();
        vh.GetUIVertexStream(vertextList);
        int count = vertextList.Count;

        ApplyGradient(vertextList, 0, count);
        vh.Clear();
        vh.AddUIVertexTriangleStream(vertextList);

    }

    private void ApplyGradient(List<UIVertex> vertexList, int start, int end)
    {

        print(end);
        if (direction == DirectionType.horizontal)
        {
            float leftX = 0;
            float RightX = 0;
            for (int i = 0; i < end; i++)
            {
                float x = vertexList[i].position.x;

                if (x > RightX)
                {
                    RightX = x;
                }
                else if (x < leftX)
                {
                    leftX = x;
                }
            }

            float uiElementWeight = RightX - leftX;

            for (int i = 0; i < end; i++)
            {
                UIVertex uiVertex = vertexList[i];
                uiVertex.color = Color32.Lerp(firstColor, secondColor, (uiVertex.position.x - leftX) / uiElementWeight);
                vertexList[i] = uiVertex;
            }
        }
        else
        {
            float topY = 0;
            float downY = 0;
            for (int i = 0; i < end; i++)
            {
                float y = vertexList[i].position.y;

                if (y > downY)
                {
                    downY = y;
                }
                else if (y < topY)
                {
                    topY = y;
                }
            }

            float uiElementWeight = downY - topY;

            for (int i = 0; i < end; i++)
            {
                UIVertex uiVertex = vertexList[i];
                uiVertex.color = Color32.Lerp(secondColor, firstColor, (uiVertex.position.y - topY) / uiElementWeight);
                vertexList[i] = uiVertex;
            }
        }
    }
}
