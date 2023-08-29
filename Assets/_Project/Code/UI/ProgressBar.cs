using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider sliderInstance;
    [SerializeField] private TextMeshProUGUI textMeshInstance;

    [Header("Values")]
    [SerializeField] private int _value;
    public int value {  get => _value; set 
        { 
            _value = value;
            UpdateValue();
        } }

    [SerializeField] private int _minValue;
    public int minValue { get => _minValue; set 
        { 
            _minValue = value;
            UpdateMinValue();
        } }

    [SerializeField] private int _maxValue;
    public int maxValue { get => _maxValue; set 
        { 
            _maxValue = value;
            UpdateMaxValue();
        } }

#if UNITY_EDITOR
    void OnValidate()
    {
        UpdateMinValue();
        UpdateMaxValue();
        UpdateValue();
    }
#endif

    private void UpdateValue()
    {
        int clampedValue = Mathf.Clamp(_value, minValue, maxValue);
        if (clampedValue != _value)
            _value = clampedValue;
        sliderInstance.value = _value;
        UpdateText();
    }

    private void UpdateMinValue()
    {
        sliderInstance.minValue = minValue;
        UpdateText();
    }

    private void UpdateMaxValue()
    {
        sliderInstance.maxValue = maxValue;
        UpdateText();
    }

    private void UpdateText()
    {
        textMeshInstance.text = $"{value}/{maxValue}";
    }
}