using Oculus.Platform.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class DroneBehavior : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [Tooltip("Lista de objetos hacia donde el drón volará. Idealmente Empty Game Objects.")]
    [SerializeField] private GameObject[] waypoints; // Array de los waypoints que va a seguir el dron. Debe haber mínimo uno que debe ser el origen.
                                                     // Un waypoint esta definido como un objeto cualquiera en el mundo; el drón se dirige primero a la posición del primer waypoint de la lista y luego al próximo y así
                                                     // sucesivamente hasta completar su recorrido. Idealmente, estos objetos no serán visibles por el usuario, ya que el drón chocaría con un objeto físico. En vez, es mejor
                                                     // que los waypoints sean Empty Game Objects con un icono para que sea visible *solo* en la escena de edición, y que todos los waypoints se guarden bajo un mismo parent Game Object.

    [Header("Movement Settings")]
    [Tooltip("Repetir vuelo de drón al terminar el recorrido.")]
    public bool LoopTime;

    [Range(0.0f, 5.0f)] public float MoveSpeed; // Velocidad que el dron va a tener moviendose entre cada waypoint.
    [Range(0.0f, 5.0f)] public float RotateSpeed; // Velocidad de rotacion hacia cada waypoint que el dron tendrá.


    private int nextWaypointIndex; // El indice en el array del próximo waypoint.
    private bool inMovement; // Bool que revisa si el dron esta en movimiento o no; se usa para decidir si ya llegó a su objetivo.


    void Awake()
    {
        if (waypoints.Length == 0) // Si no hay waypoints agregados al dron en el inspector, este script se borra para evitar errores.
        {
            Debug.Log($"El dron {gameObject.name} no tiene ningún waypoint de origen, se borró su script.");
            Destroy(this);
        }
        else
        {
            transform.position = waypoints[0].transform.position; // La posición inicial del drón siempre será la del primer waypoint, incluso si el drón esta en otro lugar.
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

        // TODO : Tal vez modificar el movimiento para que sea más suave, ahora mismo esta muy rígido.

    }

    void RotateDrone()
    {
        // TODO : Rotación de drón hacia el próximo waypoint.
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
