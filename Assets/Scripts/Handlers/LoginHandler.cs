using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoginHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField txtUsername;
    [SerializeField] private TMP_InputField txtPassword;
    [SerializeField] private Button btnLogin;
    [SerializeField] private TextMeshProUGUI txtError;
    private readonly string apiUrl = "https://localhost:44328/Account/Login";

    private void Awake() => btnLogin.onClick.AddListener(() => OnButtonClickLogin());

    private void OnButtonClickLogin()
    {
        StopAllCoroutines();
        StartCoroutine(HandleLogin());
    }
    private IEnumerator HandleLogin()
    {
        if (string.IsNullOrEmpty(txtUsername.text) || string.IsNullOrEmpty(txtPassword.text))
        {
            ShowError("Please enter your username and password");
            yield return new WaitForSeconds(3f);
            HideError();
            yield break;
        }

        var loginRequest = new LoginRequest { Username = txtUsername.text, Password = txtPassword.text };
        var jsonBody = JsonConvert.SerializeObject(loginRequest);

        var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.responseCode);
            ShowError("Username or password is not correct.");
            yield return new WaitForSeconds(3f);
            HideError();
        }
        else
        {
            Debug.Log("Login successfully.");
            SceneManager.LoadScene("LevelScene");
        }
    }

    private void ShowError(string message) => txtError.text = message;
    private void HideError() => txtError.text = string.Empty;

    private class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
