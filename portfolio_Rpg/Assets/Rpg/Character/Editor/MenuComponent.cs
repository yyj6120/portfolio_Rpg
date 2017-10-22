using UnityEditor;
using Utils.Editor;
using Rpg.Item;

namespace Rpg.Character.Editor
{
    class MenuComponent
    {
        [MenuItem("Rpg/Resources/New AudioSurface")]
        static void NewAudioSurface()
        {
            ScriptableObjectUtility.CreateAsset<AudioSurface>();
        }
    }
}
