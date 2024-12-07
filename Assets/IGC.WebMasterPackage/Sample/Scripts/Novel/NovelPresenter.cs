using UnityEngine;

namespace IGC.WebMasterPackage.Sample.Novel
{
    public class NovelPresenter : MonoBehaviour
    {
        NovelModel novelModel;
        NovelView novelView;
        public void Init(int initialId)
        {
            novelModel = new NovelModel();
            novelView = GetComponent<NovelView>();
            novelModel.Init();
            novelView.Init(novelModel.GetData(initialId));
        }

        public void UpdateCard(int id)
        {
            novelView.Init(novelModel.GetData(id));
        }
    }
}