using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    /*
     * A basic example script that maps the possible actions of a guard-type agent.
     * You can create your own actions script foreach agent and program any type of logic.
     */
    public class GuardActions : MonoBehaviour
    {
        private AudioSource _shotSound;
        private Image _gameOverPanel;

        private Animation _doorOpenAnim;

        private void Awake()
        {
            // avoid manual references foreach new agent
            _shotSound = GetComponent<AudioSource>();
            _gameOverPanel = GameObject.FindWithTag("GameOverPanel").GetComponent<Image>();
            _doorOpenAnim = GameObject.FindWithTag("Door").GetComponent<Animation>();
        }

        public void Shot()
        {
            // A demo action that performs a gunshot
            _shotSound.Play();
            _gameOverPanel.enabled = true;
        }

        public void OpenDoor()
        {
            // A demo action that performs a door opening
            _doorOpenAnim.Play();
        }
    }
}