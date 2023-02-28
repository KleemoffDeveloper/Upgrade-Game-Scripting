using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool roundActive = false;

    private void Start()
    {
        Update3DUI(true);
    }

    private void Update()
    {
        Update3DUI(false);
    }

    void Update3DUI(bool initializing)
    {
        foreach (var ui in GameObject.FindGameObjectsWithTag("3D UI"))
        {
            if (initializing)
            {
                ui.transform.localScale = new Vector3(-ui.transform.localScale.x, ui.transform.localScale.y, ui.transform.localScale.z);
            }

            ui.transform.LookAt(Camera.main.transform.position);
        }
    }
}
