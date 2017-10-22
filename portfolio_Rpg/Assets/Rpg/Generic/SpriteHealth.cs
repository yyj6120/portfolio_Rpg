﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rpg.Character;
using UnityEngine.UI;
public class SpriteHealth : MonoBehaviour
{
    public Character iChar;
    public Slider healthSlider;
    public Slider damageDelay;
    public float smoothDamageDelay;
    public Text damageCounter;
    public float damageCounterTimer = 1.5f;

    private bool inDelay;
    private float damage;
    private float currentSmoothDamage;

    void Start()
    {
        iChar = transform.GetComponentInParent<Character>();
        if (iChar == null)
        {
            Debug.LogWarning("The character must have a ICharacter Interface");
            Destroy(this.gameObject);
        }
        healthSlider.maxValue = iChar.maxHealth;
        healthSlider.value = healthSlider.maxValue;
        //damageDelay.maxValue = iChar.maxHealth;
        //damageDelay.value = healthSlider.maxValue;
        //damageCounter.text = string.Empty;
    }

    void Update()
    {
        SpriteBehaviour();
    }

    void SpriteBehaviour()
    {
        if (Camera.main != null)
            transform.LookAt(Camera.main.transform.position, Vector3.up);

        if (iChar == null || iChar.currentHealth <= 0)
            Destroy(gameObject);

        healthSlider.value = iChar.currentHealth;
    }

    public void Damage(float value)
    {
        try
        {
            healthSlider.value -= value;

            this.damage += value;
            damageCounter.text = damage.ToString("00");
            if (!inDelay)
                StartCoroutine(DamageDelay());
        }
        catch
        {
            Destroy(this);
        }
    }

    IEnumerator DamageDelay()
    {
        inDelay = true;
        while (damageDelay.value > healthSlider.value)
        {
            damageDelay.value -= smoothDamageDelay;
            yield return null;
        }
        inDelay = false;
        damage = 0;
        yield return new WaitForSeconds(damageCounterTimer);
        damageCounter.text = string.Empty;
    }
}