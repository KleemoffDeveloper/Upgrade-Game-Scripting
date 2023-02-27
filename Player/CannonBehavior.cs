using UnityEngine;

public class CannonBehavior : MonoBehaviour
{

    [System.Serializable]
    public struct Cannon
    {
        [Header("Profile")]
        public string name;
        public int id;
    }

    public Cannon CannonInfo;

    [Header("Transform")]
    public float m_XRotation;
    public float m_YRotation;
    public float m_XMax;
    public float m_YMax;

    private void Start()
    {
        // CannonInfo.id = Create an array of cannons in the GameManager or something 
    }

    private void Update()
    {
        UpdateTransform();
    }

    void UpdateTransform()
    {
        m_XRotation = Mathf.Clamp(m_XRotation, -m_XMax, m_XMax);
        m_YRotation = Mathf.Clamp(m_YRotation, -m_YMax, m_YMax);

        transform.rotation = Quaternion.Euler(m_YRotation, m_XRotation, 0);
    }
}
