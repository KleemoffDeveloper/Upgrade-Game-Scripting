using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [System.Serializable]
    public struct Enemy
    {
        [Header("Profile")]
        public string name;
        public int id;

        [Header("Attributes")]
        public float damage;
        public float moveSpeed;
        public float turnSpeed;
        [Range(0, 100)] public float health;
        public float viewDistance;
    }

    public Enemy EnemyInfo;

    [Header("Characteristics")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] faces;

    [Header("Pathing")]
    [SerializeField] Transform target;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
        
        spriteRenderer.sprite = faces[Random.Range(0, faces.Length)];

        var Enemies = GameObject.FindObjectsOfType<EnemyBehavior>();

        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i].gameObject == gameObject)
            {
                EnemyInfo.id = i;
            }
        }

        StartCoroutine(AutoCleanup());
    }

    private void FixedUpdate()
    {
        if (gameManager.roundActive)
        {
            // Makes the enemy rotate towards (look at) the active target when there are no obstacles in the path or when their "bottoms" aren't touching the ground
            if (!IsGroundDown() || !ObstacleInPath())
            {
                FixRotation(target.position);
            }
            // If there are obstacles in the path, the enemy will rotate towards the directions where there are no obstacles
            else if (ObstacleInPath())
            {
                FindDirection();
            }
        }
    }

    private void Update()
    {
        ObstacleDetection();
    }

    void FixRotation(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        Quaternion rotGoal = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.eulerAngles, rotGoal.eulerAngles) > 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, EnemyInfo.turnSpeed);
        }
    }

    void FindDirection()
    {

    }

    IEnumerator AutoCleanup()
    {
        while (true)
        {
            if (transform.position.y < -100)
            {
                Destroy(gameObject);
            }

            yield return null;
        }
    }

    // Limited to 5 observable directions
    [System.Serializable]
    public struct Obstacle
    {
        public GameObject m_gameObject;
        public int m_direction;
    }
    public Obstacle[] obstacles = new Obstacle[5];
    Ray[] rays = new Ray[5];
    int[] directions = { -90, -45, 0, 45, 90 };
    void ObstacleDetection()
    {
        // Projects rays around this object and if they hit something that isn't "Enemy" then add that obstacle the array

        RaycastHit[] hits = new RaycastHit[rays.Length];

        string[] avoidLayers = { "Enemy", "Ground" };

        for (int i = 0; i < directions.Length; i++)
        {
            Quaternion spreadAngle = Quaternion.AngleAxis(directions[i], transform.up);
            Vector3 newVector = spreadAngle * transform.forward;

            rays[i] = new Ray(transform.position, newVector);

            if (Physics.Raycast(rays[i], out hits[i], EnemyInfo.viewDistance, ~LayerMask.GetMask(avoidLayers)))
            {
                obstacles[i].m_gameObject = hits[i].transform.gameObject;
                obstacles[i].m_direction = directions[i];
                Debug.DrawRay(rays[i].origin, rays[i].direction * EnemyInfo.viewDistance, Color.red);
            }
            else
            {
                obstacles[i].m_gameObject = null;
                obstacles[i].m_direction = 0;
                Debug.DrawRay(rays[i].origin, rays[i].direction * EnemyInfo.viewDistance, Color.green);
            }
        }
    }

    // This indicates that the enemy's "feet" are on the ground (standing up-right)
    bool IsGroundDown()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        return Physics.Raycast(ray, out hit, GetComponent<Collider>().bounds.extents.y, ~LayerMask.GetMask("Ground"));
    }

    bool ObstacleInPath()
    {
        foreach(var obstacle in obstacles)
        {
            if(obstacle.m_gameObject != null)
            {
                Debug.Log("Obstacle in path...");
                return true;
            }
        }

        Debug.Log("No obstacles...");
        return false;
    }
}
