using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    [SerializeField] private Slider _redSlider;
    [SerializeField] private Slider _greenSlider;
    [SerializeField] private Slider _blueSlider;

    public void SetStartColor(Color color)
    {
        _redSlider.value = color.r * 255;
        _greenSlider.value = color.g * 255;
        _blueSlider.value = color.b * 255;
    }

    public Color GetColor()
    {
        return new Color(_redSlider.value / 255, _greenSlider.value / 255, _blueSlider.value / 255);
    }
}