using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System.Collections;

public class QuizHandler : MonoBehaviour
{
    public VideoPlayer videoPlayer;   // The VideoPlayer component
    public GameObject quizPanel;      // The UI panel that contains quiz elements
    public TextMeshProUGUI questionText;         // Text element for displaying the question
    public Button[] answerButtons;    // Buttons for the answers
    public TextMeshProUGUI feedbackText;         // Text for feedback
    public Question[] questions;      // An array of questions

    private int currentQuestionIndex = 0;

    void Start()
    {
        quizPanel.SetActive(false); // Hide the quiz initially
        StartCoroutine(StartQuizAfterDelay(10f)); // Start quiz after 10 seconds for debugging
    }

    // Coroutine to start the quiz after a delay
    IEnumerator StartQuizAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartQuiz();
    }

    // Start the quiz when video ends
    public void StartQuiz()
    {
        currentQuestionIndex = 0;
        quizPanel.SetActive(true); // Show the quiz panel
        ShowNextQuestion();
    }

    // Display the next question
    void ShowNextQuestion()
    {
        if (currentQuestionIndex < questions.Length)
        {
            Question currentQuestion = questions[currentQuestionIndex];
            questionText.text = currentQuestion.text;

            // Set up answer buttons
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (i < currentQuestion.answers.Length)
                {
                    answerButtons[i].gameObject.SetActive(true);
                    answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i].answerText;
                    answerButtons[i].onClick.RemoveAllListeners(); // Clear previous listeners
                    int index = i; // Capture index for the listener
                    answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false); // Hide unused buttons
                }
            }
        }
        else
        {
            EndQuiz(); // End quiz if no more questions
        }
    }

    // Handle answer selection
    void OnAnswerSelected(int answerIndex)
    {
        Question currentQuestion = questions[currentQuestionIndex];

        if (currentQuestion.answers[answerIndex].isCorrect)
        {
            feedbackText.text = "Correct!";
        }
        else
        {
            feedbackText.text = "Wrong!";
        }

        currentQuestionIndex++;
        Invoke("ShowNextQuestion", 2f); // Move to the next question after 2 seconds
    }

    // End the quiz
    void EndQuiz()
    {
        feedbackText.text = "Quiz Completed!";
        quizPanel.SetActive(false); // Hide quiz panel
    }
}

[System.Serializable]
public class Question
{
    public string text;      // The quiz question text
    public Answer[] answers; // An array of possible answers
}

[System.Serializable]
public class Answer
{
    public string answerText; // The answer text
    public bool isCorrect;    // Whether this is the correct answer
}