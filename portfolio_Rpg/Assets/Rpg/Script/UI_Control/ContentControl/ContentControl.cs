using UnityEngine;

public interface IContent
{
    void InputClassSelectContent();
    void InputBaseEquipment();
    void InputNameSetting();
}
public class ContentControl : MonoBehaviour
{
    #region Variables
    public GameObject classSelectObject;
    public GameObject baseEquipmentContentObject;
    public GameObject nameSettingContentObject;

    IContent content;
    IContent classSelect;
    IContent baseEquipment;
    IContent nameSetting;

    public IContent ClassSelect
    {
        get
        {
            return classSelect;
        }

        set
        {
            classSelect = value;
        }
    }

    public IContent BaseEquipment
    {
        get
        {
            return baseEquipment;
        }

        set
        {
            baseEquipment = value;
        }
    }

    public IContent NameSetting
    {
        get
        {
            return nameSetting;
        }

        set
        {
            nameSetting = value;
        }
    }

    public IContent Content
    {
        get
        {
            return content;
        }

        set
        {
            content = value;
        }
    }


    #endregion
    #region Initialize Content
    private void Awake()
    {
        classSelect = new ClassSelectContent(this,classSelectObject);
        nameSetting = new NameSettingContent(this,nameSettingContentObject);
        BaseEquipment = new BaseEquipMentContent(this,baseEquipmentContentObject);
        //시작 콘텐츠
        Content = ClassSelect;
        Content.InputClassSelectContent();
    }
    #endregion
    #region Content State , Content Change

    public void InputClassSelectContent()
    {
        Content.InputClassSelectContent();
    }

    public void InputBaseEquipment()
    {
        Content.InputBaseEquipment();
    }

    public void InputNameSet()
    {
        Content.InputNameSetting();
    }

    public void SetContent(IContent content)
    {
        this.Content = content;
    }
    #endregion
}
