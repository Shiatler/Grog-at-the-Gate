using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-100)]
public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public GameObject dialoguePanel;

    private Queue<string> sentences;
    private Queue<Sound> sounds;
    private AudioManager audioManager;
    private string currentPlayingSound; // Track currently playing sound

    // Start is called before the first frame update #############################
    void Start()
    {
        sentences = new Queue<string>();
        sounds = new Queue<Sound>();
        audioManager = FindObjectOfType<AudioManager>();
    }
    // Start the dialogue #########################################################
    public void StartDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);
        HandleUI handleUI = GameObject.FindObjectOfType<HandleUI>();
        if (handleUI != null)
        {
            handleUI.hideUI();
        }

        Time.timeScale = 0f;
        
        nameText.text = dialogue.name;

        sentences.Clear();
        sounds.Clear();

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            sentences.Enqueue(dialogue.sentences[i]);
            if (dialogue.sounds != null && i < dialogue.sounds.Length && dialogue.sounds[i] != null)
            {
                sounds.Enqueue(dialogue.sounds[i]);
            }
            else
            {
                sounds.Enqueue(null); // Keep queue in sync
            }
        }

        DisplayNextSentence();
    }

    // Display the next sentence ##################################################
    public void DisplayNextSentence()
    {
        // Deselect button so hover effect works again
        EventSystem.current.SetSelectedGameObject(null);
        
        // Stop current sound if playing (for spam clicking)
        if (!string.IsNullOrEmpty(currentPlayingSound) && audioManager != null)
        {
            audioManager.Stop(currentPlayingSound);
        }
        
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Play corresponding sound for this sentence
        if (sounds.Count > 0)
        {
            Sound sound = sounds.Dequeue();
            if (sound != null && audioManager != null)
            {
                currentPlayingSound = sound.name;
                audioManager.Play(sound.name);
            }
            else
            {
                currentPlayingSound = null;
            }
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    // Type the sentence letter by letter #######################################
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    // End the dialogue ##########################################################
    void EndDialogue()
    {
        // Stop current sound if playing
        if (!string.IsNullOrEmpty(currentPlayingSound) && audioManager != null)
        {
            audioManager.Stop(currentPlayingSound);
            currentPlayingSound = null;
        }
        
        dialoguePanel.SetActive(false);
        Time.timeScale = 1f;
        HandleUI handleUI = GameObject.FindObjectOfType<HandleUI>();
        if (handleUI != null)
        {
            handleUI.showUI();
        }
    }
}
