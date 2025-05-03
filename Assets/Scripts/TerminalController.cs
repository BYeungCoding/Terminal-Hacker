
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System;

public class TerminalController : MonoBehaviour
{

    public GameObject terminalPanel; // Assign the terminal panel GameObject in the inspector
    public ScrollRect outputScroll; // Reference to the ScrollRect component for scrolling output
    public TMP_Text outputText; // Reference to the Text component to display terminal output
    public TMP_InputField inputField; // Reference to the InputField component for user input
    public AngerMeter angerMeter; // Reference to the AngerMeter component (optional, if you want to integrate with it)
    private List<string> commandHistory = new List<string>(); // Store command history for the terminal
    private int historyIndex = 0; // Track the current index in the command history for navigation
    public bool isTerminalVisible = false; // Track the visibility of the terminal panel
    public AudioClip TerminalOpenClip; // Assign the audio file directly in the inspector
    public AudioClip CommandSoundClip; // Assign the audio file directly in the inspector
    private AudioSource audioSource;
    public MapPrinter mapPrinter; // Reference to the MapPrinter component (optional, if you want to integrate with it)
    public levelGen levelGen; // Reference to the LevelGen component (optional, if you want to integrate with it)


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //start  
        terminalPanel.SetActive(false); // Start with the terminal panel hidden
        inputField.onSubmit.AddListener(SubmitCommand); // Add listener to process input when the user presses Enter

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (inputField.isFocused)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Tab)) // Toggle terminal visibility with the backquote key (`)
        {
            OpenTerminal();

            //Play terminal opening sound
            if (TerminalOpenClip != null)
            {
                audioSource.PlayOneShot(TerminalOpenClip); // Play the terminal opening sound
            }

            isTerminalVisible = !isTerminalVisible;
            terminalPanel.SetActive(isTerminalVisible);

            if (isTerminalVisible)
            {
                inputField.ActivateInputField(); // Focus the input field when the terminal is shown
            }
        }

        if (isTerminalVisible && commandHistory.Count > 0) // Allow navigation through command history with Up/Down arrows when terminal is active
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


    public void OpenTerminal(){
        isTerminalVisible = !isTerminalVisible;
        terminalPanel.SetActive(isTerminalVisible);

        if (isTerminalVisible)
        {
            inputField.ActivateInputField(); // Focus the input field when the terminal is shown
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


    public void LogToTerminal(string message)
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
        string[] parts = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string command = parts[0];
        string[] args = parts.Skip(1).ToArray();

        //Play feedback sound
        if (CommandSoundClip != null)
        {
            audioSource.PlayOneShot(CommandSoundClip); // Play the command feedback sound
        }

        //Processing commands 
        switch (command)
        {
            case "help":
                LogToTerminal("Available commands: help, clear, exit, debuff, cure");
                LogToTerminal("Available Bash commands: ls, ls -l, ls -a, cd, vim, mv, pwd, rm, touch");
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
            case "ls":
                if (args.Length == 0)
                {
                    bool includeHidden = false;
                    LogToTerminal("ls used: showing basic map");
                    LogToTerminal("Map of current directory: \n");
                    LogToTerminal("╔═════════ Floor Map ═════════╗\n" + mapPrinter.GetFloorLayout(includeHidden) + "╚════════════════════════════╝");
                }
                else if (args.Contains("-l"))
                {
                    bool includeHidden = false;
                    LogToTerminal("ls -l used: showing detailed map info");
                    LogToTerminal("Detailed file listing:\n");
                    LogToTerminal(mapPrinter.GetDetailedFileList(includeHidden));
                }
                else if (args.Contains("-a"))
                {
                    bool includeHidden = true;
                    LogToTerminal("ls -a used: showing all including hidden files");
                    LogToTerminal("Map of current directory: \n");
                    LogToTerminal("╔═════════ Floor Map ═════════╗\n" + mapPrinter.GetFloorLayout(includeHidden) + "╚════════════════════════════╝");

                }
                else
                {
                    LogToTerminal("ls used with unknown flag: " + string.Join(" ", args));
                }
                break;
            case "cd":
                if (args.Length > 0)
                {
                    if (args.Length > 1)
                    {
                        LogToTerminal("cd error: only one argument allowed");
                        break;
                    }
                    string target = args[0];
                    LogToTerminal($"cd used: changing directory to {target}");
                    // Add logic for actual directory change if you want
                }
                else
                {
                    LogToTerminal("cd error: missing target directory");
                }
                break;
            case "vim":
                if (args.Length > 0)
                {
                    string target = args[0];
                    LogToTerminal($"vim used: editing {target}");

                        LogToTerminal("Wrong file name");

                }
                else
                {
                    LogToTerminal("vim error: missing target file");
                }
                break;
            case "mv":
                if (args.Length > 0)
                {
                    string target = args[0];
                    LogToTerminal($"mv used: moving {target}");
                }
                else
                {
                    LogToTerminal("mv error: missing target file");
                }
                break;
            case "pwd":
                LogToTerminal("pwd was used: showing you the current path of your directory");
                break;
            case "rm":
                if (args.Length > 0)
                {
                    string target = args[0];
                    LogToTerminal($"rm used: deleting {target}");
                }
                else
                {
                    LogToTerminal("rm error: missing target file");
                }
                break;
            case "touch":
                if (args.Length > 0)
                {
                    string target = args[0];
                    LogToTerminal($"touch used: updating timestamp on/creating {target}");
                    // Add logic for handling whether or not the file already exists.
                }
                else
                {
                    LogToTerminal("touch error: missing target file");
                }
                break;
            case "cat":
                if (args.Length > 0){

                }
                break;
            default:
                LogToTerminal("Unknown command: " + command);
                break;
        }
    }
}
