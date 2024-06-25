using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuestionDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtQuestionNumber;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private Button[] btnAnswers;
    [SerializeField] private TextMeshProUGUI[] txtAnswers;

    private bool IsEvaluating = false;

    private readonly string apiUrl = "https://localhost:44328/Question";

    // Start is called before the first frame update
    void Awake() {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        txtQuestionNumber.text = "Question " + GameManager.Instance.QuestionNumber + ":";
        StartCoroutine(SetupQuestion());
    }

    private void OnDisable()
    {
        IsEvaluating = false;
        errorMessage.text = string.Empty;
    }

    private IEnumerator SetupQuestion()
    {
        ResetQuestion();
        using var request = new UnityWebRequest(apiUrl + "/Random", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.responseCode);
        }
        else
        {
            Debug.Log("Get question successfully.");
            var json = request.downloadHandler.text;
            var question = JsonConvert.DeserializeObject<Question>(json);
            Debug.Log(json);

            txtTitle.text = question.Title;
            for (int i = 0; i < 4; i++)
            {
                int answerId = question.Answers[i].AnswerId;
                int questionId = question.QuestionId;

                txtAnswers[i].text = question.Answers[i].Content;
                btnAnswers[i].onClick.AddListener(() => StartCoroutine(EvaluateAnswer(questionId, answerId)));
            }
        }

        // Update is called once per frame
        IEnumerator EvaluateAnswer(int questionId, int answerId)
        {
            if (IsEvaluating) yield break;
            IsEvaluating = true;
            var evaluateRequest = new EvaluateQuestionRequest { QuestionId = questionId, AnswerId = answerId };
            var jsonBody = JsonConvert.SerializeObject(evaluateRequest);

            using var request = new UnityWebRequest(apiUrl + "/Check", "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            Time.timeScale = 1f;

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                errorMessage.color = Color.red;
                errorMessage.text = "Wrong answer!!!";
                yield return new WaitForSeconds(0.3f);
                GameManager.Instance.WrongAnswer();
            }
            else
            {
                errorMessage.color = Color.green;
                errorMessage.text = "Correct!!!";
                yield return new WaitForSeconds(0.3f);
                GameManager.Instance.CorrectAnswer();
            }
        }
    }

    private void ResetQuestion()
    {
        txtTitle.text = string.Empty;
        txtQuestionNumber.text = string.Empty;
        for (int i = 0; i < 4; i++)
        {
            txtAnswers[i].text = string.Empty;
            btnAnswers[i].onClick.RemoveAllListeners();
        }
    }
}

public class EvaluateQuestionRequest
{
    public int QuestionId { get; set; }
    public int AnswerId { get; set; }
}