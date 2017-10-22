using UnityEngine;
using System.Collections;

public class DestroyGameObject : MonoBehaviour
{
    public float delay;

    private void OnEnable()
    {

    }
    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
