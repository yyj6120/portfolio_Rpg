using UnityEngine;

public class NameSettingContent : IContent
{
    ContentControl contentControl;
    GameObject NameSet;
    public NameSettingContent(ContentControl contentControl, GameObject NameSet)
    {
        this.contentControl = contentControl;
        this.NameSet = NameSet;
        this.NameSet.SetActive(false);
    }

    public void InputClassSelectContent()
    {
        NameSet.SetActive(false);
        contentControl.SetContent(contentControl.ClassSelect);
        contentControl.Content.InputClassSelectContent();
    }

    public void InputBaseEquipment()
    {
        NameSet.SetActive(false);
        contentControl.SetContent(contentControl.BaseEquipment);
        contentControl.Content.InputBaseEquipment();
    }

    public void InputNameSetting()
    {
        NameSet.SetActive(true);
    }
}
