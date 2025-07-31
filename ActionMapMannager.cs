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