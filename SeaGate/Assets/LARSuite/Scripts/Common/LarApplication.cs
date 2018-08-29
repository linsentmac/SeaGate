using UnityEngine;
using System.Collections;

namespace LARSuite
{
    /// <summary>
    /// The main entry point for Lar application. It do necessary Lotus initialization and
    /// it maintains several services, like <see cref="ActionMainQueue"/>.
    /// </summary>
    public class LarApplication : MonoBehaviour {

        public static LarApplication Instance() {
            LarApplication instance = null;
            GameObject go = GameObject.Find("LarApplication");
            if (go) {
                instance = go.GetComponent<LarApplication>();
            }
        
            return instance;
        }

        void Awake() {
            FileManager.Instance.Init();
        }

        void Start() {

        }

        void Update() {

            if (!ActionMainQueue.Instance.IsEmpty()) {
               
                ActionMainQueue.Instance.ExecuteActions();
            }

        }

        public void Quit() {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
           
#else
        Application.Quit();
#endif

        }
    }
}


