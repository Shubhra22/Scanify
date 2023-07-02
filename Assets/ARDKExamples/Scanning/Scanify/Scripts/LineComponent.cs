using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LineComponent : Graphic
{
    public Vector2 pointA;
    public Vector2 pointB;

    public float thickness = 1;

    private VertexHelper vertexHelper;

    public delegate void OnLineMoved();

    public event OnLineMoved onLineMoved;

    public void LineMoved()
    {
        if(onLineMoved!=null)
        {
            onLineMoved();
        }
    }

    public void SetAll()
    {
        this.SetAllDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        //vertexHelper = vh;
        //DrawLine(pointA, pointB, thickness, this.color, vh);

        Vector2 pointA = - Vector3.right * rectTransform.rect.width * 0.5f;
        Vector2 pointB = Vector3.right * rectTransform.rect.width * 0.5f;

        //Debug.DrawLine(pointA, pointB);

        DrawLine(pointA, pointB, thickness, this.color, vh);
       // DrawLine(vh, thickness, this.color);
    }


    void DrawLine(Vector2 pointA, Vector2 pointB, float customThickness, Color color, VertexHelper vh)
    {
        vh.Clear();

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = pointA - new Vector2(0, customThickness / 2);
        vh.AddVert(vertex);

        vertex.position = pointA + new Vector2(0, customThickness / 2);//1
        vh.AddVert(vertex);

        vertex.position = pointB + new Vector2(0, customThickness / 2);//3
        vh.AddVert(vertex);

        vertex.position = pointB - new Vector2(0, customThickness /2);
        vh.AddVert(vertex);

        this.pointA = pointA;
        this.pointB = pointB;

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    

}
