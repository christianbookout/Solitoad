using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBug : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Bug bugComponent;
    private GridSpace currSpace;

    private void Awake()
    {
        bugComponent = GetComponent<Bug>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (GridController.gamePhase != GridController.GamePhase.PickingBugs) return;

        if (currSpace) currSpace.RemoveBug();
        currSpace = null;
        bugComponent.moving = false;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GridController.gamePhase != GridController.GamePhase.PickingBugs) return;

        if (TryGetComponent<Collider2D>(out var collider))
        {
            Vector2 currentPosition = transform.position;
            Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(currentPosition, collider.bounds.size.x);

            GridSpace closestSpace = null;
            float closestDistance = float.MaxValue;

            foreach (var overlappingCollider in overlappingColliders)
            {
                if (overlappingCollider.TryGetComponent<GridSpace>(out var gridSpace))
                {
                    float distance = Vector2.Distance(currentPosition, overlappingCollider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestSpace = gridSpace;
                    }
                }
            }
            // If the bug landed on a space, then add it to the space (if it can). Otherwise the bug should wander around.
            if (closestSpace != null && closestSpace.PlayerCanAddBugs())
            {
                closestSpace.AddBug(bugComponent);
                currSpace = closestSpace;
            } else
            {
                bugComponent.moving = true;
            }
            
        }
    }
}
