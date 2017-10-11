using UnityEngine.UI;
using UnityEngine;
class ArcherButton : IClassSelectState
{
    ClassButtonControl classButtonControl;
    GameObject archerDescriptionWindow;
    GameObject archer;

    public ArcherButton(ClassButtonControl characterSelectControl, GameObject archerDescriptionWindow, GameObject archer)
    {
        this.classButtonControl = characterSelectControl;
        this.archerDescriptionWindow = archerDescriptionWindow;
        this.archer = archer;
    }
    public void InputWarriorButton()
    {
        archerDescriptionWindow.SetActive(false);
        archer.SetActive(false);
        classButtonControl.SetState(classButtonControl.WarriorState);
        classButtonControl.State.InputWarriorButton();
    }
    public void InputMagicianButton()
    {
        archerDescriptionWindow.SetActive(false);
        archer.SetActive(false);
        classButtonControl.SetState(classButtonControl.MagicianState);
        classButtonControl.State.InputMagicianButton();
    }
    public void InputArcherButton()
    {
        archerDescriptionWindow.SetActive(true);
        archer.SetActive(true);
    }
}
