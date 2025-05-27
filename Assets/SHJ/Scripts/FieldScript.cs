using UnityEngine;

public class FieldScript : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        int gridSize = (int)transform.localScale.x;

        float scaleX = (int)(transform.localScale.x / 2);
        float scaleY = (int)(transform.localScale.x / 2);
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.positionCount = (gridSize * 2) * 2 - 1;

        lineRenderer.SetPosition(0, new Vector3(-scaleX, scaleY, 0f));

        int index = 1;

        bool toggleInt = true;
        for (int i = 0; i < gridSize; ++i)
        {
            int num = toggleInt? 1 : -1;
            lineRenderer.SetPosition(index, new Vector3(num * scaleX, scaleY - i, 0));
            ++index;
            if (i + 1 != gridSize)
            {
                lineRenderer.SetPosition(index, new Vector3(num * scaleX, scaleY - 1 - i, 0));
                toggleInt = !toggleInt;
                ++index;
            }
        }

        bool toggleInt2 = true;
        for (int i = 0; i < gridSize; ++i)
        {
            int num = toggleInt2 ? 1 : -1;
            lineRenderer.SetPosition(index, new Vector3(scaleX - i,num * scaleY, 0));
            ++index;
            if (i + 1 != gridSize)
            {
                lineRenderer.SetPosition(index, new Vector3(scaleX - i - 1, num * scaleY, 0));
                ++index;
                toggleInt2 = !toggleInt2;
            }
        }

    }
}
