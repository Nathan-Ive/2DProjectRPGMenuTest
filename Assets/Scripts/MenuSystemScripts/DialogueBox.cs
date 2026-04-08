using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;

public class DialogueBox : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    [Header("Input")]
    public InputManager inputManager;

    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueEnded;

    private List<string> currentLines = new List<string>();
    private int currentLineIndex = 0;
    private bool isActive = false;

    public void StartDialogue(List<string> lines)
    {
        if (lines == null || lines.Count == 0)
            return;

        currentLines = lines;
        currentLineIndex = 0;
        isActive = true;

        dialoguePanel.SetActive(true);
        dialogueText.text = currentLines[0];

        inputManager.OnConfirmPressed.AddListener(AdvanceDialogue);
        OnDialogueStarted?.Invoke();
    }

    void AdvanceDialogue()
    {
        if (!isActive)
            return;

        currentLineIndex++;

        if (currentLineIndex < currentLines.Count)
        {
            dialogueText.text = currentLines[currentLineIndex];
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isActive = false;
        dialoguePanel.SetActive(false);
        inputManager.OnConfirmPressed.RemoveListener(AdvanceDialogue);
        OnDialogueEnded?.Invoke();
    }
}