using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard.rKey.wasPressedThisFrame)
        {
            StartSceneLoad();
        }

    }
    public void StartSceneLoad()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartSceneLoad(string _sceneName)
    {
        Debug.Log("StartSceneLoad");
        SceneManager.LoadScene(_sceneName);
    }
}
