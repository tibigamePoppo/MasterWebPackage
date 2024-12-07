using UnityEngine;

namespace IGC.WebMasterPackage.Sample.CardGame
{
    public class CardPresenter : MonoBehaviour
    {
        CardModel cardModel;
        CardView cardView;
        public void Init(int initialId)
        {
            cardModel = new CardModel();
            cardView = GetComponent<CardView>();
            cardModel.Init();
            cardView.Init(cardModel.GetData(initialId));
        }

        public void UpdateCard(int id)
        {
            cardView.Init(cardModel.GetData(id));
        }
    }
}