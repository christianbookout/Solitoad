using UnityEngine;

public class Hand : MonoBehaviour
{
    // Reference to the grabbing hand sprite
    public Sprite grabbingHandSprite;

    // Reference to the open hand sprite
    public Sprite openHandSprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        Cursor.visible = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        transform.position = new Vector3(mousePosition.x, mousePosition.y - 2, 0);

        if (Input.GetMouseButton(0))
        {
            spriteRenderer.sprite = grabbingHandSprite;
        }
        else
        {
            spriteRenderer.sprite = openHandSprite;
        }
    }
}
