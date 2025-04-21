using UnityEngine;
using UnityEngine.InputSystem;

public class InputEnabler : MonoBehaviour
{
    public InputActionAsset inputAsset;
    void Start()
    {
        foreach (var map in inputAsset.actionMaps)
        {
            Debug.Log("Activando Action Map: " + map.name);
            map.Enable();
        }
    }
    void OnEnable()
    {
        if (inputAsset != null)
        {
            inputAsset.Enable();
            Debug.Log("Input Actions habilitadas");
        }
        else
        {
            Debug.LogWarning("No se asignó el InputActionAsset");
        }
    }
}
