using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IGC.WebMasterPackage.Sample.CardGame
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _popularity;
        [SerializeField] private TextMeshProUGUI _monetary;
        [SerializeField] private TextMeshProUGUI _cuteness;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;

        public void Init(CardStruct cardStruct)
        {
            _name.text = cardStruct.name;
            _image.sprite = cardStruct.sprite;
            _popularity.text = cardStruct.popularity.ToString();
            _monetary.text = cardStruct.monetary.ToString();
            _cuteness.text = cardStruct.cuteness.ToString();
            _text.text = cardStruct.text;
        }
    }
}