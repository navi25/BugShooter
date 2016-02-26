using UnityEngine;
using System.Collections;

namespace Gamestrap
{
    public class GameplayUI : MonoBehaviour
    {

        public GameObject pausePanel;

        private bool pause;


        /// <summary>
        /// It activates the pause animation in the pause panel
        /// </summary>
        public bool Pause
        {
            get { return pause; }
            set
            {
                pause = value;

                if (pause)
                {
                    pausePanel.GetComponent<Animator>().SetBool("Visible", true);
                }
                else
                {
                    pausePanel.GetComponent<Animator>().SetBool("Visible", false);
                }
            }
        }

    }
}