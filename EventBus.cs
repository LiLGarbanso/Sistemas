using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus 
{
    /*Para suscribirse y dessuscribirse de los eventos:
     * 
     * private void OnEnable()
        {
            EventBus.NombreEvento += FuncionCallback;
        }

        private void OnDisable()
        {
            EventBus.Evento -= FuncionCallback;
        }
    
        En el cuerpo del objeto a suscribir
     */

	//--------Declaración de eventos--------//
	
	//Con parámetros
    public static event Action<GameObject> OnEvent1, OnEvent2, OnEvent3;
	
	//Sin parámetros
    public static event Action OnEvent4, OnEvent5;

	//----------Funciones para invocar los eventos----------//
    //Se llama a estas funciones, no a los eventos directamente,
	//pero las subscripciones si que se hacen al propio evento 
	
	//Llamadas a eventos con parámetros (puede ser cualquier otra cosa que GameObject)
    public static void LanzarEvento1(GameObject data) => OnEvent1?.Invoke(data);
    public static void LanzarEvento3(GameObject data) => OnEvent2?.Invoke(data);
    public static void LanzarEvento3(GameObject data) => OnEvent3?.Invoke(data);

	//Llamadas a eventos sin parámetros
    public static void LanzarEvento4() => OnEvent4?.Invoke();
    public static void LanzarEvento5() => OnEvent5?.Invoke();
}