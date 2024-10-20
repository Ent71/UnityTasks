using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Threading.Tasks;
using System;

public class ColorSelectorMenu : MonoBehaviour
{
    [SerializeField] private ColorSelector _colorSelector;
    [SerializeField] private ColorSelectorPanel _colorSelectorPanel;
    [SerializeField] private Renderer _cubeRenderer;
    [SerializeField] private Button _okButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _colorSelectorButton;

    private TaskCompletionSource<Color> _taskCompletionSource;

    private void OnEnable()
    {
        _colorSelectorButton.onClick.AddListener(OnColorSelectorButton);
    }

    private void OnDisable()
    {
        _colorSelectorButton.onClick.AddListener(OnColorSelectorButton);
    }

    private void OnColorSelectorButton()
    {
        _colorSelectorButton.gameObject.SetActive(false);
        OpenColorSelector();
    }

    private async void OpenColorSelector()
    {
        Color cubeColor = _cubeRenderer.material.color;

        try
        {
            Color selectedColor = await ShowColorSelectorDialog();
            _cubeRenderer.material.color = selectedColor;
        }

        catch(OperationCanceledException)
        {
            _cubeRenderer.material.color = cubeColor;
        }
    }

    private Task<Color> ShowColorSelectorDialog()
    {
        _taskCompletionSource = new TaskCompletionSource<Color>();

        _colorSelectorPanel.gameObject.SetActive(true);
        _colorSelector.SetStartColor(_cubeRenderer.material.color);

        _okButton.onClick.AddListener(OnOkButtonClick);
        _cancelButton.onClick.AddListener(OnCancelButtonClick);

        return _taskCompletionSource.Task;
    }

    private void OnOkButtonClick()
    {
        _taskCompletionSource.SetResult(_colorSelector.GetColor());
        CleanUp();
    }

    private void OnCancelButtonClick()
    {
        _taskCompletionSource.SetCanceled(); 
        CleanUp();
    }

    private void CleanUp()
    {
        _colorSelectorButton.gameObject.SetActive(true);
        _colorSelectorPanel.gameObject.SetActive(false);

        _okButton.onClick.RemoveListener(OnOkButtonClick);
        _cancelButton.onClick.RemoveListener(OnCancelButtonClick);
    }
}