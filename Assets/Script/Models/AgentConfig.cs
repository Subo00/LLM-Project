using UnityEngine;

namespace Script.Models
{
    [CreateAssetMenu(fileName = "AgentConfig", menuName = "SmartBehaviour/AgentConfig", order = 1)]
    public class AgentConfig : ScriptableObject
    {
        [SerializeField] private string defaultErrorAnswer = "";

        [TextArea(5, 20)] [SerializeField] private string contextPrompt = "";
        [TextArea(5, 20)] [SerializeField] private string answerGuideline = "";
        [TextArea(5, 20)] [SerializeField] private string actionsGuideline = "";

        public string DefaultErrorAnswer => defaultErrorAnswer;
        public string ContextPrompt => contextPrompt;
        public string AnswerGuideline => answerGuideline;
        public string ActionsGuideline => actionsGuideline;
    }
}