using UnityEngine;
/*
 * Interfaz que define un elemento con el que se puede interactuar
 * Cuando se define esta interfaz, el objeto debe heredar también la clase Monobehaviour,
 * si no, no podrá ser arrastrada a la escena. EJ: "public class ShipPart : Monobehaviour, IInteractable"
 */
public interface IInteractable
{
    void Interact(GameObject interactor);	//Reacción del objeto al interactuar
    string GetPrompt();	//Por si queremos mostrar algún tipo de mensaje antes de interactuar
    int Priority { get; } // Cuanto mayor, más importante

}

