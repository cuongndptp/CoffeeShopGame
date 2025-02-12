using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public enum Layer { Held, ArrangeZone, Player, Ground, KitchenObject, Environment, DeliveryZone }

    //private const string HELD = "Held";
    //private const string ARRANGE_ZONE = "ArrangeZone";
    //private const string PLAYER = "Player";
    //private const string GROUND = "Ground";
    //private const string KITCHEN_OBJECT = "KitchenObject";
    
    public static LayerMask GetLayerMask(Layer layer)
    {
        return LayerMask.NameToLayer(layer.ToString());
    }
}
