using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateManager : MonoBehaviour
{
    [HideInInspector]
    public PlayerBaseState currentState;

    [HideInInspector]
    public PlayerIdleState idleState = new PlayerIdleState();
    [HideInInspector]
    public PlayerWalkState walkState = new PlayerWalkState();
    [HideInInspector]
    public PlayerTargetState targetState = new PlayerTargetState();

    [HideInInspector] public Vector2 movement;
    [HideInInspector] public bool isTargeting = false;
    [HideInInspector] public bool isSneaking = false;

    public FarmManager farm;
    public float default_speed = 1;

    CharacterController controller;
    Inventory inv;
    GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        inv = GetComponent<Inventory>();

        SwitchState(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    // Handle Input //

    void OnMove(InputValue moveVal)
    {
        movement = moveVal.Get<Vector2>();
    }

    void OnTarget(InputValue targetVal)
    {
        if (targetVal.isPressed) 
            isTargeting = true;
        else
            isTargeting = false;
    }

    void OnInteract()
    {
        if (target.TryGetComponent<Interactable>(out Interactable i))
        {
            i.Interact(inv.currentItem, farm);
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

        Vector3 look = Vector3.RotateTowards(transform.forward, actual_movement, 0.5f, 0.5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), Time.deltaTime * 50);

        controller.Move(actual_movement * Time.deltaTime * speed);
    }

    //SHOULD THIS BE SOMEWHERE ELSE? DIFFERENT SCRIPT??
    public void CheckTarget()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Interactable");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, mask))
        {
            GameObject gameHit = hit.transform.gameObject;
            if (Vector3.Distance(gameHit.transform.position, transform.position) < 5)
                ChangeTarget(gameHit);
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
                target.GetComponent<MeshRenderer>().material.color = Color.white;

            target = gameHit;
            if (target)
            {
                MeshRenderer targetMesh = target.GetComponent<MeshRenderer>();
                if (targetMesh != null)
                {
                    targetMesh.material.color = Color.red;
                }
            }

        }
    }
}
