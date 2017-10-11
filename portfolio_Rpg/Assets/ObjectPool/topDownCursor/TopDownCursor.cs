using UnityEngine;
using System.Collections;
using Rpg.Character;

public class TopDownCursor : MonoBehaviour
{
    public ThirdPersonInput topDownInput;
    public GameObject cursorObject;

    void Start()
    {
        if (!topDownInput)
            Destroy(gameObject);
        topDownInput.onEnableCursor = Enable;
        topDownInput.onDisableCursor = Disable;
    }

    public void Enable(Vector3 position)
    {
        cursorObject.SetActive(true);
        cursorObject.transform.position = position + (Vector3.up * 0.1f);
    }

    public void Disable()
    {
        cursorObject.SetActive(false);
    }

}
