using System.Collections;
using UnityEngine;

namespace IGC.WebMasterPackage.Sample.Novel
{
    public class NovelPanelView : MonoBehaviour
    {
        [SerializeField] NovelPresenter _novelPresenter;
        private int _id = 1;

        void Start()
        {
            StartCoroutine(WaitLoadData());
        }

        IEnumerator WaitLoadData()
        {
            yield return new WaitUntil(() => MasterDataApiClient.loadIsEnd);
            _novelPresenter.Init(_id);
        }

        public void NextPage()
        {
            if (MasterDataApiClient.masterData.Story.Length - 1 <= _id) return;
            _id = _id + 1;
            _novelPresenter.UpdateCard(_id);
        }

        public void LastPage()
        {
            if (1 >= _id) return;
            _id = _id - 1;
            _novelPresenter.UpdateCard(_id);
        }
    }
}