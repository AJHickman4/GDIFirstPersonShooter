using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class sliderUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI volNumText;
    [SerializeField] float maxVol = 100f;

    public void ChangeVolText(float vol)
    {
        float volNum = vol * maxVol;
        volNumText.text = volNum.ToString("0");
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
