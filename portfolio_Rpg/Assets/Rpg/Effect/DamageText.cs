using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public Text Damage;
    void Awake()
    {
        Damage = transform.GetChild(0).GetComponent<Text>();
    }
    private void OnEnable()
    {
        SetDamagePlayText();
    }
    IEnumerator StartEffect()
    {
        float StartTime = Time.time;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            float timePassed = Time.time - StartTime;
            float rate = timePassed / 1f;
            Damage.color = new Color(1, 1f, 0.2f, 1f - rate);
            if (timePassed > 1f)
            {
                break;
            }
        }
    }
    void Off()
    {
        gameObject.SetActive(false);
    }
    public void SetDamagePlayText()
    {
       // iTween.MoveBy(gameObject, new Vector3(0, Random.Range(0.5f, 1.7f), 0f), 1f);
        StartCoroutine("StartEffect");
        Invoke("Off", 1.5f);
    }
}
