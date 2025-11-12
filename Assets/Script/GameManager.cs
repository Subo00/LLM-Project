using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject endingPanel;
    [SerializeField] private TMP_Text text;
    public List<Script.SmartBehaviourAgent> agents = new List<Script.SmartBehaviourAgent>();


    void Awake()
    {
         Instance = this;
    }

    public void EndGame(bool isWin)
    {
        endingPanel.SetActive(true);
        text.text = isWin ? "Congradulation!" : "Oh no....";
    }

    public Script.SmartBehaviourAgent GetOtherAgent(Script.SmartBehaviourAgent SmartBehaviourAgent)
    {
        foreach(var agent in agents)
        {
            if(SmartBehaviourAgent != agent) return SmartBehaviourAgent;
        }
        return null;
    }
}
