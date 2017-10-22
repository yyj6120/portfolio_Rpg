using UnityEngine;
using System.Collections;

public class DisableGameObject : MonoBehaviour
{
    public float delay;
    private void OnEnable()
    {
        Debug.Log("1");
        StartCoroutine(GoDisable());
    }
    IEnumerator GoDisable()
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}

