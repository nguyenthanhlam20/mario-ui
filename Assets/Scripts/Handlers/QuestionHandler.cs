using UnityEngine;

public class QuestionHandler : MonoBehaviour
{
    [SerializeField] private QuestionDisplayer questionDisplayer;

    private void Awake() => questionDisplayer.gameObject.SetActive(false);

    public void ShowQuestion() => questionDisplayer.gameObject.SetActive(true);

    public void HideQuestion() => questionDisplayer.gameObject.SetActive(false);
}
