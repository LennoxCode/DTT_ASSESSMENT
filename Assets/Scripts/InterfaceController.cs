using UnityEngine;
using UnityEngine.UI;
using System;
using DefaultNamespace;

namespace MyNamespace
{
    
    public enum MazeAlgorithm
    {
        DepthFirst, Kruskal, Prim
        
    }
    /// <summary>
    /// <c> InterfaceController </c> is a controller class which enables the communication between the maze model
    /// and the GUI. It holds references to all the interface elements and one copy of the mazeGen.
    /// </summary>
    public class InterfaceController : MonoBehaviour
    {
        [SerializeField] private Dropdown algoDropdown;
        [SerializeField] private Slider widthSlider;
        [SerializeField] private Text widthDisplay;
        [SerializeField] private Text heightDisplay;
        [SerializeField] private Slider heightSlider;
        [SerializeField] private Text speedDisplay;
        [SerializeField] private Slider speedSlider;
        [SerializeField] private BaseMaze mazeGen;
        
        void Start()
        {
            mazeGen.OnMazeGenFinished += () => { SetSliders(true);};
            OnWidthChanged();
            OnHeightChanged();
            OnSpeedValueChanged();
            PopulateDropdown();
        }
        /// <summary>
        /// populates the drop down with every option of the Algorithm enum. using the build in Enum library
        /// it iterates over every value and ands it to the menu. lastly the text is changed to the default option.
        /// </summary>
        private void PopulateDropdown()
        {
            algoDropdown.options.Clear();
            foreach (MazeAlgorithm algo in Enum.GetValues(typeof(MazeAlgorithm)))
            {
                algoDropdown.options.Add(new Dropdown.OptionData(algo.ToString()));
            }
            algoDropdown.captionText.text = "" + (MazeAlgorithm)algoDropdown.value;
        }
        /// <summary>
        /// Callback function when the slider value of the speed slider changes the interface is updated accordingly
        /// and the setter of the Maze is called
        /// </summary>
        public void OnSpeedValueChanged()
        {
            float sliderVal = speedSlider.value;
            speedDisplay.text = "" + sliderVal;
            mazeGen.SetSpeed(sliderVal);
        }
        /// <summary>
        /// Callback function when the slider value of the width slider changes the interface is updated accordingly
        /// and the setter of the Maze is called
        /// </summary>
        public void OnWidthChanged()
        {
            int sliderVal = (int) widthSlider.value;
            widthDisplay.text = "" + sliderVal;
            mazeGen.SetWidth(sliderVal);
            mazeGen.GenerateGrid();
        }
        /// <summary>
        /// Callback function when the slider value of the height slider changes the interface is updated accordingly
        /// and the setter of the Maze is called
        /// </summary>
        public void OnHeightChanged()
        {
            int sliderVal = (int) heightSlider.value;
            heightDisplay.text = "" + sliderVal;
            mazeGen.SetHeight(sliderVal);
            mazeGen.GenerateGrid();
        }
        /// <summary>
        /// Callback function when the algorithm changes. It is not necessary to update this for the model
        /// because it is only needed when a maze is generated
        /// </summary>
        public void OnDropDownChanged()
        {
            algoDropdown.captionText.text = "" + (MazeAlgorithm)algoDropdown.value;
        }
        /// <summary>
        /// callback for the generation button. this function calls the generation algorithm with the corresponding
        /// maze algorithm
        /// </summary>
        public void OnGeneratePressed()
        {
            SetSliders(false);
            mazeGen.RunGeneration((MazeAlgorithm)algoDropdown.value);
        }

        private void SetSliders(bool value)
        {
            widthSlider.interactable = value;
            heightSlider.interactable = value;
        }
    }

}
