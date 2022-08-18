using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MyNamespace
{
    
    public enum MazeAlgorithm
    {
        DepthFirst, Kruskal, Prim
        
    }
    public class InterfaceController : MonoBehaviour
    {
        [SerializeField] private Dropdown algoDropdown;
        [SerializeField] private Slider widthSlider;
        [SerializeField] private Text widthDisplay;
        [SerializeField] private Text heightDisplay;
        [SerializeField] private Slider heightSlider;
        [SerializeField] private Text speedDisplay;
        [SerializeField] private Slider speedSlider;
        [SerializeField] private Maze mazeGen;

        private KeyValuePair<String, MazeAlgorithm> map;
        void Start()
        {
            OnWidthChanged();
            OnHeightChanged();
            OnSpeedValueChanged();
            PopulateDropdown();
        }

        private void PopulateDropdown()
        {
            algoDropdown.options.Clear();
            foreach (MazeAlgorithm algo in Enum.GetValues(typeof(MazeAlgorithm)))
            {
                algoDropdown.options.Add(new Dropdown.OptionData(algo.ToString()));
            }
            algoDropdown.captionText.text = "" + (MazeAlgorithm)algoDropdown.value;
        }
     
        public void OnSpeedValueChanged()
        {
      
            float sliderVal = speedSlider.value;
            speedDisplay.text = "" + sliderVal;
            mazeGen.SetSpeed(sliderVal);
                  
        
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

        public void OnDropDownChanged()
        {
            algoDropdown.captionText.text = "" + (MazeAlgorithm)algoDropdown.value;
        }
        public void OnGeneratePressed()
        {
            mazeGen.RunGeneration((MazeAlgorithm)algoDropdown.value);
        }
    }

}
