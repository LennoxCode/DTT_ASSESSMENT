using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{
    [SerializeField] private Slider widthSlider;
    [SerializeField] private Text widthDisplay;
    [SerializeField] private Text heightDisplay;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Maze mazeGen;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnWidthChanged()
    {
        int sliderVal = (int) widthSlider.value;
        widthDisplay.text = "" + sliderVal;
        mazeGen.SetWidth(sliderVal);
        mazeGen.GenerateGrid();
    }

    public void OnHeightChanged()
    {
        int sliderVal = (int) heightSlider.value;
        heightDisplay.text = "" + sliderVal;
        mazeGen.SetHeight(sliderVal);
        mazeGen.GenerateGrid();
    }
}
