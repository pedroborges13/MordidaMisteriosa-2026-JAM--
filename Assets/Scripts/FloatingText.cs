using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lifeTime;
    private TextMeshProUGUI textMesh;

    public void Setup(string text, Color color)
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.color = color;
        //Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }
}
