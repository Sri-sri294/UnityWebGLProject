using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.UI; // For Button

public class ITDTask : MonoBehaviour
{
    public AudioSource audioSource; // AudioSource for task sounds
    public AudioSource backgroundMusic; // Background music
    public AudioClip[] taskSounds; // Task sounds (beeps)
    public AudioClip correctSound; // Correct feedback sound
    public AudioClip wrongSound; // Wrong feedback sound

    public GameObject instructionPanel; // Instruction panel
    public Button startButton; // Start button
    public Button[] choiceButtons; // First, Second, Third buttons
    public TextMeshProUGUI questionText; // Question text
    public TextMeshProUGUI feedbackText; // Feedback text
    public TextMeshProUGUI scoreText; // Score text
    public TextMeshProUGUI trialCounterText; // Trial counter text

    private int correctChoiceIndex; // Index of the correct choice
    private int score = 0; // User's score
    private int currentTrial = 0; // Current trial count
    private const int totalTrials = 30; // Total number of trials

    void Start()
    {
        // Initial state: Only Start button and instructions visible
        instructionPanel.SetActive(true);
        startButton.gameObject.SetActive(true);

        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false); // Hide choice buttons
        }

        questionText.gameObject.SetActive(false); // Hide question text
        feedbackText.gameObject.SetActive(false); // Hide feedback text
        scoreText.gameObject.SetActive(false); // Hide score text
        trialCounterText.gameObject.SetActive(false); // Hide trial counter text

        backgroundMusic.Play(); // Start background music
        startButton.onClick.AddListener(StartTask); // Attach StartTask to the Start button
    }

    void StartTask()
    {
        // Transition to task state
        backgroundMusic.Stop(); // Stop background music
        instructionPanel.SetActive(false); // Hide instructions
        startButton.gameObject.SetActive(false); // Hide Start button

        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(true); // Show choice buttons
        }

        questionText.gameObject.SetActive(true); // Show question text
        feedbackText.gameObject.SetActive(false); // Hide feedback initially
        scoreText.gameObject.SetActive(true); // Show score text
        trialCounterText.gameObject.SetActive(true); // Show trial counter text

        currentTrial = 0; // Reset trial count
        StartCoroutine(StartTrials()); // Start the trials
    }

    IEnumerator StartTrials()
    {
        for (currentTrial = 1; currentTrial <= totalTrials; currentTrial++)
        {
            // Update trial counter and question text
            trialCounterText.text = $"Trial {currentTrial}/{totalTrials}";
            questionText.text = "Which sound came from a different direction?";
            feedbackText.gameObject.SetActive(false); // Hide feedback text during trial

            yield return PlayITDSounds(); // Play the ITD sounds

            // Enable choice buttons for user input
            foreach (Button button in choiceButtons)
            {
                button.interactable = true; // Make buttons clickable
            }

            // Wait for user to select an answer
            bool userMadeChoice = false;
            foreach (Button button in choiceButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    HandleChoice(button); // Handle the user's choice
                    userMadeChoice = true; // Mark that the user made a choice
                });
            }

            yield return new WaitUntil(() => userMadeChoice); // Wait until the user selects a choice

            yield return new WaitForSeconds(2f); // Pause for 2 seconds before next trial
        }

        EndTask(); // End the task after all trials
    }

    IEnumerator PlayITDSounds()
    {
        int correctIndex = Random.Range(0, 3); // Randomize the correct index
        correctChoiceIndex = correctIndex;

        int[] directions = { -1, -1, -1 }; // Default directions (all left)
        directions[correctIndex] = 1; // Correct sound from the right

        for (int i = 0; i < directions.Length; i++)
        {
            // Set the sound panning based on the direction
            audioSource.panStereo = directions[i] == -1 ? -1f : 1f;

            // Debug log for troubleshooting
            Debug.Log("Playing sound: " + taskSounds[i].name + " at direction: " + (directions[i] == -1 ? "Left" : "Right"));

            audioSource.PlayOneShot(taskSounds[i]); // Play the sound
            yield return new WaitForSeconds(1f); // Wait before the next sound
        }

        // Reset panning to the center
        audioSource.panStereo = 0f;
    }

    void HandleChoice(Button selectedButton)
    {
        // Check if the selected button is correct
        int selectedIndex = System.Array.IndexOf(choiceButtons, selectedButton);

        if (selectedIndex == correctChoiceIndex)
        {
            score++; // Update score
            feedbackText.text = "Correct!";
            feedbackText.color = Color.green;
            audioSource.PlayOneShot(correctSound);
        }
        else
        {
            feedbackText.text = "Wrong!";
            feedbackText.color = Color.red;
            audioSource.PlayOneShot(wrongSound);
        }

        feedbackText.gameObject.SetActive(true); // Show feedback text
        scoreText.text = $"Score: {score}"; // Update score text

        // Disable buttons after user selects an answer
        foreach (Button button in choiceButtons)
        {
            button.interactable = false; // Disable buttons
        }
    }

    void EndTask()
    {
        // Show task completion message
        questionText.text = "Task Complete! Thank you for participating.";
        feedbackText.gameObject.SetActive(false); // Hide feedback text
        trialCounterText.gameObject.SetActive(false); // Hide trial counter text

        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false); // Hide choice buttons
        }
    }
}
