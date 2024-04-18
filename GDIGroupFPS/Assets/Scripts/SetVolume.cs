using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    [SerializeField] string mixerGroup = "MainMusic";
    [SerializeField] AudioMixer mix;
    [SerializeField] Slider slider;
    [SerializeField] float volScale = 20f;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderValueChange);
        
    }

    private void OnSliderValueChange(float vol)
    {
        mix.SetFloat(mixerGroup, Mathf.Log10(vol) * volScale);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(mixerGroup, slider.value);
    }

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(mixerGroup, slider.value);
    }
}
