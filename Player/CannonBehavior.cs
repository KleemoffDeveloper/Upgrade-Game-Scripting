using System.Collections;
using UnityEngine;

public class CannonBehavior : MonoBehaviour
{

    [System.Serializable]
    public struct Cannon
    {
        [Header("Profile")]
        public string name;
        public int id;

        [Header("Attributes")]
        public float reloadTime;
    }

    public Cannon CannonInfo;

    [Header("Transform")]
    public Transform CannonBody;
    public Transform CannonBarrelPivot;
    public Transform CannonBarrelSpawn;
    public float m_XRotation;
    public float m_YRotation;
    [SerializeField] private float m_XMax;
    [SerializeField] private float m_YMax;

    [Header("Effects")]
    [SerializeField] GameObject poofEffect;

    private void Start()
    {
        var Cannons = GameObject.FindObjectsOfType<CannonBehavior>();

        for (int i = 0; i < Cannons.Length; i++)
        {
            if (Cannons[i] == gameObject)
            {
                CannonInfo.id = i;
            }
        }
    }

    public void UpdateTransform(float x, float y)
    {
        m_XRotation += x;
        m_YRotation -= y;

        m_XRotation = Mathf.Clamp(m_XRotation, -m_XMax, m_XMax);
        m_YRotation = Mathf.Clamp(m_YRotation, -m_YMax, m_YMax);

        CannonBody.rotation = Quaternion.Euler(0, m_XRotation, 0);
        CannonBarrelPivot.rotation = Quaternion.Euler(m_YRotation, transform.eulerAngles.y, 0);
    }

    bool canFire = true;
    public void ShootCannon(GameObject ballPrefab)
    {
        IEnumerator CannonShot()
        {
            canFire = false;

            // Code here...

            Instantiate(poofEffect, CannonBarrelSpawn.position, Quaternion.identity);

            GameObject ball = Instantiate(ballPrefab, CannonBarrelSpawn.position, Quaternion.identity);

            var ballBehavior = ball.GetComponent<BallBehavior>();

            var ballRigidbody = ball.GetComponent<Rigidbody>();

            ballRigidbody.AddForce(CannonBarrelSpawn.forward * ballBehavior.BallInfo.force);

            // Reload...

            float timer = CannonInfo.reloadTime;

            while(timer > 0)
            {
                timer -= Time.deltaTime;

                Debug.Log($"Reloading for {timer}s");

                yield return null;
            }

            canFire = true;

            yield return null;
        }

        if (canFire)
        {

            StartCoroutine(CannonShot());
        }
    }
}
