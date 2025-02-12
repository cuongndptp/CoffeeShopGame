using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.KitchenObject
{
    [System.Serializable]
    public class ObjectData
    {
        public string objectName; // ✅ Store the asset name instead of a random ID
        public string objectType; // Used to identify what type of object this is (e.g., "KitchenObject", "Player", etc.)
        public float[] position;
        public float[] rotation;
    }
}
