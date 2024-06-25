using UnityEngine;

public class DeathBerrier : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
           StartCoroutine(GameManager.Instance.GameOver());
        }
        else
        {
            Destroy(other.gameObject);
        }
    }

}
