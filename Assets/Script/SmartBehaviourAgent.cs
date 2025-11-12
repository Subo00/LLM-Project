using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Script.Enum;
using Script.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Script
{
    public class SmartBehaviourAgent : MonoBehaviour
    {
        [Tooltip("Where the model is hosted")] [SerializeField]
        private string apiBaseUrl = "http://localhost:11434/api/generate";

        [Tooltip("The target model to use for inference")] [SerializeField]
        private Model targetModel = Model.Lama32;
        public string[] flagsSet = new string[]{};
        // The base prompt for basic context and structure guidelines
        private readonly string _basePrompt = "You are an agent simulating a video game character. " +
                                              "You'll respond using a JSON structure " +
                                              "with two fields: 'answer' and 'flags'. " +
                                              "Always use both fields even if they are empty. " +
                                              "Always be sure to put the JSON fields between the double quotes. " +
                                              $"This is an example: {JsonUtility.ToJson(new AgentResponseJSON())}";

        // Keep the conversation context vector with this agent
        private List<long> _context = new();

        [Tooltip("The type of this agent. You can also create your own configuration asset.")] [SerializeField]
        private AgentConfig agentConfig;

        [Tooltip("The agent will have also this specific behaviour in addition to the configuration behavior.")]
        [SerializeField]
        [TextArea]
        private string specificBehaviour;

        // The agents answer panel
        private TMP_Text _agentAnswerText;

        private bool _selected;

        private void Awake()
        {
            // avoid manual references foreach new agent
            _agentAnswerText = GameObject.FindWithTag("AgentAnswer").GetComponent<TMP_Text>();
        }

        public void Start()
        {
            GameManager.Instance.agents.Add(this);
        }

        public void SetSelected(bool selected)
        {
            // The player has selected or unselected this agent
            _selected = selected;
        }

        public void Talk(string message)
        {
            // Send a message to the LLM model
            SendApiRequest(message);
        }

        private void PerformAgentAction(string action)
        {
            // Request an action using method names
            // Just create your own actions script for an agent that will handle all possible actions
            gameObject.SendMessage(action, false);
        }

        private void SendApiRequest(string message)
        {
            if (!_selected) return;
            _agentAnswerText.text = "<Thinking>";

            // A standard HTTP call to the model server
            StartCoroutine(SendApiRequestCoroutine(message));
        }

        private IEnumerator SendApiRequestCoroutine(string prompt)
        {
            var systemPrompt = $"{_basePrompt}\n" +
                               $"{agentConfig.ContextPrompt}\n" +
                               $"{agentConfig.AnswerGuideline}\n" +
                               $"{agentConfig.ActionsGuideline}\n" +
                               $"{specificBehaviour}";
            string flagsArray = "["+String.Join("\",\"",GameManager.Instance.GetOtherAgent(this).flagsSet)+"]";
            var requestData = new RequestData
            {
                model = targetModel.GetString(),
                system = systemPrompt,
                prompt = $"{{\"text\":\"{prompt}\",\"flags\":{flagsArray}}}",
                options = new RequestData.Options { num_ctx = 8192 },
                stream = false,
                context = _context
            };

            var jsonBody = JsonUtility.ToJson(requestData);

            using var request = new UnityWebRequest(apiBaseUrl, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            Debug.Log(request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                var jsonResponse = request.downloadHandler.text;

                // Parse the model server response
                var responseData = JsonUtility.FromJson<ApiResponseData>(jsonResponse);

                if (!string.IsNullOrEmpty(responseData.response))
                {
                    try
                    {
                        // Parse the agent response
                        AgentResponseJSON parsedResponse = JsonUtility.FromJson<AgentResponseJSON>(responseData.response);
                        _agentAnswerText.text = parsedResponse.answer;
                        this.flagsSet = parsedResponse.flags;
                        foreach(var flag in parsedResponse.flags)
                        {
                            if(flag == "FORGIVEN")
                            {
                                PerformAgentAction("Forgive");
                            }
                            else if(flag == "ANGRY_FOREVER")
                            {
                                PerformAgentAction("StayAngryForever");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _agentAnswerText.text = agentConfig.DefaultErrorAnswer;
                        Debug.LogError($"Response parsing failed: {responseData.response}");
                        Debug.LogError($"Error: {e}");

                        Debug.Log($"Request: {jsonBody}");
                        Debug.Log($"Response: {jsonResponse}");
                    }
                }

                if (responseData.context != null)
                {
                    _context = responseData.context;
                }
            }
            else
            {
                Debug.LogError("Request failed.");
                _agentAnswerText.text = agentConfig.DefaultErrorAnswer;
            }
        }
    }
}