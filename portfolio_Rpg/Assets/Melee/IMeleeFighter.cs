using UnityEngine;
using System.Collections;
using Rpg.Character;

public interface IMeleeFighter
{
    void OnEnableAttack();

    void OnDisableAttack();

    void ResetAttackTriggers();

    void BreakAttack(int breakAtkID);

    void OnRecoil(int recoilID);

    void OnReceiveAttack(Damage damage, IMeleeFighter attacker);

    Character Character();
}

public static class IMeeleFighterHelper
{
    /// <summary>
    /// check if gameObject has a <see cref="vIMeleeFighter"/> Component
    /// </summary>
    /// <param name="receiver"></param>
    /// <returns>return true if gameObject contains a <see cref="vIMeleeFighter"/></returns>
    public static bool IsAMeleeFighter(this GameObject receiver)
    {
        return receiver.GetComponent<IMeleeFighter>() != null;
    }

    /// <summary>
    /// Get <see cref="vIMeleeFighter"/> of gameObject
    /// </summary>
    /// <param name="receiver"></param>
    /// <returns>the <see cref="vIMeleeFighter"/> component</returns>
    public static IMeleeFighter GetMeleeFighter(this GameObject receiver)
    {
        return receiver.GetComponent<IMeleeFighter>();
    }
}
