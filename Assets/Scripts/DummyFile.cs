using System.IO;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework.Internal.Commands;

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
    public string fileContents = "Default text";
    public GameObject fileEditor;
    public FileEditor editorScript;
    public LogicScript logicManager;
    public DebuffManager debuffManager;
    private int debuffChance;
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
        creationDate = DateTime.Now.AddDays(-UnityEngine.Random.Range(75, 200));
        lastAccessed = DateTime.Now.AddDays(UnityEngine.Random.Range(1, 75));

        if (logicManager == null)
        {
            GameObject managerObj = GameObject.Find("Logic Manager");
            if (managerObj != null)
            {
                logicManager = managerObj.GetComponent<LogicScript>();
                Debug.Log("Logic manager assigned.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(debuffManager == null){
            GameObject debuffManage = GameObject.Find("Effects Zone");
            if (debuffManage != null)
            {
                debuffManager = debuffManage.GetComponent<DebuffManager>();
                Debug.Log("Debuffs assigned.");
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            float dist = Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
            if (dist < 5f && !terminalController.isTerminalVisible)
            {
                if (isCorrupted)
                {
                    Debug.Log("This file is corrupted. You cannot read it.");
                    debuffChance = UnityEngine.Random.Range(10,0);
                    if(debuffChance <= 5){
                        Debug.Log("DebuffScript: " + debuffManager.currDebuffLength);
                        debuffManager.ApplyDebuff(DebuffType.Corruption);
                        debuffManager.ApplyDebuff(DebuffType.FirewallRage);
                    } else if(debuffChance >= 6){
                        Debug.Log("DebuffScript: " + debuffManager.currDebuffLength);
                        debuffManager.ApplyDebuff(DebuffType.Slow);
                        debuffManager.ApplyDebuff(DebuffType.FirewallRage);
                    }
                }
                else if (isWin)
                {
                    Debug.Log("You found the WIN file!");
                    if (logicManager != null)
                    {
                        logicManager.TriggerGameOver(); // <- your existing method
                    }
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

    public void OpenEditor()
    {
        GameObject editor = Instantiate(fileEditor);
        FileEditor editorScript = editor.GetComponent<FileEditor>();
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
        terminalController.LogToTerminal(editorScript.inputField.text);
    }

    string GenerateRandomFileName()
    {
        string[] names = { "system", "config", "report", "log", "data", "tmp", "cache", "session" };
        string[] extensions = { ".txt", ".log", ".dat", ".bin", ".cfg", ".tmp" };
        return names[UnityEngine.Random.Range(0, names.Length)] + UnityEngine.Random.Range(100, 999) + extensions[UnityEngine.Random.Range(0, extensions.Length)];
    }
}
