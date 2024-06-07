using UnityEngine;

public class SceneMoveButton : MonoBehaviour
{
    private bool isMoved = false;

    public void MoveScene(string sceneName)
    {
        // 연속으로 눌러지는 걸 막는다.
        if (isMoved)
        {
            return;
        }

        SceneLoader.Instance.LoadScene(sceneName);
        isMoved = true;
    }
}
