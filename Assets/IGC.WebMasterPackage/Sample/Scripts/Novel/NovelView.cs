using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IGC.WebMasterPackage.Sample.Novel
{
    public class NovelView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _talker;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;

        public void Init(NovelStruct novelStruct)
        {
            _talker.text = novelStruct.talker;
            _image.sprite = novelStruct.sprite;
            _text.text = novelStruct.text;
        }
    }
}