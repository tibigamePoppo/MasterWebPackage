using System.Linq;
using UnityEngine;

namespace IGC.WebMasterPackage
{
    public class MasterDataFacade : MonoBehaviour
    {
        public static MasterDataFacade instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        public Sprite GetSprite(string spriteName)
        {
            var sprite = MasterDataApiClient.sprites.Where(s => s.name == spriteName).FirstOrDefault();
            if (sprite == null || sprite == default)
            {
                Debug.LogError($"指定した名前のspriteは存在しません。 : {spriteName}");
                return null;
            }
            return sprite;
        }
    }
}