using UnityEngine;


namespace Script
{
    public class PlushieActions : MonoBehaviour
    {
        public void Forgive()
        {
            Debug.Log("WE MADE IT");
            GameManager.Instance.EndGame(true);
        }

        public void StayAngryForever()
        {
            GameManager.Instance.EndGame(false);
        }
    }

}

