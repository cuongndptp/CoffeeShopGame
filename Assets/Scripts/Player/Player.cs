using Assets.Scripts.KitchenObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Singleton
    public static Player Instance { get; private set; }
    public enum Mode
    {
        Normal,
        Put
    }

    //Freeze Look
    private bool freezedLook = false;

    private Mode mode;

    [SerializeField] public GameInput gameInput;
    [SerializeField] private Transform interactPoint;
    //Move
    [SerializeField] private float moveSpeed = 5f;

    //Look
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private Vector3 offset; // Adjust offset as needed

    //Jump
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundLayer;
    //Interact
    [SerializeField] private float interactRange = 5f;

    private KitchenObject kitchenObject;


    private Rigidbody rb;
    private Vector2 inputVector;
    private Vector2 lookInput;

    [SerializeField] private Transform playerCamera; // Assign the player's camera in the Inspector
    private float cameraPitch = 0f; // Tracks up/down camera rotation

    public event EventHandler<OnPlayerModeChangedEventArgs> OnPlayerModeChanged;
    public class OnPlayerModeChangedEventArgs : EventArgs
    {
        public Mode mode;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (gameInput == null)
        {
            Debug.LogWarning("gameInput was null. Attempting to find it in the scene.");
            gameInput = FindObjectOfType<GameInput>();
        }

        SubscribeToGameInput();
    }

    public void SubscribeToGameInput()
    {
        if (gameInput == null) return;

        gameInput.OnPlayerJump += GameInput_OnPlayerJump;
        gameInput.OnPlayerInteract += GameInput_OnPlayerInteract;
        gameInput.OnPlayerDrop += GameInput_OnPlayerDrop;
        gameInput.OnPlayerPrimary += GameInput_OnPlayerPrimary;
        gameInput.OnPlayerInteractAlternative += GameInput_OnPlayerInteractAlternative;
        gameInput.OnPlayerThrow += GameInput_OnPlayerThrow;
        gameInput.OnPlayerOpenRecipe += GameInput_OnPlayerOpenRecipe;
    }


    public void UnsubscribeFromGameInput()
    {
        if (gameInput == null) return;

        Debug.Log("Unsubscribing from game input events.");
        gameInput.OnPlayerJump -= GameInput_OnPlayerJump;
        gameInput.OnPlayerInteract -= GameInput_OnPlayerInteract;
        gameInput.OnPlayerDrop -= GameInput_OnPlayerDrop;
        gameInput.OnPlayerPrimary -= GameInput_OnPlayerPrimary;
        gameInput.OnPlayerInteractAlternative -= GameInput_OnPlayerInteractAlternative;
        gameInput.OnPlayerThrow -= GameInput_OnPlayerThrow;
    }

    private void GameInput_OnPlayerThrow(object sender, EventArgs e)
    {
        if (kitchenObject != null)
        {
            var thrownKitchenObject = kitchenObject;
            kitchenObject.Release(this);
            thrownKitchenObject.Throw();
        }
    }

    private void GameInput_OnPlayerInteractAlternative(object sender, EventArgs e)
    {
        if (kitchenObject == null)
        {
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, interactRange))
            {
                if (hitInfo.transform.TryGetComponent<Holder>(out Holder holder))
                {
                    holder.TryArrangeDish(out Transform dish);
                }

                if (hitInfo.transform.TryGetComponent<Ingredient>(out Ingredient ingredient))
                {
                    Holder holderOfIngredient = ingredient.GetHolder();
                    if (holderOfIngredient != null)
                    {
                        holderOfIngredient.TryArrangeDish(out Transform dish);
                    }
                }


                if (hitInfo.transform.TryGetComponent<Stock>(out Stock stock))
                {
                    stock.InteractAlternative(this);
                }

                if(hitInfo.transform.TryGetComponent<Mixer>(out Mixer mixer))
                {
                    mixer.InteractAlternative(this);
                }
                if (hitInfo.transform.TryGetComponent<Stove>(out Stove stove))
                {
                    stove.InteractAlternative();
                }
            }
        }
    }

    private void Start()
    {
        mode = Mode.Normal;
    }



    private void GameInput_OnPlayerPrimary(object sender, System.EventArgs e)
    {
        if (mode == Mode.Normal)
        {

        }
        else if (mode == Mode.Put)
        {

            Holder holder = kitchenObject as Holder;
            if (holder != null)
            {
                //Cast a ray from player camera to any collider with layer ArrangeZone
                //Instantiate an empty gameObject at the collided point, named it arrangingPoint
                //Set the held kitchen object to be child of arrangingPoint and hold the kitchen object there.
                if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, interactRange, LayerManager.GetLayerMask(LayerManager.Layer.ArrangeZone)))
                {
                    //Get arrange cupboard to get the template
                    ArrangeCupboard arrangeCupboard = hitInfo.transform.GetComponent<ArrangeCupboard>();
                    //Spawn the arrangePoint as child of the hitInfo
                    holder.SetReadyToArrange(true);
                    arrangeCupboard.PlayerPutKitchenObjectAtHit(this, kitchenObject, hitInfo);


                }
                else
                {
                    Debug.Log("No arrange zone found");
                }
            }



            Dish dish = kitchenObject as Dish;
            if (dish != null)
            {

                if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, interactRange, 1 << LayerMask.NameToLayer("DeliveryZone")))
                {

                    //Get the DeliveryZone
                    DeliveryZone deliveryZone = hitInfo.transform.GetComponent<DeliveryZone>();
                    //Let deliveryZone check the delivery and signal the order to proceed
                    if (deliveryZone != null)
                    {
                        deliveryZone.TryAddOrder(dish);
                        dish.Release(this);
                        dish.DestroySelf();
                    }
                }
            }


        }
    }

    private void GameInput_OnPlayerDrop(object sender, System.EventArgs e)
    {
        if (kitchenObject != null)
        { kitchenObject.Release(this); }
    }

    private void GameInput_OnPlayerInteract(object sender, System.EventArgs e)
    {
        // Cast a ray from the camera's position in the direction the player is looking
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, interactRange))
        {
            if (hitInfo.transform == null) return; // Object was destroyed before we could use it

            if (hitInfo.transform.TryGetComponent(out KitchenObject kitchenObject))
            {
                kitchenObject.Interact(this);
                if (hitInfo.transform == null) return; // Object may have been destroyed
            }
            if (hitInfo.transform.TryGetComponent(out Appliance appliance))
            {
                appliance.Interact(this);
                if (hitInfo.transform == null) return;
            }
            if (hitInfo.transform.TryGetComponent(out CustomerAI customerAI))
            {
                customerAI.Interact(this);
                if (hitInfo.transform == null) return;
            }
            if (hitInfo.transform.TryGetComponent(out PlateShelves dishShelves))
            {
                dishShelves.Interact(this);
                if (hitInfo.transform == null) return;
            }
            if (hitInfo.transform.TryGetComponent(out ShopKeeper shopSeller))
            {
                shopSeller.Interact();
                if (hitInfo.transform == null) return;
            }
            if (hitInfo.transform.TryGetComponent(out Stove stove))
            {
                stove.Interact(this);
            }
            if (hitInfo.transform.TryGetComponent(out SleepableObject sleepableObject))
            {
                sleepableObject.Interact();
            }
        }
    }

    private void Update()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactRange, Color.green);
        // Get input values
        lookInput = gameInput.GetLookVector();
        if (!freezedLook)
        {
            HandleLook();
        }
        //Debug.Log("Mode: " + mode);

    }

    private void LateUpdate()
    {
        Vector3 viewportPosition = Vector3.zero;
        Camera camera = playerCamera.GetComponent<Camera>();
        interactPoint.position = camera.ViewportToWorldPoint(viewportPosition + offset);
    }

    private void FixedUpdate()
    {
        inputVector = gameInput.GetMovementVectorNormalized();
        HandleMovement();
    }



    private void GameInput_OnPlayerOpenRecipe(object sender, EventArgs e)
    {
        RecipeUI.Instance.OpenCloseShop();
    }
    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        moveDirection = transform.TransformDirection(moveDirection); // Make movement relative to the player

        rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);
    }

    private void HandleLook()
    {
        // Horizontal rotation (yaw)
        transform.Rotate(Vector3.up, lookInput.x * sensitivity);

        // Vertical rotation (pitch)
        cameraPitch -= lookInput.y * sensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -120f, 120f); // Restrict vertical rotation
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    public void GameInput_OnPlayerJump(object sender, System.EventArgs e)
    {
        if (rb == null)
        {
            Debug.LogWarning("rb is null!");
            return;
        }

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    }

    public bool TryReleaseObject()
    {
        if(kitchenObject != null)
        {
            kitchenObject.Release(this);
            return true;
        }
        else
        {
            return false;
        }
    }

    public Transform GetInteractPoint()
    {
        return interactPoint;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public bool PlayerHasHolder()
    {
        return kitchenObject.GetKitchenObjectSO().type == KitchenObjectSO.Type.Holder;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void SetMode(Mode mode)
    {
        if (this.mode != mode)
        {

            this.mode = mode;
            OnPlayerModeChanged?.Invoke(this, new OnPlayerModeChangedEventArgs { mode = this.mode });
        }

    }
    public Mode GetMode()
    {
        return mode;
    }

    public void SetFreezedLook(bool freezedLook)
    {
        this.freezedLook = freezedLook;
    }

    public bool IsFreezedLook()
    {
        return freezedLook;
    }

    public ObjectData SaveData()
    {
        return new ObjectData
        {
            //objectID = "Player", // Unique identifier
            objectType = "Player",
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            rotation = new float[] { transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z }
        };
    }

    public void LoadData(ObjectData data)
    {
        gameInput = GameInput.Instance;
        Player player = GetComponent<Player>();
        player.enabled = true;
        transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        transform.rotation = Quaternion.Euler(data.rotation[0], data.rotation[1], data.rotation[2]);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

}
