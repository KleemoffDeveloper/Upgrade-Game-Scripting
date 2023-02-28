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
        public float speed;
        [Range(0, 100)] public float health;
    }

    public Enemy EnemyInfo;

    [Header("Characteristics")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] faces;

    private void Start()
    {
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

    private void Update()
    {
        // FindPath(Vector3.zero);
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

    void FindPath(Vector3 destination)
    {
        Debug.Log($"Finding path to {destination}");
    }
}
