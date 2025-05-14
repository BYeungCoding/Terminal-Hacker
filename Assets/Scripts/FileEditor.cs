using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FileEditor : MonoBehaviour
{
    public TMP_InputField inputField;
    private DummyFile linkedFile;
    public TMP_InputField exCommands;
    public GameObject exCommandInput;
    public TMP_Text exText;
    private string initialText;
    private bool textModified = false;
    public bool isVisible = false;
    public GameObject saveWarning;
    public UnityEngine.UI.Text question;
    public string questionText;
    public AngerMeter angerMeter;
    public LogicScript logicManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool IsInputFieldSelected()
    {
        return EventSystem.current.currentSelectedGameObject == inputField.gameObject;
    }
    void Start()
    {
        initialText = inputField.text;
        inputField.onValueChanged.AddListener(OnTextChanged);
        questionText = question.text;
        if (logicManager == null)
        {
            GameObject managerObj = GameObject.Find("Logic Manager");
            if (managerObj != null)
            {
                logicManager = managerObj.GetComponent<LogicScript>();
            }
        }
    }

    void OnTextChanged(string newText){
        textModified = initialText != newText;
        if(newText.Contains(";")){
            inputField.text = newText.Substring(0, newText.Length-1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Semicolon)){
            Debug.Log("Ex Command should be toggled");
            StartCoroutine(endOfLineCursor());
        }
        if(Input.GetKeyDown(KeyCode.Tab)){
            Debug.Log("This is the text before the Invoke: " + inputField.text);
            Debug.Log("This is the text after the Invoke: " + inputField.text);
            inputField.DeactivateInputField();
            Debug.Log("This is the text after the Deactivate: " + inputField.text);
        }
        if(Input.GetKeyDown(KeyCode.I)){
            focusVim();
        }
        if(Input.GetKeyDown(KeyCode.Return)){
            string command = exCommands.text.Trim();
            Debug.Log("Command entered: " + command);
            switch(command){
                case ":q":
                    Debug.Log("Quiting vim");
                    if(!textModified){
                        CloseEditor();
                    } else {
                        saveWarning.SetActive(true);
                    }
                    break;
                case ":w":
                    SaveFile();
                    exCommands.DeactivateInputField();
                    exCommandInput.SetActive(false);
                    saveWarning.SetActive(false);
                    CheckAnswer();
                    break;
                case ":q!":
                    CloseEditor();
                    break;
                case ":wq":
                    SaveFile();
                    CheckAnswer();
                    CloseEditor();
                    break;
            }
        }
        if(exCommands.isFocused && exCommands.text.Trim() == ":"){
            if(Input.GetKeyDown(KeyCode.Backspace)){
                exCommands.DeactivateInputField();
            }
        }
    }

    void focusVim(){
        inputField.ActivateInputField();
        inputField.MoveToEndOfLine(true,false);
        inputField.selectionFocusPosition = inputField.text.Length;
        inputField.selectionAnchorPosition = inputField.text.Length;
        exCommandInput.SetActive(false);
    }

    IEnumerator endOfLineCursor(){
        exCommandInput.SetActive(true);
        exCommands.text = ":";
        exCommands.Select();
        exCommands.ActivateInputField();
        yield return null;
        exCommands.MoveToEndOfLine(true,false);
        exCommands.selectionAnchorPosition = 1;  // Anchor at caret position
        exCommands.selectionFocusPosition = 1;   // Focus at caret position
        Debug.Log("Ex Command Input Active: " + exCommandInput.activeSelf + " Ex Command Focused: " + exCommands.isFocused);
    }
    public void Setup(DummyFile file){
        linkedFile = file;
        inputField.text = file.fileContents;
        question.text = file.question;
        isVisible = true;
        Debug.Log("isVisible: " + isVisible);
    }

    public void SaveFile(){
        linkedFile.fileContents = inputField.text;
        textModified = false;
        Debug.Log("File Saved: " + linkedFile.fileID);
    }

    public void CloseEditor(){
        Destroy(this.gameObject);
    }

    public void CheckAnswer(){
        string[] questionParts = questionText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string[] parts = linkedFile.fileContents.Split(' ',StringSplitOptions.RemoveEmptyEntries);
        string questionCommand = questionParts[3];
        string file1 = "";
        string file2 = "";
        string userFile1 = "";
        string userFile2 = "";
        if(questionParts.Length == 8){
            file1 = questionParts[7][..^1];
            userFile1 = parts[1];
        } else if(questionParts.Length == 12){
            var files = questionParts.Where(s => s.Contains(".")).ToArray();
            file1 = files[0];
            file2 = files[1][..^1];
            userFile1 = parts[1];
            userFile2 = parts[2];
        }
        string userCommand = parts[0];

        Debug.Log("userCommand: " + userCommand + " userFile: " + userFile1 + " questionCommand: " + questionCommand + " file1: " + file1);
        switch(questionCommand){
            case "edit":
                if(userCommand == "vim" && (file1 == userFile1) && (linkedFile.isSolved == false)){
                    angerMeter.CalmDown();
                    if(linkedFile.isWin){
                        logicManager.WinFileSolved();
                    }
                    linkedFile.isSolved = true;
                    Debug.Log("Cure applied");
                }
                break;
            case "delete":
                if(userCommand == "rm" && (file1 == userFile1) && (linkedFile.isSolved == false)){
                    angerMeter.CalmDown();
                    if(linkedFile.isWin){
                        logicManager.WinFileSolved();
                    }
                    linkedFile.isSolved = true;
                    Debug.Log("Cure applied");
                }
                break;
            case "create":
                if(userCommand == "touch" && (file1 == userFile1) && (linkedFile.isSolved == false)){
                    angerMeter.CalmDown();
                    if(linkedFile.isWin){
                        logicManager.WinFileSolved();
                    }
                    linkedFile.isSolved = true;
                    Debug.Log("Cure applied");
                }
                break;
            case "read":
                if(userCommand == "cat" && (file1 == userFile1) && (linkedFile.isSolved == false)){
                    angerMeter.CalmDown();
                    if(linkedFile.isWin){
                        logicManager.WinFileSolved();
                    }
                    linkedFile.isSolved = true;
                    Debug.Log("Cure applied");
                }
                break;
            case "change":
                if(userCommand == "mv" && (file1 == userFile1) && (file2 == userFile2) && (linkedFile.isSolved == false)){
                    angerMeter.CalmDown();
                    if(linkedFile.isWin){
                        logicManager.WinFileSolved();
                    }
                    linkedFile.isSolved = true;
                    Debug.Log("Cure applied");
                }
                break;
            default:
                Debug.Log("An Error Has Occurred");
                break;
        }
    }
}
