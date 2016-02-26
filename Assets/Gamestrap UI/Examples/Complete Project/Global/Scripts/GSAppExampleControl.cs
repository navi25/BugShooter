using UnityEngine;
using System.Collections;

namespace Gamestrap
{
    /// <summary>
    /// Main Control that handles all of the global Application logic, specially the scene transitions
    /// </summary>
    public class GSAppExampleControl : MonoBehaviour
    {
        public static GSAppExampleControl Instance;
        private static int VisibleVariable = Animator.StringToHash("Visible");

        public Animator faderAnimator;
        public AnimationClip fadingClip;

        void Awake()
        {
            if (Instance != null)
            {
                //ApplicationControl already exists
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(ESceneNames sceneName)
        {
            StartCoroutine(LoadSceneTransitions(sceneName));
        }

        private IEnumerator LoadSceneTransitions(ESceneNames sceneName)
        {
            faderAnimator.SetBool(VisibleVariable, true);
            yield return new WaitForEndOfFrame();

            yield return new WaitForSeconds(fadingClip.length);
#if UNITY_4_6
        //LoadLevelAsync wasn't in the free version of Unity in 4.6 so did this for compatibility reasons
        Application.LoadLevel(sceneName.ToString());
        while (Application.isLoadingLevel)
        {
            yield return new WaitForEndOfFrame();
        }
#else
            yield return Application.LoadLevelAsync(sceneName.ToString());
#endif
            faderAnimator.SetBool(VisibleVariable, false);
        }
    }
}