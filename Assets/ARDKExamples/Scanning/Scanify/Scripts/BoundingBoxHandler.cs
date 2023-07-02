using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxHandler : MonoBehaviour
{
    public LineComponent Left;
    public LineComponent Right;
    public LineComponent Up;
    public LineComponent Down;

    public DotComponent LT;
    public DotComponent RT;
    public DotComponent LB;
    public DotComponent RB;

    public Bounds meshBounds;

    //float leftRightPoints

    // Start is called before the first frame update
    void Start()
    {
        Left.onLineMoved += Left_onLineMoved;
        Right.onLineMoved += Right_onLineMoved;
        Up.onLineMoved += Up_onLineMoved;
        Down.onLineMoved += Down_onLineMoved;

        LT.onDotMoved += LT_onDotMoved;
        RT.onDotMoved += RT_onDotMoved;
        LB.onDotMoved += LB_onDotMoved;
        RB.onDotMoved += RB_onDotMoved;

        //Vector2 pos = Camera.main.WorldToScreenPoint(new Vector2(extent.x, extent.y));
        //print(Camera.main.WorldToScreenPoint(new Vector3(0.5f,0.5f,0)));

        
    }

    public void SetCornerPoints(Vector2 LTPos, Vector2 RTPos, Vector2 LBPos, Vector2 RBPos)
    {
        LT.Position = Camera.main.WorldToScreenPoint(LTPos);
        RT.Position = Camera.main.WorldToScreenPoint(RTPos);
        LB.Position = Camera.main.WorldToScreenPoint(LBPos);
        RB.Position = Camera.main.WorldToScreenPoint(RBPos);
    }

    private void DotMoved(LineComponent lineComponentX , LineComponent lineComponentY, DotComponent dotX, DotComponent dotY,Vector2 pos)
    {
        Vector3 tposX = lineComponentX.transform.position;
        tposX.x = pos.x;
        lineComponentX.transform.position = tposX;

        Vector3 tposY = lineComponentY.transform.position;
        tposY.y = pos.y;
        lineComponentY.transform.position = tposY;

        Vector3 dposX = dotX.transform.position;
        dposX.x = pos.x;
        dotX.transform.position = dposX;

        Vector3 dposY = dotY.transform.position;
        dposY.y = pos.y;
        dotY.transform.position = dposY;

        lineComponentX.LineMoved();
        lineComponentY.LineMoved();

        //HorizontalMove();
        //VerticalMove();
    }

    void MoveDotsOnLineMove(DotComponent d1, Vector2 pos)
    {
        Vector3 tPos = d1.transform.position;
        if(pos.x!=0)
        {
            tPos.x = pos.x;
        }
        else if(pos.y!=0)
        {
            tPos.y = pos.y;
        }
        d1.transform.position = tPos;

    }

    private void RB_onDotMoved(Vector2 pos)
    {
        DotMoved(Right, Down, RT,LB, pos);
    }

    private void RT_onDotMoved(Vector2 pos)
    {
        DotMoved(Right, Up, RB,LT,pos);
    }

    private void LB_onDotMoved(Vector2 pos)
    {
        DotMoved(Left, Down, LT,RB,pos);
    }

    private void LT_onDotMoved(Vector2 pos)
    {
        print("D"+LT.transform.position);
        DotMoved(Left, Up, LB, RT,pos);
    }

    private void Down_onLineMoved()
    {
        VerticalMove();
        MoveDotsOnLineMove(LB, new Vector2(0,Down.transform.position.y));
        MoveDotsOnLineMove(RB, new Vector2(0, Down.transform.position.y));
    }

    private void Up_onLineMoved()
    {
        VerticalMove();
        MoveDotsOnLineMove(LT, new Vector2(0, Up.transform.position.y));
        MoveDotsOnLineMove(RT, new Vector2(0, Up.transform.position.y));
    }

    private void Right_onLineMoved()
    {
        HorizontalMove();
        MoveDotsOnLineMove(RT, new Vector2(Right.transform.position.x,0));
        MoveDotsOnLineMove(RB, new Vector2(Right.transform.position.x,0));
    }

    private void Left_onLineMoved()
    {
        HorizontalMove();
        MoveDotsOnLineMove(LT, new Vector2(Left.transform.position.x,0));
        MoveDotsOnLineMove(LB, new Vector2(Left.transform.position.x,0));
    }

    void HorizontalMove()
    {
        RectTransform rectTransformLeft = Left.GetComponent<RectTransform>();
        RectTransform rectTransformRight = Right.GetComponent<RectTransform>();

        RectTransform rectTransformUp = Up.GetComponent<RectTransform>();
        RectTransform rectTransformDown = Down.GetComponent<RectTransform>();

        float width = rectTransformRight.anchoredPosition.x - rectTransformLeft.anchoredPosition.x;
        float anchorPosX = 0.5f * (rectTransformRight.anchoredPosition.x + rectTransformLeft.anchoredPosition.x);


        rectTransformUp.anchoredPosition = new Vector2(anchorPosX, rectTransformUp.anchoredPosition.y);
        rectTransformUp.sizeDelta = new Vector2(width, rectTransformUp.sizeDelta.y);

        rectTransformDown.anchoredPosition = new Vector2(anchorPosX, rectTransformDown.anchoredPosition.y);
        rectTransformDown.sizeDelta = new Vector2(width, rectTransformDown.sizeDelta.y);

        Up.SetAllDirty();
        Down.SetAllDirty();
    }    

    void VerticalMove()
    {
        RectTransform rectTransformUp = Up.GetComponent<RectTransform>();
        RectTransform rectTransformDown = Down.GetComponent<RectTransform>();

        RectTransform rectTransformLeft = Left.GetComponent<RectTransform>();
        RectTransform rectTransformRight = Right.GetComponent<RectTransform>();

        float width = rectTransformUp.anchoredPosition.y - rectTransformDown.anchoredPosition.y;
        float anchorPosX = 0.5f * (rectTransformUp.anchoredPosition.y + rectTransformDown.anchoredPosition.y);

        rectTransformLeft.anchoredPosition = new Vector2(rectTransformLeft.anchoredPosition.x, anchorPosX);
        rectTransformLeft.sizeDelta = new Vector2(width, rectTransformLeft.sizeDelta.y);

        rectTransformRight.anchoredPosition = new Vector2(rectTransformRight.anchoredPosition.x, anchorPosX);
        rectTransformRight.sizeDelta = new Vector2(width, rectTransformRight.sizeDelta.y);

        Left.SetAllDirty();
        Right.SetAllDirty();
    }
    
}
