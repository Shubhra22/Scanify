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
    [SerializeField]private Scrollbar slider;
    public RulerComponent ruler;
    public float sliderValue;

    [SerializeField]
    [Tooltip("The scene's ARScanManager")]
    private ARScanManager _scanManager;

    public float SliderValue
    {
        get
        {
            return sliderValue;
        }
    }

    private void Start()
    {
        slider.onValueChanged.AddListener(onSliderValueChanged);
        _scanManager.maxScanDistance = 0.8f;

        float newVal = math.remap(0f, 1f, ruler.MinValue, ruler.MaxValue, slider.value);
        sliderValue = newVal;
        sliderValueText.text = newVal.ToString("0.0m");
    }

    void onSliderValueChanged(float val)
    {

        float newVal = math.remap(0f, 1f, ruler.MinValue, ruler.MaxValue, val);
        sliderValue = newVal;
        // float newValue = Math.remap(a, b, c, d, x);
        sliderValueText.text = newVal.ToString("0.0m");
        _scanManager.maxScanDistance = sliderValue;
     }
}
