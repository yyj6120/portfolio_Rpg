using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

interface IClassSelectState
{
    void InputWarriorButton();
    void InputMagicianButton();
    void InputArcherButton();
}

class ClassButtonControl : MonoBehaviour
{
    #region Variables
    public GameObject warriorWindow;
    public GameObject warriorObject;

    public GameObject magicionWindow;
    public GameObject magicionObject;

    public GameObject archerWindow;
    public GameObject archerObject;

    public GameObject currentObject;

    IClassSelectState warriorState;
    IClassSelectState magicianState;
    IClassSelectState archerState;
    IClassSelectState state;

    internal IClassSelectState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    internal IClassSelectState WarriorState
    {
        get
        {
            return warriorState;
        }

        set
        {
            warriorState = value;
        }
    }

    internal IClassSelectState MagicianState
    {
        get
        {
            return magicianState;
        }

        set
        {
            magicianState = value;
        }
    }

    internal IClassSelectState ArcherState
    {
        get
        {
            return archerState;
        }

        set
        {
            archerState = value;
        }
    }
    #endregion
    #region Initialize BaseClass
    private void Start()
    {
        warriorState = new WarriorButton(this , warriorWindow , warriorObject);
        magicianState = new MagicionButton(this , magicionWindow , magicionObject);
        archerState = new ArcherButton(this , archerWindow , archerObject);
        State = WarriorState;
        State.InputWarriorButton();
    }
    #endregion
    #region State Method , StateChage
    public void InputWarriorButton()
    {
        State.InputWarriorButton();
    }

    public void InputMagicianButton()
    {
        State.InputMagicianButton();
    }

    public void InputArcherButton()
    {
        State.InputArcherButton();
    }
    public void SetState(IClassSelectState state)
    {
        this.State = state;
    }
    #endregion
}
