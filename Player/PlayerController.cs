using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] InputAction[] actions;

    [Header("Cannon")]
    [SerializeField] CannonBehavior activeCannon;
    [SerializeField] [Range(15, 30)] float rotationSpeed;
    [SerializeField] GameObject ballPrefab;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        foreach (var action in actions)
        {
            action.Enable();
        }
    }

    private void OnDisable()
    {
        foreach(var action in actions)
        {
            action.Disable();
        }
    }

    private void Update()
    {
        if(activeCannon != null)
        {
            HandleCannonRotation();

            if (actions[2].WasPressedThisFrame())
            {
                activeCannon.ShootCannon(ballPrefab);
            }
        }
    }

    void HandleCannonRotation()
    {
        float x = actions[0].ReadValue<float>() * rotationSpeed * Time.deltaTime;
        float y = actions[1].ReadValue<float>() * rotationSpeed * Time.deltaTime;

        activeCannon.UpdateTransform(x, y);
    }
}
