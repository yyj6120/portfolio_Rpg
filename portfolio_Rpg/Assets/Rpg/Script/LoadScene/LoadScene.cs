using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public string sceneName;
    public void OnLoadScene()
    {
        Application.LoadLevel(sceneName);
    }
}
