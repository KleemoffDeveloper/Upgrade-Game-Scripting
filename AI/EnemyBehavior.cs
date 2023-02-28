using System.Collections;
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
            if (Enemies[i] == gameObject)
            {
                EnemyInfo.id = i;
            }
        }

        StartCoroutine(AutoCleanup());

        FindPath(new Vector3(5, 0, 10));
    }

    private void FixedUpdate()
    {
        if (gameManager.roundActive)
        {
            // Makes the enemy rotate towards (look at) the active target when there are no obstacles in the path
            if (!IsGroundDown() || !ObstacleInPath())
            {
                FixRotation(target.position);
            }

            // Makes the enemy move towards the active target while dodging obstacles
            FindPath(target.position);
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
    GameObject[] obstacles = new GameObject[5];
    Ray[] rays = new Ray[5];
    void ObstacleDetection()
    {
        // Projects rays around this object and if they hit something that isn't "Enemy" then add that obstacle the array

        int[] directions = { -90, -45, 0, 45, 90 };

        RaycastHit[] hits = new RaycastHit[rays.Length];

        string[] avoidLayers = { "Enemy", "Ground" };

        for (int i = 0; i < directions.Length; i++)
        {
            Quaternion spreadAngle = Quaternion.AngleAxis(directions[i], transform.up);
            Vector3 newVector = spreadAngle * transform.forward;

            rays[i] = new Ray(transform.position, newVector);

            if (Physics.Raycast(rays[i], out hits[i], EnemyInfo.viewDistance, ~LayerMask.GetMask(avoidLayers)))
            {
                obstacles[i] = hits[i].transform.gameObject;
                Debug.DrawRay(rays[i].origin, rays[i].direction * EnemyInfo.viewDistance, Color.red);
            }
            else
            {
                obstacles[i] = null;
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

    void FindPath(Vector3 destination)
    {
        // Rays from each observable direction
        Ray neg90 = rays[0];
        Ray neg45 = rays[1];
        Ray zero = rays[2];
        Ray pos45 = rays[3];
        Ray pos90 = rays[4];

        // Avoids the directions in which the rays are touching an obstruction
    }

    bool ObstacleInPath()
    {
        return obstacles[0] != null;
    }
}
