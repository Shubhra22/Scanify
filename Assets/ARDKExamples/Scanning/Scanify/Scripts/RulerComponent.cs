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
    [HideInInspector]public float thickness;
    [HideInInspector] public float length;
    [HideInInspector] public Color lineColor;
}

[ExecuteInEditMode]
public class RulerComponent : MaskableGraphic
{
    [Header("Line Looks")]
    public float thickness = 1;
    public float length = 1;
    public float space = 1;

    [Header("Scale Properties")]
    [SerializeField]
    private float minValue;

    [SerializeField]
    private float maxValue;

    [SerializeField]
    [Range(0.1f,100)]
    private float unitScale = 0.1f;

    [SerializeField]
    private int longScale = 10;

    public List<Point> points;

    private float rectWidth;

    public TMP_Text txtMeshObj;

    [InspectorButton("OnGenerateLabelsClicked")]
    public bool GenerateLabels;

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

    void OnGenerateLabelsClicked()
    {
        AddPoints();
    }


    protected override void Start()
    {
        //UpdatePoints();
        //AddPoints();
    }

    void UpdatePoints()
    {
        float diff = maxValue - minValue;
        points = new List<Point>();
        for (int i = 0; i < diff * longScale; i++)
        {
            float factor = rectTransform.pivot.x;
            Vector2 pos = new Vector2(-rectTransform.rect.width * factor + i, 0);

            Point p = new Point();

            p.numberOnScale = minValue + i* minValue;
            p.position = pos;
            points.Add(p);
        }
    }

    void AddPoints()
    {
        float diff = maxValue - minValue;// 0 - 5
        points = new List<Point>();

        float pointCount = diff / unitScale;
       // float width = pointCount * space ;

        for (int i = 0; i <= (int)pointCount; i++)
        {
            float factor = rectTransform.pivot.x;
            Vector2 pos = Vector2.right * space * i + new Vector2(-rectTransform.rect.width * factor + i, 0);

            Point p = new Point();
            //p.numberOnScale = minValue + (i + 1) * unitScale;
            //p.thickness = i + 1 > 0 && (i + 1) % longScale == 0 ? thickness + 1.0f : thickness;
            //p.lineColor = i + 1 > 0 && (i + 1) % longScale == 0 ? Color.white : Color.gray;
            //p.length = i + 1 > 0 && (i + 1) % longScale == 0 ? length + 5f : length;

            p.numberOnScale = minValue + (i) * unitScale;
            p.thickness = (i) % longScale == 0 ? thickness + 1.0f : thickness;
            p.lineColor = (i) % longScale == 0 ? Color.white : Color.gray;
            p.length = (i) % longScale == 0 ? length + 5f : length;

            p.position = pos;
            points.Add(p);

            if((i) % longScale == 0)
            {
                TMP_Text textObject = Instantiate(txtMeshObj, transform);
                textObject.text = p.numberOnScale.ToString();
                Vector2 txtPos = pos;
                txtPos.y = pos.y - p.length * 0.5f;
                textObject.transform.localPosition = txtPos;
            }
            
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
            Vector2 p = points[i].position;
            t = points[i].thickness;
            l = points[i].length;
            c = points[i].lineColor;

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
