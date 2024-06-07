using UnityEngine;

public class SceneMoveButton : MonoBehaviour
{
    public void MoveScene(string sceneName)
    {
        SceneLoader.Instance.LoadScene(sceneName);
    }
}
