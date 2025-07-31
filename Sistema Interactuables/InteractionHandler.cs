/*
 *	Clase que implementa el sistema para interactuar con objetos interactuables.
 *	El objeto que contenga el script debe tener un collider como trigger para detectar
 *	los objetos interactuables. Los objetos interactuables deben implementar la interfaz IInteractable
 *	y deben estar en la layerMask definida para poder detectarse. El comportamiento de cada interactuable
 *	se define en la propia implementación de la interfaz. Idealmente este script irá en el GameObject del
 *	personaje jugable si lo hay.
 */

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
