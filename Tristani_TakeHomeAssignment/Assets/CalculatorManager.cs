using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalculatorManager : MonoBehaviour
{
    // We are going to serialize all these fields so I can point to the their correct objects.
    // This is better than doing a find function, that is generally terrible practice
    // _P and _L refer to portrait and landscape counterparts respectivly
    [Header("Portrait")]
    [SerializeField] private TMP_Text equationText_P;
    [SerializeField] private TMP_Text resultText_P;
    [SerializeField] private Transform historyPanel_P;

    [Header("Landscape")]
    [SerializeField] private TMP_Text equationText_L;
    [SerializeField] private TMP_Text resultText_L;
    [SerializeField] private Transform historyPanel_L;

    [Header("History")]
    [SerializeField] private GameObject historyEntryPrefab;

    private CalculatorState state;
    //private bool lastPortrait;

    void Start()
    {
        //Create our singleton to store the data
        state = CalculatorState.Instance;
    }

    public void OnNumberClick(string number)
    {
        // Will be referenced in the button itself. Add the corresponding number to the equation string (held by our singleton)
        state.currentInput += number;
        RedrawEquation();
    }

    // Going to create this bool that we'll use to verify operators in the operatorclick function
    bool IsOperator(string s) => s == "+" || s == "-" || s == "*" || s == "/";

    public void OnOperatorClick(string op)
    {
        // This is the same as OnNumberClick, just for operators
        // Check if there is already an input and add to the token list
        if (!string.IsNullOrEmpty(state.currentInput))
        {
            state.tokens.Add(state.currentInput);
            state.currentInput = "";
        }

        // ^1 refers to the last element
        // basically checking if they already put an operator in
        // If they did, lets just replace it with the newest one they clicked
        if (state.tokens.Count > 0 && IsOperator(state.tokens[^1]))
        {
            state.tokens[^1] = op;
        }
        // Else just simply add the operator to the list
        else
        {
            state.tokens.Add(op);
        }

        RedrawEquation();
    }

    public void OnToggleSign()
    {
        // Special function that is only going to be referenced by the plusminus button
        // Very simple add or remove of minus
        if (string.IsNullOrEmpty(state.currentInput)) return;

        if (state.currentInput.StartsWith("-"))
        {
            state.currentInput = state.currentInput.Substring(1);
        }
        else
        {
            state.currentInput = "-" + state.currentInput;
        }
            
        RedrawEquation();
    }

    public void OnClear()
    {
        // Clear the singleton and then clear the text fields
        state.currentInput = "";
        state.tokens.Clear();
        RedrawEquation();
        RedrawResult("");
    }

    public void OnEquals()
    {
        // Checks for current input, add it to tokens list so it'll be used in equation
        if (!string.IsNullOrEmpty(state.currentInput))
        {
            state.tokens.Add(state.currentInput);
            state.currentInput = "";
        }

        // Head over to evaluate function to evaluate our tokens
        string result = Evaluate(state.tokens);
        RedrawResult(result);

        // Creating the string we show to the user
        string fullEq = string.Join(" ", state.tokens);
        state.history.Add($"{fullEq} = {result}");

        // Clear our lists and texts
        state.tokens.Clear();
        RedrawEquation();
        RedrawHistory();
    }

    void RedrawEquation()
    {
        // Join our tokens up until now
        string eq = string.Join(" ", state.tokens);

        // Add the current input
        if (!string.IsNullOrEmpty(state.currentInput)) eq += " " + state.currentInput;

        // Show the equation text, simple null check that we assigned the serilized field.
        if (equationText_P != null) equationText_P.text = eq;
        if (equationText_L != null) equationText_L.text = eq;
    }

    void RedrawResult(string result)
    {
        // Update the result text
        // Simple null check so we dont error out if we forgot to assign to the serilized field
        if (resultText_P != null) resultText_P.text = result;
        if (resultText_L != null) resultText_L.text = result;
    }

    void RedrawHistory()
    {
        // Clear both panels
        foreach (Transform child in historyPanel_P) Destroy(child.gameObject);
        foreach (Transform child in historyPanel_L) Destroy(child.gameObject);

        // Here we will add to the history (instantiate our history text prefab)
        foreach (string entry in state.history)
        {
            if (historyPanel_P)
            {
                var goP = Instantiate(historyEntryPrefab, historyPanel_P);
                goP.GetComponentInChildren<TMP_Text>().text = entry;
            }

            if (historyPanel_L)
            {
                var goL = Instantiate(historyEntryPrefab, historyPanel_L);
                goL.GetComponentInChildren<TMP_Text>().text = entry;
            }

        }
    }

    string Evaluate(List<string> tokens)
    {
        // Take our list of tokens (our inputs)
        // Check for their existence, then let's create our result, which we'll return as a float
        if (tokens.Count == 0) return 0.ToString();
        // We also want to make sure this is a valid equation, IE not operators in the front or back
        if ((IsOperator(tokens[0])) || IsOperator(tokens[^1])) return "error";

        // We'll create the result as a float so we can evaluate everything combined
        float result = float.Parse(tokens[0]);

        for (int i = 1; i < tokens.Count; i += 2)
        {
            string op = tokens[i];
            float next = float.Parse(tokens[i + 1]);

            // Switch statement to perform the proper operation when re arive at them
            switch (op)
            {
                case "+": result += next; break;
                case "-": result -= next; break;
                case "*": result *= next; break;
                case "/": result /= next; break;
            }
        }
        return result.ToString();
    }
}