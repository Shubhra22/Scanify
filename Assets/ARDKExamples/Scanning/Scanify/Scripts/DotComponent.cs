using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotComponent : Image
{

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
            DotMoved(value);
        }
    }

    public delegate void OnDotMoved(Vector2 pos);

    public event OnDotMoved onDotMoved;

    public void DotMoved(Vector2 pos)
    {
        if (onDotMoved != null)
        {
            onDotMoved(pos);
        }
    }
}
