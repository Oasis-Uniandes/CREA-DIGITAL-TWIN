using Oculus.Platform.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class DroneBehavior : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [Tooltip("Lista de objetos hacia donde el dr�n volar�. Idealmente Empty Game Objects.")]
    [SerializeField] private GameObject[] waypoints; // Array de los waypoints que va a seguir el dron. Debe haber m�nimo uno que debe ser el origen.
                                                     // Un waypoint esta definido como un objeto cualquiera en el mundo; el dr�n se dirige primero a la posici�n del primer waypoint de la lista y luego al pr�ximo y as�
                                                     // sucesivamente hasta completar su recorrido. Idealmente, estos objetos no ser�n visibles por el usuario, ya que el dr�n chocar�a con un objeto f�sico. En vez, es mejor
                                                     // que los waypoints sean Empty Game Objects con un icono para que sea visible *solo* en la escena de edici�n, y que todos los waypoints se guarden bajo un mismo parent Game Object.

    [Header("Movement Settings")]
    [Tooltip("Repetir vuelo de dr�n al terminar el recorrido.")]
    public bool LoopTime;

    [Range(0.0f, 5.0f)] public float MoveSpeed; // Velocidad que el dron va a tener moviendose entre cada waypoint.
    [Range(0.0f, 5.0f)] public float RotateSpeed; // Velocidad de rotacion hacia cada waypoint que el dron tendr�.


    private int nextWaypointIndex; // El indice en el array del pr�ximo waypoint.
    private bool inMovement; // Bool que revisa si el dron esta en movimiento o no; se usa para decidir si ya lleg� a su objetivo.


    void Awake()
    {
        if (waypoints.Length == 0) // Si no hay waypoints agregados al dron en el inspector, este script se borra para evitar errores.
        {
            Debug.Log($"El dron {gameObject.name} no tiene ning�n waypoint de origen, se borr� su script.");
            Destroy(this);
        }
        else
        {
            transform.position = waypoints[0].transform.position; // La posici�n inicial del dr�n siempre ser� la del primer waypoint, incluso si el dr�n esta en otro lugar.
            UpdateGoal();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inMovement)
        {
            MoveDrone();
            RotateDrone();
        }
    }


    void MoveDrone()
    {
        Vector3 targetPosition = waypoints[nextWaypointIndex].transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
        if (transform.position == targetPosition)
        {
            UpdateGoal();
        }

        // TODO : Tal vez modificar el movimiento para que sea m�s suave, ahora mismo esta muy r�gido.

    }

    void RotateDrone()
    {
        // TODO : Rotaci�n de dr�n hacia el pr�ximo waypoint.
    }

    void UpdateGoal()
    {
        inMovement = true;
        if (nextWaypointIndex + 1 < waypoints.Length)
        {
            nextWaypointIndex++;
        } 
        else if (waypoints.Length > 1 && LoopTime) 
        {
            nextWaypointIndex = 0;
        }
        else
        {
            inMovement = false;
        }
    }
}
