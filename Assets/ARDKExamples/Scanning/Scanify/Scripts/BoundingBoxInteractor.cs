using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BoundingBoxInteractor : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerEnterHandler
{
    public Texture2D cursorTex;

    public enum Edge
    {
        None,
        Left,
        Right,
        Up,
        Down,
    }

    public enum Dot
    {
        None,
        LB,
        RB,
        LT,
        RT,
    }


    public Dot dot;
    public Edge edge;


    LineComponent lineComponent;
    DotComponent dotComponent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //print(eventData.position);
        Vector3 pos = transform.position;

        switch (edge)
        {
            case Edge.None:
                break;

            case Edge.Left:
                pos.x = eventData.position.x;
                lineComponent.LineMoved();
                break;
            case Edge.Right:
                pos.x = eventData.position.x;
                lineComponent.LineMoved();
                break;
            case Edge.Up:
                print("s");
                pos.y = eventData.position.y;
                lineComponent.LineMoved();
                break;
            case Edge.Down:
                pos.y = eventData.position.y;
                lineComponent.LineMoved();
                break;
        }

        switch (dot)
        {
            case Dot.None:
                break;

            default:
                pos.y = eventData.position.y;
                pos.x = eventData.position.x;
                dotComponent.DotMoved(pos);
                break;
        }

        //pos.x = eventData.position.x;
        transform.position = pos;
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Cursor.SetCursor(cursorTex, new Vector2(0f,0f), CursorMode.Auto);
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        lineComponent = GetComponent<LineComponent>();
        dotComponent = GetComponent<DotComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

