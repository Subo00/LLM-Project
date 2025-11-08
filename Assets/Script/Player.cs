using TMPro;
using UnityEngine;

namespace Script
{
    public class Player : MonoBehaviour
    {
        // The prompt input field
        [SerializeField] private TMP_InputField playerPromptInput;

        // The selected agent reference
        private GameObject _target;

        // We want to show which agent is selected
        [SerializeField] private TMP_Text selectedTargetText;

        // We want to rotate towards the selected agent
        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            // Check if we clicked with the mouse left button
            if (!Input.GetMouseButtonDown(0)) return;
            // Check if we hit something
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;
            // Check if we hit an actual agent using the object tag
            var selectedObject = hit.collider.gameObject;
            if (!selectedObject.tag.Equals("SmartAgent")) return;

            // Select the target agent and look at him
            selectedTargetText.text = $"Talking to: {selectedObject.name}";
            transform.rotation = Quaternion.LookRotation(selectedObject.transform.position - transform.position);
            UpdateAgentSelection(_target, selectedObject);
            _target = selectedObject;
        }

        public void TalkToSelectedAgent()
        {
            // If no agent has been selected, just clear the input
            if (!_target)
            {
                playerPromptInput.text = "";
                return;
            }

            if (string.IsNullOrEmpty(playerPromptInput.text)) return;
            var message = playerPromptInput.text;
            playerPromptInput.text = "";

            // Say something to the agent
            _target.SendMessage("Talk", message);
        }

        private void UpdateAgentSelection(GameObject oldTarget, GameObject newTarget)
        {
            if (oldTarget)
                oldTarget.SendMessage("SetSelected", false);
            newTarget.SendMessage("SetSelected", true);
        }
    }
}