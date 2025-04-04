using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class TerminalController : MonoBehaviour
{

    public GameObject terminalPanel; // Assign the terminal panel GameObject in the inspector
    public ScrollRect outputScroll; // Reference to the ScrollRect component for scrolling output
    public TMP_Text outputText; // Reference to the Text component to display terminal output
    public TMP_InputField inputField; // Reference to the InputField component for user input
    public AngerMeter angerMeter; // Reference to the AngerMeter component (optional, if you want to integrate with it)
    private List<string> commandHistory = new List<string>(); // Store command history for the terminal
    private int historyIndex = 0; // Track the current index in the command history for navigation
    private bool isTerminalVisible = false; // Track the visibility of the terminal panel

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //start  
        terminalPanel.SetActive(false); // Start with the terminal panel hidden
        inputField.onSubmit.AddListener(SubmitCommand); // Add listener to process input when the user presses Enter
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Toggle terminal visibility with the backquote key (`)
        {
            isTerminalVisible = !isTerminalVisible;
            terminalPanel.SetActive(isTerminalVisible);

            if (isTerminalVisible)
            {
                inputField.ActivateInputField(); // Focus the input field when the terminal is shown
            }
        }

        if(isTerminalVisible && commandHistory.Count > 0) // Allow navigation through command history with Up/Down arrows when terminal is active
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (historyIndex > 0)
                {
                    historyIndex--;
                    inputField.text = commandHistory[historyIndex];
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (historyIndex < commandHistory.Count - 1)
                {
                    historyIndex++;
                    inputField.text = commandHistory[historyIndex];
                }
                else
                {
                    inputField.text = "";
                    historyIndex = commandHistory.Count; // Reset to the end of the history
                }
            }
        }
    }

void SubmitCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return; // Ignore empty commands
        }

        // Add the command to the history
        if (commandHistory.Count == 0 || commandHistory[commandHistory.Count - 1] != input)
        {
            commandHistory.Add(input);
            historyIndex = commandHistory.Count; // Move to the end of the history for new input
        }

        // Process the command (for now, just echo it back)
        outputText.text += "\n> " + input;
        ProcessCommand(input);

        // Clear the input field after submission
        inputField.text = "";

        // Scroll to the bottom of the output
        outputScroll.verticalNormalizedPosition = 0f;
        inputField.ActivateInputField(); // Re-activate the input field for the next command
    }


void LogToTerminal(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return; // Ignore empty messages
        }

        // Append the message to the output text
        outputText.text += "\n" + message;
        Canvas.ForceUpdateCanvases();
        // Scroll to the bottom of the output
        outputScroll.verticalNormalizedPosition = 0f;
    }

    void ProcessCommand(string input)
    {
        string[] parts = input.ToLower().Split(' ');
        string command = parts[0];

        //Processing commands 
        switch (command)
        {
            case "help":
                LogToTerminal("Available commands: help, clear, exit, debuff, cure");
                break;
            case "clear":
                outputText.text = ""; // Clear the terminal output
                break;
            case "exit":
                isTerminalVisible = false;
                terminalPanel.SetActive(false); // Hide the terminal panel
                break;
            case "debuff":
                if (angerMeter != null)
                {
                    angerMeter.AppyDebuff(true); // Apply debuff to increase anger level
                    LogToTerminal("Debuff applied: Anger level will increase faster.");
                }
                else
                {
                    LogToTerminal("AngerMeter component not assigned.");
                }
                break;
            case "cure":
                if (angerMeter != null)
                {
                    angerMeter.AppyDebuff(false); // Remove debuff to return to normal anger level increase
                    LogToTerminal("Cure applied: Anger level will now increase at a normal rate.");
                }
                else
                {
                    LogToTerminal("AngerMeter component not assigned.");
                }
                break;
            default:
                LogToTerminal("Unknown command: " + command);
                break;
        }
    }
}