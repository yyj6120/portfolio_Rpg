﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Rpg.Item;

public class MeleeWeapon : MeleeAttackObject , IEquipment
{
    [Header("Melee Weapon Settings")]
    public MeleeType meleeType = MeleeType.OnlyAttack;
    [Header("Attack Settings")]
    public float distanceToAttack = 1;
    [Tooltip("Trigger a Attack Animation")]
    public int attackID;
    [Tooltip("Change the MoveSet when using this Weapon")]
    public int movesetID;
    [Tooltip("How much stamina will be consumed when attack")]
    public float staminaCost;
    [Tooltip("How much time the stamina will wait to start recover")]
    public float staminaRecoveryDelay;
    [Header("Defense Settings")]
    [Range(0, 100)]
    public int defenseRate = 100;
    [Range(0, 180)]
    public float defenseRange = 90;
    [Tooltip("Trigger a Defense Animation")]
    public int defenseID;
    [Tooltip("What recoil animatil will trigger")]
    public int recoilID;
    [Tooltip("Can break the oponent attack, will trigger a recoil animation")]
    public bool breakAttack;

    [HideInInspector]
    public UnityEngine.Events.UnityEvent onDefense;

    public void OnEquip(Item item)
    {
        var damage = item.attributes.Find(attribute => attribute.name == Rpg.Item.ItemAttributes.Damage);
        var staminaCost = item.attributes.Find(attribute => attribute.name == Rpg.Item.ItemAttributes.StaminaCost);
        var defenseRate = item.attributes.Find(attribute => attribute.name == Rpg.Item.ItemAttributes.DefenseRate);
        var defenseRange = item.attributes.Find(attribute => attribute.name == Rpg.Item.ItemAttributes.DefenseRange);

        if (damage != null)
            this.damage.damageValue = damage.value;
        if (staminaCost != null)
            this.staminaCost = staminaCost.value;
        if (defenseRate != null)
            this.defenseRate = defenseRate.value;
        if (defenseRange != null)
            this.defenseRange = defenseRate.value;
    }

    public void OnUnequip(Item item)
    {

    }

    public void OnDefense()
    {
        onDefense.Invoke();
    }
}

public enum MeleeType
{
    OnlyDefense, OnlyAttack, AttackAndDefense
}
