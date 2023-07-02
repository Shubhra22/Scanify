using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using Niantic.ARDK.Extensions.Scanning;

public class CustomSlider : MonoBehaviour
{
    public TMP_Text sliderValueText;
    [SerializeField]private Scrollbar scrollBar;
    public RulerComponent ruler;

    private float sliderValue;
    public float unitScale;

    public string measurementUnit = "m";

    [SerializeField]
    [Tooltip("The scene's ARScanManager")]
    private ARScanManager _scanManager;

    public TMP_Text rangeButtonText;

    public delegate void OnSliderValueChanged(float value);
    public event OnSliderValueChanged onSliderValueChanged;

    public float SliderValue
    {
        get
        {
            return sliderValue;
        }
        set
        {
            sliderValue = value;
            if(onSliderValueChanged!=null)
            {
                onSliderValueChanged(sliderValue);
            }
        }
    }

    private void Start()
    {
        scrollBar.onValueChanged.AddListener(onScrollBarValueChanged);
        _scanManager.maxScanDistance = 0.8f;

        float newVal = unitScale + math.remap(0f, 1f, ruler.MinValue, ruler.MaxValue, 0.5f);
        SliderValue = newVal;
        sliderValueText.text = newVal.ToString("0.0") + measurementUnit; ;
    }

    void onScrollBarValueChanged(float val)
    {

        float newVal = unitScale + math.remap(0f, 1f, ruler.MinValue, ruler.MaxValue, val);
        SliderValue = newVal;
        // float newValue = Math.remap(a, b, c, d, x);
        sliderValueText.text = newVal.ToString("0.0") + measurementUnit;
        if(rangeButtonText!=null)
        {
            rangeButtonText.text = "RANGE: "+newVal.ToString("0.0") + measurementUnit;
        }
        _scanManager.maxScanDistance = sliderValue;
     }
}
