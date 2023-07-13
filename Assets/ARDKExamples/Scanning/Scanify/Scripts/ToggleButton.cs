using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using DG.Tweening;

[RequireComponent(typeof(Toggle))]
[RequireComponent(typeof(Image))]

public class ToggleButton : MonoBehaviour
{
    public Color selectedColor;
    public Color unselectedColor;

    public GameObject rangeScrollBar;

    Sequence sq;

    //public delegate void OnToggleStateChanged(bool clicked);
    //public event OnToggleStateChanged onToggleStateChanged;

    // Start is called before the first frame update
    void Start()
    {
        Toggle toggle = GetComponent<Toggle>();
        // base.Start();
        toggle.onValueChanged.AddListener(onClickToggleButton);
        rangeScrollBar.SetActive(false);
        sq = DOTween.Sequence();
    }

    private void onClickToggleButton(bool value)
    {
        GetComponent<Image>().color = value ? selectedColor : unselectedColor;
        //sq.Append(rangeScrollBar.transform.)
        rangeScrollBar.SetActive(value);
    }

   
}
