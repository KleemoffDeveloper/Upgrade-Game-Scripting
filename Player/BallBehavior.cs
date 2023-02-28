using System.Collections;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    [System.Serializable]
    public struct Ball
    {
        [Header("Profile")]
        public string name;
        public int id;

        [Header("Attributes")]
        public float damage;
        public float force;
        public float splashRadius;
    }

    public Ball BallInfo;

    [Header("Effects")]
    [SerializeField] GameObject[] explosionEffects;

    private void Start()
    {
        var Balls = GameObject.FindObjectsOfType<BallBehavior>();

        for (int i = 0; i < Balls.Length; i++)
        {
            if (Balls[i] == gameObject)
            {
                BallInfo.id = i;
            }
        }

        StartCoroutine(AutoCleanup());
    }

    IEnumerator AutoCleanup()
    {
        while (true)
        {
            if(transform.position.y < -100)
            {
                Destroy(gameObject);
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // All of the enemies inside of the splash radius will recieve knockback and damage
        foreach (var enemy in GameObject.FindObjectsOfType<EnemyBehavior>())
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= BallInfo.splashRadius)
            { 
                var behavior = enemy.GetComponent<EnemyBehavior>();

                var rigibody = enemy.GetComponent<Rigidbody>();

                behavior.EnemyInfo.health -= BallInfo.damage;

                rigibody.AddExplosionForce(BallInfo.force / 4, transform.position, distance * BallInfo.splashRadius, 1f);
            }
        }

        foreach (var effect in explosionEffects)
        {
            Instantiate(effect, transform.position + Vector3.up, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
