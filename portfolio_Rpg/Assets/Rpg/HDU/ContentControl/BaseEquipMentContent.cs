using UnityEngine;
public class BaseEquipMentContent : IContent
{
    ContentControl contentControl;
    GameObject baseEquipment;
    public BaseEquipMentContent(ContentControl contentControl , GameObject baseEquipment)
    {
        this.contentControl = contentControl;
        this.baseEquipment = baseEquipment;
        this.baseEquipment.SetActive(false);
    }
    public void InputClassSelectContent()
    {
        baseEquipment.SetActive(false);
        contentControl.SetContent(contentControl.ClassSelect);
        contentControl.Content.InputClassSelectContent();
    }

    public void InputBaseEquipment()
    {
        baseEquipment.SetActive(true);
    }

    public void InputNameSetting()
    {
        baseEquipment.SetActive(false);
        contentControl.SetContent(contentControl.NameSetting);
        contentControl.Content.InputNameSetting();
    }
}
