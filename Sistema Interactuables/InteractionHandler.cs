public class InteractionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    private IInteractable currentInteractable;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & interactableLayer) == 0) return;
        if (!other.TryGetComponent<IInteractable>(out var interactable)) return;

        // Si no hay ninguno o el nuevo tiene prioridad igual o mayor
        if (currentInteractable == null || interactable.Priority >= currentInteractable.Priority)
        {
            currentInteractable = interactable;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<IInteractable>(out var interactable)) return;

        if (interactable == currentInteractable)
        {
            currentInteractable = null; // Si quieres, aquí podrías buscar otro en la zona
        }
    }

    void OnInteract(InputAction.CallbackContext context)
    {
		if (context.started)
			currentInteractable?.Interact(gameObject);
    }
}
