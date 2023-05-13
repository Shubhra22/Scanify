using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Point
{
    public Vector2 position;
    public float numberOnScale;
    float width;
}

[ExecuteInEditMode]
public class RulerComponent : MaskableGraphic
{
    public float thickness = 1;
    public float length = 1;
    public float space = 1;

    [SerializeField]
    private float minValue;

    [SerializeField]
    private float maxValue;

    public List<Point> points;

    private float rectWidth;

    public float MinValue
    {
        get
        {
            return minValue;
        }
        set
        {
            minValue = value;
            UpdatePoints();
        }
    }

    public float MaxValue
    {
        get
        {
            return maxValue;
        }
        set
        {
            maxValue = value;
            UpdatePoints();
        }
    }

    protected override void Start()
    {
        UpdatePoints();
        
    }

    void UpdatePoints()
    {
        float diff = maxValue - minValue;
        points = new List<Point>();
        for (int i = 0; i < diff * 10 + 1; i++)
        {
            float factor = rectTransform.pivot.x;
            Vector2 pos = new Vector2(-rectTransform.rect.width * factor + i, 0);


            Point p = new Point();
            p.numberOnScale = minValue + i* minValue;
            p.position = pos;
            points.Add(p);
        }
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float t = thickness;
        float l = length;
        Color c = Color.gray;

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 p = points[i].position + Vector2.right * space * i;

            if(points[i].numberOnScale == (int)points[i].numberOnScale)
            {
                t = thickness + 1.0f;
                l = length + 5f;
                c = Color.white;
            }
            else
            {
                t = thickness;
                c = Color.gray;
                l = length;
                
            }

            DrawLine(p, p + new Vector2(0, l), t,c,vh);
        }


        for (int j = 0; j < points.Count; j++)
        {
            int i = j * 4;
            vh.AddTriangle(i+0, i+1, i+2);
            vh.AddTriangle(i+2, i+1, i+3);
        }
        rectWidth = points.Count * space + points.Count * thickness;
        rectTransform.rect.Set(rectTransform.rect.x, rectTransform.rect.y, rectWidth, rectTransform.rect.height);
    }

    void CreateLineOnPoint(Vector2 point, VertexHelper vh)
    {
        vh.Clear();

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = Color.blue;

        vertex.position = point;
        vh.AddVert(vertex);

        vertex.position = new Vector3(0, thickness);//1
        vh.AddVert(vertex);

        vertex.position = new Vector3(thickness, thickness);//2
        vh.AddVert(vertex);

        vertex.position = new Vector3(thickness, 0);//3
        vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }


    void DrawLine(Vector2 pointA, Vector2 pointB, float customThickness, Color color,VertexHelper vh)
    {
       
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = pointA;
        vh.AddVert(vertex);

        vertex.position = pointA + new Vector2(customThickness, 0);//1
        vh.AddVert(vertex);

        vertex.position = pointB;
        vh.AddVert(vertex);

        vertex.position = pointB + new Vector2(customThickness, 0);//3
        vh.AddVert(vertex);
    }


}
