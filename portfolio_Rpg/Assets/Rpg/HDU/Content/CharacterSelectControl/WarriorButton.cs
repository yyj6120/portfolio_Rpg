using UnityEngine.UI;
using UnityEngine;
class WarriorButton : MonoBehaviour, IClassSelectState
{
    ClassButtonControl classButtonControl;
    GameObject warriorDescriptionWindow;
    GameObject warrior;
    public WarriorButton(ClassButtonControl characterSelectControl , GameObject warriorDescriptionWindow , GameObject warrior)
    {
        this.classButtonControl = characterSelectControl;
        this.warriorDescriptionWindow = warriorDescriptionWindow;
        this.warrior = warrior;
    }
    public void InputWarriorButton()
    {
        warriorDescriptionWindow.SetActive(true);
        warrior.SetActive(true);
    }

    public void InputMagicianButton()
    {
        warriorDescriptionWindow.SetActive(false);
        warrior.SetActive(false);
        classButtonControl.SetState(classButtonControl.MagicianState);
        classButtonControl.State.InputMagicianButton();
    }

    public void InputArcherButton()
    {
        warriorDescriptionWindow.SetActive(false);
        warrior.SetActive(false);
        classButtonControl.SetState(classButtonControl.ArcherState);
        classButtonControl.State.InputArcherButton();
    }

}

