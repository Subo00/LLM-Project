using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject endingPanel;
    [SerializeField] private TMP_Text text;


    void Awake()
    {
         Instance = this;
    }

    public void EndGame(bool isWin)
    {
        endingPanel.SetActive(true);
        text.text = isWin ? "Congradulation!" : "Oh no....";
    }
}
