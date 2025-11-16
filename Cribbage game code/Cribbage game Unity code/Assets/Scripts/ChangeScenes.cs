using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    //Change the scene to a scene specified by the parameter
    public void ChangeToScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}