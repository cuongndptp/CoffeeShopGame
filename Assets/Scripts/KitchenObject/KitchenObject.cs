using UnityEngine;
using System;
using Assets.Scripts.KitchenObject;

public abstract class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private Rigidbody rb;

    public string objectID { get; private set; }  // Unique ID for saving

    public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Assign a unique ID (useful for identifying objects when saving)
        objectID = Guid.NewGuid().ToString();
    }

    public abstract void Interact(Player player);

    public virtual void HoldTo(Transform holdingPoint)
    {
        gameObject.layer = LayerMask.NameToLayer("Held");
        rb.isKinematic = true;
        transform.SetParent(holdingPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public virtual void Release(Player player)
    {
        rb.isKinematic = false;
        transform.parent = null;
        gameObject.layer = LayerMask.NameToLayer("KitchenObject");
        player.SetMode(Player.Mode.Normal);
        player.SetKitchenObject(null);
    }

    public virtual void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void Throw()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            Vector3 throwDirection = Camera.main.transform.forward;
            float throwForce = 10f;
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }
    }

    public void PlayerPickUp(Player player)
    {
        player.SetKitchenObject(this);
        HoldTo(player.GetInteractPoint());
    }

    // Save object data
    public ObjectData SaveData()
    {
        var objectData = new ObjectData
        {
            objectName = kitchenObjectSO.name,
            objectType = "KitchenObject",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            rotation = new float[] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z }
        };
        Debug.Log($"Saved KitchenObject: {objectData.objectName}, Position: {objectData.position[0]}, {objectData.position[1]}, {objectData.position[2]}");
        return objectData;
    }

    // Load object data
    public void LoadData(ObjectData data)
    {
        transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        transform.rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);

        Debug.Log($"Loaded KitchenObject: {data.objectName} at {transform.position}");
    }
}
