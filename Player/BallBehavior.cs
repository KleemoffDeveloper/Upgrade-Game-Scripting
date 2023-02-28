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
        // If you collided with an enemy, decrease their health by BallInfo.damage;
        if(other.GetComponent<EnemyBehavior>() != null)
        {
            
        }

        foreach(var effect in explosionEffects)
        {
            Instantiate(effect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
