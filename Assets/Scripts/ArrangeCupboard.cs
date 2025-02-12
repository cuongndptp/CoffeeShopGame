using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ArrangeCupboard : MonoBehaviour
{
    [SerializeField] public Transform arrangePointTemplate;

    public void PlayerPutKitchenObjectAtHit(Player player, KitchenObject kitchenObject, RaycastHit hitInfo)
    {
        Transform arrangingPoint = Instantiate(arrangePointTemplate, hitInfo.transform);
        arrangingPoint.gameObject.SetActive(true);
        arrangingPoint.gameObject.SetActive(true);
        arrangingPoint.transform.position = hitInfo.point;
        arrangingPoint.transform.rotation = Quaternion.identity;
        kitchenObject.transform.SetParent(arrangingPoint.transform);
        kitchenObject.transform.localPosition = Vector3.zero; // Center it in the arranging point
        kitchenObject.transform.localRotation = Quaternion.identity; // Reset rotation
        player.SetMode(Player.Mode.Normal);
        player.SetKitchenObject(null);
    }


}
