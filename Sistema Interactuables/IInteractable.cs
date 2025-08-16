using UnityEngine;
/*
 * Interfaz que define un elemento con el que se puede interactuar
 */
public interface IInteractable
{
    void Interact(GameObject interactor);	//Reacción del objeto al interactuar
    string GetPrompt();	//Por si queremos mostrar algún tipo de mensaje antes de interactuar
    int Priority { get; } // Cuanto mayor, más importante

}
