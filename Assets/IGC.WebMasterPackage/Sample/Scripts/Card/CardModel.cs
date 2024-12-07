using UnityEngine;
using System.Linq;

namespace IGC.WebMasterPackage.Sample.CardGame
{
    public class CardModel
    {
        Card[] _cards;
        public void Init()
        {
            _cards = MasterDataApiClient.masterData.Card;
        }

        public CardStruct GetData(int id)
        {
            CardStruct cardStruct = new CardStruct();
            cardStruct.name = _cards.Where(c => c.contentId == id).First().name;
            cardStruct.sprite = MasterDataFacade.instance.GetSprite(_cards.Where(c => c.contentId == id).First().image);
            cardStruct.popularity = _cards.Where(c => c.contentId == id).First().popularity;
            cardStruct.monetary = _cards.Where(c => c.contentId == id).First().monetary;
            cardStruct.cuteness = _cards.Where(c => c.contentId == id).First().cuteness;
            cardStruct.text = _cards.Where(c => c.contentId == id).First().text;
            return cardStruct;
        }
    }
}