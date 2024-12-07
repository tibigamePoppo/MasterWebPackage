using System.Collections;
using UnityEngine;

namespace IGC.WebMasterPackage.Sample.CardGame
{
    public class PanelView : MonoBehaviour
    {
        [SerializeField] CardPresenter cardPresenter;
        private int _id = 1;

        void Start()
        {
            StartCoroutine(WaitLoadData());
        }

        IEnumerator WaitLoadData()
        {
            yield return new WaitUntil(() => MasterDataApiClient.loadIsEnd);
            cardPresenter.Init(_id);
        }

        public void NextCard()
        {
            if (MasterDataApiClient.masterData.Card.Length - 1 <= _id) return;
            _id = _id + 1;
            cardPresenter.UpdateCard(_id);
        }

        public void LastCard()
        {
            if (1 >= _id) return;
            _id = _id - 1;
            cardPresenter.UpdateCard(_id);
        }
    }
}