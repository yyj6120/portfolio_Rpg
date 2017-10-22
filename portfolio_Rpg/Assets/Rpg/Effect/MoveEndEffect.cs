using UnityEngine;

public class MoveEndEffect : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        Invoke("off", 0.3f);
    }
    void off()
    {
        gameObject.SetActive(false);
    }
}
