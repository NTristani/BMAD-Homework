using System.Collections.Generic;
using UnityEngine;

public class CalculatorState : MonoBehaviour
{
    // This is a singleton we are going to use to save our data
    // It will take in current input, operators, and move them to a saved history
    // I am making this to have a location separated from the UI that saves our data
    // My implementation will not be reliant on the individual UI elements to save data
    // (like history and what it needs to currently show)
    // This allows me to create a portrait and landscape version of our UI which do not need to hold data

    public static CalculatorState Instance;

    public string currentInput = "";
    public List<string> tokens = new();
    public List<string> history = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }
}
