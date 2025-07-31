/*
*	Clase que gestiona el cambio del mapa de acciones del InputSystem.
*	Se utiliza la clase EventBus para invocar el cambio mediante eventos,
*	por lo que se puede llamar desde cualquier parte del proyecto.
*	Requiere declarar en el EventBus el evento OnCambiarActionMap<string> y una 
*	función para invocar el evento (CambiarActionMap(string newMap))
*/
public class ActionMapMannager : MonoBehaviour
{
	public PlayerInput _playerInput;
	private string currentMap;
	[SerializeField] private string defaultMap, globalMap;
	
	public void OnEnable()
	{
		EventBus.OnCambiarActionMap += CambiarActionMap;
	}
	
	public void Disable()
	{
		EventBus.OnCambiarActionMap -= CambiarActionMap;
	}
	
	public void Start()
	{
		_playerInput.actions.FindActionMap(globalMap).Enable();	//Siempre debe estar activo
		CambiarActionMap(defaultMap);
	}
	
	public void CambiarActionMap(string newMap)
	{
		if(currentMap == newMap)
			return;
		
		if (String.IsNullOrEmpty(newMap))
			currentMap = defaultMap;
		
		if (!String.IsNullOrEmpty(currentMap))
			_playerInput.actions.FindActionMap(currentMap).Disable();
		
		_playerInput.actions.FindActionMap(newMap).Enable();
		currentMap = newMap;	
	}
}
