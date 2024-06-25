using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayAgainHandler : MonoBehaviour
{
    [SerializeField] private Button playAgain;

    private void Awake()
    {
        playAgain.onClick.AddListener(() => PlayAgain());
    }

    public void PlayAgain() => SceneManager.LoadScene("LevelScene");
}
