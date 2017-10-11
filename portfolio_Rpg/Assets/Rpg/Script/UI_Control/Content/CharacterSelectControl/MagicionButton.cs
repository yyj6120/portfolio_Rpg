using UnityEngine.UI;
using UnityEngine;

class MagicionButton : IClassSelectState
{
    ClassButtonControl classButtonControl;
    GameObject magicionDescriptionWindow;
    GameObject magicion;

    public MagicionButton(ClassButtonControl characterSelectControl, GameObject magicionDescriptionWindow, GameObject magicion)
    {
        this.classButtonControl = characterSelectControl;
        this.magicionDescriptionWindow = magicionDescriptionWindow;
        this.magicion = magicion;
    }
    public void InputWarriorButton()
    {
        magicionDescriptionWindow.SetActive(false);
        magicion.SetActive(false);
        classButtonControl.SetState(classButtonControl.WarriorState);
        classButtonControl.State.InputWarriorButton();
    }
    public void InputMagicianButton()
    {
        magicionDescriptionWindow.SetActive(true);
        magicion.SetActive(true);
    }
    public void InputArcherButton()
    {
        magicionDescriptionWindow.SetActive(false);
        magicion.SetActive(false);
        classButtonControl.SetState(classButtonControl.ArcherState);
        classButtonControl.State.InputArcherButton();
    }
}
