using UnityEngine;

public class ForwardVisualizer : MonoBehaviour
{
    public float rayLength = 2.0f;
    public Color gizmoColor = Color.blue;
    private void OnDrawGizmos()
    {
        DrawForwardRay();
    }

    // Update is called once per frame
    private void DrawForwardRay()
    {
        Vector3 startPos = transform.position;
        Vector3 forwardDir = transform.forward * rayLength;
        Vector3 endPos = startPos + forwardDir;

        Gizmos.color = gizmoColor;
        Gizmos.DrawRay(startPos, forwardDir);
    }
}
