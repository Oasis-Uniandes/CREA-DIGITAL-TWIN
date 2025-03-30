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

    [Tooltip("Curva personalizable para editar la velocidad del dron en su recorrido de un waypoint al proximo. Rango: (0, 10]")]
    public AnimationCurve SpeedCurve; // Esta es una curva que se puede cambiar en el editor como sea necesario. Su dominio es [0, 1], por lo que cualquier punto fuera de estos no es considerado en la velocidad.
                                      // Esta determina la velocidad del dron en un solo recorrido de un waypoint a otro, no en el recorrido completo de toda la lista de waypoints. Esto quiere decir que al principio
                                      // del trayecto de un punto a otro, la velocidad del dron sera el punto 0 de la curva, y al llegar al destino su velocidad sera el punto 1 de la curva. Por eso, la curva debe siempre
                                      // ser en todo momento mayor que cero, ya que si el dron se detiene en cualquier momento no sera capaz de continuar en la curva y se quedara quieto por siempre.

    [Tooltip("Velocidad de rotacion para mirar al proximo punto")]
    [Range(0.0f, 5.0f)] public float RotateSpeed;


    [Header("Other Settings")]
    [Tooltip("Tiempo de espera para comenzar vuelo hacia el proximo waypoint. Aqui sucede la rotacion en X.")]
    [Range(0.0f, 5.0f)] public float WaitTimeBetweenPoints; 


    private float currentSpeed; // Velocidad actual, proporcional al punto actual de la curva de velocidad.
    private float waypointProgress; // Progreso del recorrido del anterior waypoint hacia el proximo waypoint, este siendo un valor entre 0 y 1.
    private Vector3 lastPosition; // Posicion del dron al llegar al anterior waypoint.
    private int nextWaypointIndex; // El indice en el array del próximo waypoint.
    private bool inMovement; // Bool que revisa si el dron esta en movimiento o no; se usa para decidir si ya llegó a su objetivo.
    private bool inRotationY; // Bool que revisa si el dron esta rotando su eje X para apuntar al proximo waypoint.

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
            if (waypoints.Length > 1)
            {
               // transform.LookAt(waypoints[1].transform);
               // transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
                inMovement = true;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (inMovement)
        {
            MoveDrone();
        }

        //if (inRotationY)
            //RotateYaxis();
    }

    void MoveDrone()
    {
        Vector3 targetPosition = waypoints[nextWaypointIndex].transform.position; // Busca la posicion del proximo waypoint
        waypointProgress = 1 - (Vector3.Distance(transform.position, targetPosition) / Vector3.Distance(lastPosition, targetPosition)); // Usa distancias para calcular cuanto progreso se ha realizado de el recorrido del anterior waypoint al proximo,
                                                                                                                                        // siendo un valor float entre 0 y 1 (e.g. a la mitad del recorrido, esto sera 0.5).

        currentSpeed = SpeedCurve.Evaluate(waypointProgress); // Sabiendo cuanto progreso se ha hecho de este recorrido, se evalua la velocidad en ese punto en la curva de velocidad.


        if (currentSpeed <= 0) // Revisa que la velocidad sea siempre mayor que cero para evitar errores
        {
            Debug.Log($"La velocidad inicial del dron {gameObject.name} en la curva debe ser siempre mayor a cero, se borró su script.");
            Destroy(this);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime); // Se mueve hacia el target considerando la velocidad actual y arreglando el valor para que sea independiente del framerate.

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // Aproximo la distancia para evitar cualquier error de comparacion de floats muy extensos.
             StartCoroutine(WaitForGoalUpdate());   // Si se llego ya al target waypoint, el objetivo se actualiza para el proximo waypoint, o se detiene el recorrido si se acabo la lista y no hay loop time.
    }

    /*
    void RotateYaxis() // TODO: ARREGLAR ESTA FUNCION NO SIRVE AAAAAAAAAA
    {
        GameObject nextWaypoint = waypoints[nextWaypointIndex];
        Transform target = nextWaypoint.transform;

        TestWaypointBlocks(nextWaypoint, true); // Se supone que esto deberia mostrar fisicamente el proximo waypoint con un cubo

        Vector3 lookPos = target.position - transform.position; // Encuentra el vector que en teoria apuntaria al proximo waypoint
        lookPos.y = 0; // Solo nos interesa la direccion en X
        lookPos.z = 0;

        Quaternion rotation = Quaternion.LookRotation(lookPos); // Nos define la direccion del proximo waypoint como un Quaternion
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2); // Se translada a ese quaternion

        //Debug.Log(Vector3.Dot(lookPos.normalized, transform.forward));

        if (Vector3.Dot(lookPos.normalized, transform.forward) > 0.99) // Revisa si ya llego a la direccion esperada. Por alguna razon con esta configuracion de dron, entre mas se acerca a la rotacion, el producto punto se convierte 1 y no 0,                                                                          probablemente hice algo mal jajan't
        {
            TestWaypointBlocks(nextWaypoint, false); // apaga la visualizacion del waypoint actual
            inRotationY = false;
        }
    }
    */

    private IEnumerator WaitForGoalUpdate()
    {
        inMovement = false;
        bool newUpdate = UpdateGoal();
        inRotationY = true;
        yield return new WaitForSeconds(WaitTimeBetweenPoints);
        inMovement = newUpdate;
    }

    bool UpdateGoal()
    {
        lastPosition = transform.position;  // Guarda la posicion actual para poder calcular despues la distancia total de un waypoint al proximo

        if (nextWaypointIndex + 1 < waypoints.Length)
            nextWaypointIndex++;

        else if (waypoints.Length > 1 && LoopTime)
            nextWaypointIndex = 0;

        else
        {
            inMovement = false;
            return false;
        }

        return true;
        
    }

    void TestWaypointBlocks(GameObject waypoint, bool enable)
    {
        if (waypoint.GetComponentInChildren<MeshRenderer>() != null)
            waypoint.GetComponentInChildren<MeshRenderer>().enabled = enable;
    }
}
