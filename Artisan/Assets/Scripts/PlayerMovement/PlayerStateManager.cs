using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerStateManager : MonoBehaviour
{
    [HideInInspector]
    public PlayerBaseState currentState;

    [HideInInspector]
    public PlayerIdleState idleState = new PlayerIdleState();
    [HideInInspector]
    public PlayerWalkState walkState = new PlayerWalkState();
    [HideInInspector]
    public PlayerBusyState busyState = new PlayerBusyState();


    [HideInInspector] public Vector2 movement;
    [HideInInspector] public bool isTargeting = false;
    [HideInInspector] public bool isSneaking = false;
    [HideInInspector] public string currentAnimation = "";

    public float default_speed = 1;
    [SerializeField] public Animator characterAnimator;
    [SerializeField] private GameObject cameraZones;
    

    CharacterController controller;
    Inventory inv;
    GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
        inv = GetComponent<Inventory>();

        SwitchState(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
        //print(currentState.ToString());
    }

    // Camera Triggers //
    void OnTriggerEnter(Collider other)
    {
        CameraZone c;
        if (c = other.gameObject.GetComponent<CameraZone>())
        {
            cameraZones.transform.GetChild(0).gameObject.SetActive(false); //Disable the default camera
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            if (c.requiresInteriorTransition)
            {
                c.OpenModelInterior();        
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        CameraZone c;
        if (c = other.gameObject.GetComponent<CameraZone>())
        {
            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            cameraZones.transform.GetChild(0).gameObject.SetActive(true); //Enable the default camera
            if (c.requiresInteriorTransition)
            {
                c.CloseModelInterior();        
            }
        }
    }

    // Handle Input //

    void OnMove(InputValue moveVal)
    {
        movement = moveVal.Get<Vector2>();
    }

    void OnInteract()
    {
        if (target && currentState != busyState)
        {
            if (target.TryGetComponent<Interactable>(out Interactable i))
            {
                currentAnimation = i.Interact(inv.currentHotbarSelection);
                if (currentAnimation != "")
                {
                    SwitchState(busyState);
                }
                currentAnimation = "";
            }            
        }

    }

    // Helper Functions //
    public void SwitchState(PlayerBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    public void MovePlayer(float speed)
    {
        float moveX = movement.x;
        float moveZ = movement.y;

        Vector3 actual_movement = new Vector3(moveX, 0, moveZ);
        actual_movement.Normalize();

        characterAnimator.SetFloat("Speed", actual_movement.magnitude);

        Vector3 look = Vector3.RotateTowards(transform.forward, actual_movement, 0.5f, 0.5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), Time.deltaTime * 50);

        actual_movement.y = -1;

        controller.Move(actual_movement * Time.deltaTime * speed);
    }

    //SHOULD THIS BE SOMEWHERE ELSE? DIFFERENT SCRIPT??
    public void CheckTarget()
    {
        RaycastHit hit;
        //LayerMask mask = LayerMask.GetMask("Interactable");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100) && !EventSystem.current.IsPointerOverGameObject())
        {
            GameObject gameHit = hit.transform.gameObject;
            //print(gameHit.name);
            if (gameHit.GetComponent<Interactable>() && (gameHit.layer == 3 || gameHit.layer == 6))
            {
                if (Vector3.Distance(gameHit.transform.position, transform.position) < 5)
                    ChangeTarget(gameHit);
                else
                    ChangeTarget(null);
            }
            else
                ChangeTarget(null);
        }
        else
            ChangeTarget(null);
    }
    
    public void ChangeTarget(GameObject gameHit)
    {
        if (gameHit != target)
        {
            if (target != null)
            {
                //target.GetComponent<MeshRenderer>().material.color = Color.white;
                target.layer = 3;
                for (int i = 0; i < target.transform.childCount; i++)
                {
                    target.transform.GetChild(i).gameObject.layer = 3;         
                }
            }
            target = gameHit;
            if (target)
            {
                //MeshRenderer targetMesh = target.GetComponent<MeshRenderer>();
                //if (targetMesh != null)
                //{
                    //targetMesh.material.color = Color.red;
                target.layer = 6;
                for (int i = 0; i < target.transform.childCount; i++)
                {
                    target.transform.GetChild(i).gameObject.layer = 6;         
                }
                //}
            }

        }
    }
}
