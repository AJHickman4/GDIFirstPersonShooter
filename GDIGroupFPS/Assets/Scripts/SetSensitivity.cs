using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetSensitivity : MonoBehaviour
{
    [SerializeField] string sensitivity = "MouseSensitivity";
    [SerializeField] cameraController controller;
    [SerializeField] Slider slider;
    [SerializeField] float numScale = 20f;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderValueChange);
    }

    private void OnSliderValueChange(float val)
    {
        if (controller)
            controller.sensitivity = slider.value * numScale;
        else
            slider.interactable = false;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(sensitivity, slider.value);
    }

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(sensitivity, slider.value);
    }
}
