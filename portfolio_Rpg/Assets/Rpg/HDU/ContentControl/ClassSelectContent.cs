using UnityEngine;
public class ClassSelectContent : IContent
{
    ContentControl contentControl;
    GameObject classSelectContent;
    public ClassSelectContent(ContentControl contentControl , GameObject classSelectContent)
    {
        this.contentControl = contentControl;
        this.classSelectContent = classSelectContent;
        this.classSelectContent.SetActive(false);
    }

    public void InputClassSelectContent()
    {
        classSelectContent.SetActive(true);     
    }

    public void InputBaseEquipment()
    {
        classSelectContent.SetActive(false);
        contentControl.SetContent(contentControl.BaseEquipment);
        contentControl.Content.InputBaseEquipment();
    }

    public void InputNameSetting()
    {
        classSelectContent.SetActive(false);
        contentControl.SetContent(contentControl.NameSetting);
        contentControl.Content.InputNameSetting();
    }
}
