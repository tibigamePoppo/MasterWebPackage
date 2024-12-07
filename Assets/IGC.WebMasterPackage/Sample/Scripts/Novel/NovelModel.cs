using UnityEngine;
using System.Linq;

namespace IGC.WebMasterPackage.Sample.Novel
{
    public class NovelModel : MonoBehaviour
    {
        Story[] stories;
        public void Init()
        {
            stories = MasterDataApiClient.masterData.Story;
        }

        public NovelStruct GetData(int id)
        {
            NovelStruct cardStruct = new NovelStruct();
            cardStruct.talker = stories.Where(c => c.storyId == id).First().characterName;
            cardStruct.sprite = MasterDataFacade.instance.GetSprite(stories.Where(c => c.storyId == id).First().image);
            cardStruct.text = stories.Where(c => c.storyId == id).First().text;
            return cardStruct;
        }
    }
}