//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class KitchenObject_BACKUP : MonoBehaviour
//{
//    private LayerMask heldMask;

//    [SerializeField] private KitchenObjectSO kitchenObjectSO;
//    [SerializeField] private Transform[] holderPointArray;
//    private KitchenObject holder;
//    private Rigidbody rb;
//    private List<HolderSlot> holderSlotList;
//    private bool readyToArrange;

//    public void Awake()
//    {
//        heldMask = LayerMask.NameToLayer("Held");
//    }

//    public void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        holderSlotList = new List<HolderSlot>();
//        foreach (Transform holderPoint in holderPointArray)
//        {
//            holderSlotList.Add(new HolderSlot(holderPoint));
//        }
//    }

//    public void Interact(Player player)
//    {

//        if (player.HasKitchenObject())
//        {
//            HandlePlayerWithKitchenObject(player);
//        }
//        //Player have no kitchen object in hand
//        else
//        {
//            HandlePlayerWithoutKitchenObject(player);
//        }

//    }

//    public void HandlePlayerWithKitchenObject(Player player)
//    {
//        if (player.PlayerHasHolder())
//        {
//            // Player is holding an holder
//            TryAddIngredientToHolder(player);
//        }
//        else
//        {
//            //Player's holding an ingredients
//            if (kitchenObjectSO.type == KitchenObjectSO.Type.holder)
//            {
//                //If this object is an holder -> Add holding ingre to holder
//                AddIngredientToPlayerHolder(player);
//            }
//        }
//    }

//    private void TryAddIngredientToHolder(Player player)
//    {
//        if (kitchenObjectSO.type != KitchenObjectSO.Type.ingredient) return;

//        List<HolderSlot> playerHolderSlots = player.GetKitchenObject().GetHolderSlotList();

//        if (holder != null)
//        {
//            foreach (HolderSlot slot in holder.holderSlotList)
//            {
//                if (slot.kitchenObject == this)
//                {
//                    slot.kitchenObject = null;
//                }
//            }
//        }

//        foreach (HolderSlot slot in playerHolderSlots)
//        {
//            if (slot.kitchenObject == null)
//            {
//                holder = player.GetKitchenObject();
//                slot.kitchenObject = this;
//                HoldTo(slot.holderPoint);
//                return;
//            }
//        }

//        Debug.Log("No slot available for ingredient.");
//    }

//    private void AddIngredientToPlayerHolder(Player player)
//    {
//        Transform holderPoint = AddKitchenObjectToHolderSlot(player.GetKitchenObject());
//        if (holderPoint == null)
//        {
//            Debug.LogError("No available slot in holder for ingredient.");
//            return;
//        }

//        player.GetKitchenObject().holder = this;
//        player.GetKitchenObject().HoldTo(holderPoint);
//        player.SetKitchenObject(null);
//    }

//    public void HandlePlayerWithoutKitchenObject(Player player)
//    {
//        //Player picked up the object
//        player.SetKitchenObject(this);
//        HoldTo(player.GetInteractPoint());

//        //If the object is an holder
//        if (kitchenObjectSO.type == KitchenObjectSO.Type.holder)
//        {
//            player.SetMode(Player.Mode.Put);
//        }
//        //If the object is an ingredients
//        else if (kitchenObjectSO.type == KitchenObjectSO.Type.ingredient)
//        {
//            if (holder != null)
//            {
//                //If the picked up object is part of another holder's slot -> Clear the slot.
//                foreach (HolderSlot slot in holder.holderSlotList)
//                {
//                    if (slot.kitchenObject == this)
//                    {
//                        slot.kitchenObject = null;
//                    }
//                }
//            }
//        }
//    }
//    public void PlayerDrop(Player player)
//    {
//        Release();
//        player.SetMode(Player.Mode.Normal);
//        player.SetKitchenObject(null);
//    }

//    public void Release()
//    {
//        rb.isKinematic = false;
//        rb.velocity = Vector3.zero;
//        rb.angularVelocity = Vector3.zero;

//        gameObject.layer = LayerMask.NameToLayer("KitchenObject");
//        this.transform.parent = null; //unparent object
//        this.transform.localScale = Vector3.one; // Reset scale to its original value
//    }

//    private void HoldTo(Transform holdingPoint)
//    {

//        this.gameObject.layer = heldMask;
//        if (rb != null)
//        {
//            rb.isKinematic = true;
//            rb.transform.parent = holdingPoint;
//            rb.transform.localPosition = Vector3.zero;
//            rb.transform.localRotation = Quaternion.identity;
//            rb.transform.localScale = Vector3.one;
//        }
//    }

//    public Transform AddKitchenObjectToHolderSlot(KitchenObject kitchenObject)
//    {
//        foreach (HolderSlot slot in holderSlotList)
//        {
//            if (slot.kitchenObject == null)
//            {
//                slot.kitchenObject = kitchenObject;
//                return slot.holderPoint;
//            }
//        }
//        return null;
//    }

//    public KitchenObjectSO GetKitchenObjectSO()
//    {
//        return kitchenObjectSO;
//    }

//    public List<HolderSlot> GetHolderSlotList()
//    {
//        return holderSlotList;
//    }

//    public bool IsReadyToArrange()
//    {
//        return readyToArrange;
//    }

//    [System.Serializable]
//    public class HolderSlot
//    {
//        public Transform holderPoint;
//        public KitchenObject kitchenObject; // Assign when an object is placed

//        public HolderSlot(Transform holderPoint)
//        {
//            this.holderPoint = holderPoint;
//            this.kitchenObject = null; // Initialize as empty
//        }
//    }
//}
