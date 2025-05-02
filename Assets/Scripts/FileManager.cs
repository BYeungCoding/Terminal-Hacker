using UnityEngine;

public class FileManager : MonoBehaviour
{
    public TerminalController terminalController;
    public GameObject dummyFile;
    public GameObject fileEditorScreen;
    public bool isVisible = false;
    public FileEditor fileEditor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fileEditor = fileEditorScreen.GetComponent<FileEditor>();
    }

    // Update is called once per frame
    void Update()
    {
        isVisible = fileEditor.isVisible;
    }
}
