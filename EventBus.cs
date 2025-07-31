using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Sistema para crear e invocar eventos. Hay que definir el evento y sus parámetros
* y crear una función para invocarlo. Los objetos que se quieran suscribir a este
* evento, simplemente añadirán la FuncionCallback que quierán que responda al evento
* en el OnEnable() y desuscribirla en el OnDisable(). Para invocar el evento, se debe
* llamar a EventBus.LanzarEventoX(parametros) para invocarlo.
*/
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
    public static event Action<Object> OnEvent1, OnEvent2, OnEvent3;
	
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
