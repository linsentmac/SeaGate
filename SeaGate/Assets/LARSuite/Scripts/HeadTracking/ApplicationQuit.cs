using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace LARSuite
{
public class ApplicationQuit : MonoBehaviour
{
    private int sceneIndex = -1;
    private LarManager larManager = null;

    void Awake()
    {
        //    Debug.Log("Input.backButtonLeavesApp " + Input.backButtonLeavesApp );
        var go = GameObject.FindGameObjectWithTag("LarCamera");
        if (go != null) larManager = go.GetComponent<LarManager>();

        Input.backButtonLeavesApp = false;

        var activeScene = SceneManager.GetActiveScene();
        sceneIndex = activeScene.buildIndex;
    }

    void Update()
    {
        if (!LarPlugin.Instance.IsInitialized())
            return;

        if (!Input.backButtonLeavesApp || Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Application Back");

            // Load next scene in build settings, quit when done
            if (++sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                larManager.StopAllCoroutines();
                LarPlugin.Instance.EndAR();
                LarPlugin.Instance.Shutdown();
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Application.Quit();
            }
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            LarPlugin.Instance.RecenterTracking();
        }
    }
}
}