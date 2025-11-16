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

        private IEnumerator SendApiRequestCoroutine(string promptInput)
        {
            var systemPrompt = $"{_basePrompt}\n" +
                               $"{agentConfig.ContextPrompt}\n" +
                               $"{agentConfig.AnswerGuideline}\n" +
                               $"{agentConfig.ActionsGuideline}\n" +
                               $"{specificBehaviour}";
            string flagsArray = "["+String.Join("\",\"",GameManager.Instance.GetOtherAgent(this).flagsSet)+"]";
            string prompt = $"{{\"answer\":\"{promptInput}\",\"flags\":{flagsArray},'sentBy':'Player'}}";
            var requestData = new RequestData
            {
                model = targetModel.GetString(),
                system = systemPrompt,
                prompt = prompt,
                options = new RequestData.Options { num_ctx = 8192 },
                stream = false,
                context = _context
            };
            ResponseProblem problem = ResponseProblem.None;
            for(int x = 0; x < 4; x++)
            {
                if(x > 0)
                {
                    switch(problem)
                    {
                        case ResponseProblem.InvalidJSON:
                            requestData.prompt = $"YOUR RESPONSE MUST BE VALID JSON, TRY AGAIN, and answer in character";
                            break;
                        case ResponseProblem.InvalidMessage:
                            requestData.prompt = $"Try again, you must enter an answer";
                            break;
                        case ResponseProblem.InvalidTags:
                            requestData.prompt = $"Try again, your tags were invalid";
                            break;
                        case ResponseProblem.None:
                            requestData.prompt = $"Try again\n{prompt}";
                            break;
                    }
                }
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
                            if(parsedResponse.answer == "")
                            {
                                problem = ResponseProblem.InvalidMessage;
                                continue;
                            }
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
                            if (responseData.context != null)
                            {
                                _context = responseData.context;
                            }
                            break; // We got a valid response so we exit out of the loop.
                        }
                        catch (Exception e)
                        {
                            if (responseData.context != null)
                            {
                                _context = responseData.context;
                            } // Remember what we sent before
                            // We didnt get valid json so we try again and send an extra hint to the llm
                            problem = ResponseProblem.InvalidJSON;
                            _agentAnswerText.text = agentConfig.DefaultErrorAnswer;
                            Debug.LogError($"Response parsing failed: {responseData.response}");
                            Debug.LogError($"Error: {e}");

                            Debug.Log($"Request: {jsonBody}");
                            Debug.Log($"Response: {jsonResponse}");
                        }
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

    public enum ResponseProblem
    {
        None,
        InvalidJSON,
        InvalidTags,
        InvalidMessage
    }
}