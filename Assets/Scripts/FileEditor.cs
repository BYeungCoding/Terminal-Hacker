using System.Collections;
using System.Data;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool IsInputFieldSelected()
    {
        return EventSystem.current.currentSelectedGameObject == inputField.gameObject;
    }
    void Start()
    {
        initialText = inputField.text;
        inputField.onValueChanged.AddListener(OnTextChanged);
    }

    void OnTextChanged(string newText){
        textModified = initialText != newText;
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
                    textModified = false;
                    saveWarning.SetActive(false);
                    break;
                case ":q!":
                    CloseEditor();
                    break;
                case ":wq":
                    SaveFile();
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
        isVisible = true;
        Debug.Log("isVisible: " + isVisible);
    }

    public void SaveFile(){
        linkedFile.fileContents = inputField.text;
        Debug.Log("File Saved: " + linkedFile.fileID);
    }

    public void CloseEditor(){
        Destroy(this.gameObject);
    }
}
