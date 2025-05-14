using System.IO;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DummyFile : MonoBehaviour
{
    public bool isCorrupted = false;
    public bool isHidden = false;
    public bool isWin = false;
    public string fileName;
    public DateTime creationDate;
    public DateTime lastAccessed;

    private SpriteRenderer sr;
    private Collider2D triggerZone;
    public TMP_InputField inputField;
    public TMP_Text outputText;
    public ScrollRect outputScroll;
    public int fileID;
    public TerminalController terminalController;
    public string fileContents = "<Replace with answer>";
    public string question;
    public GameObject fileEditor;
    public FileEditor editorScript;
    public LogicScript logicManager;
    public bool isSolved = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (isHidden)
        {
            sr.enabled = false;
            triggerZone = gameObject.AddComponent<BoxCollider2D>();
            triggerZone.isTrigger = true;
        }
    }

    void Awake()
    {
        fileName = GenerateRandomFileName();
        question = GeneratePuzzle();
        creationDate = DateTime.Now.AddDays(-UnityEngine.Random.Range(75, 200));
        lastAccessed = DateTime.Now.AddDays(UnityEngine.Random.Range(1, 75));

        if (logicManager == null)
        {
            GameObject managerObj = GameObject.Find("Logic Manager");
            if (managerObj != null)
            {
                logicManager = managerObj.GetComponent<LogicScript>();
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float dist = Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
            if (dist < 5f && !terminalController.isTerminalVisible)
            {
                if (isCorrupted)
                {
                    Debug.Log("This file is corrupted. You cannot read it.");
                }
                else
                {
                    Debug.Log("You read the file. It contains important information.");
                    terminalController.linkedFile = this;
                    terminalController.OpenTerminal();
                    terminalController.LogToTerminal("\n> You interacted with " + gameObject.name);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sr.enabled = true;
        }
    }

    public void OpenEditor(AngerMeter angerMeter)
    {
        GameObject editor = Instantiate(fileEditor);
        FileEditor editorScript = editor.GetComponent<FileEditor>();
        editorScript.angerMeter = angerMeter;
        editorScript.Setup(this);
    }

    public void Removefile()
    {
        Destroy(gameObject);
    }


    public void readFile()
    {
        GameObject editor = Instantiate(fileEditor);
        FileEditor editorScript = editor.GetComponent<FileEditor>();
        editorScript.Setup(this);
        editorScript.CloseEditor();
        terminalController.LogToTerminal(question);
    }

    string GenerateRandomFileName()
    {
        string[] names = { "system", "config", "report", "log", "data", "tmp", "cache", "session" };
        string[] extensions = { ".txt", ".log", ".dat", ".bin", ".cfg", ".tmp" };
        return names[UnityEngine.Random.Range(0, names.Length)] + UnityEngine.Random.Range(100, 999) + extensions[UnityEngine.Random.Range(0, extensions.Length)];
    }

    public void Reveal()
    {
        isHidden = false;
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        sr.enabled = true;

        // Remove trigger so it doesn’t re-hide or conflict
        if (triggerZone != null)
        {
            Destroy(triggerZone);
            triggerZone = null;
        }

        Debug.Log($"[Reveal] {gameObject.name} is now visible.");
    }

    string GeneratePuzzle(){
        string[] commands = { "edit", "delete", "change the name of", "read", "create" };
        string file = GenerateRandomFileName();
        string file2 = GenerateRandomFileName();
        string command = commands[UnityEngine.Random.Range(0,commands.Length)];
        if(command == "change the name of"){
            return "How would you " + command + " the file " + file + " to " + file2 + "?";
        } else {
            return "How would you " + command + " a file named " + file + "?";
        }
    }
}
