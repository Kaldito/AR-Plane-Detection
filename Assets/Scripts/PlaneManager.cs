using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneManager : MonoBehaviour
{
    // Crea variable de tipo ARPlaneManager para poder acceder a los planos detectados
    // Es Serializable para poder verlo en el inspector de Unity
    [SerializeField] private ARPlaneManager planeManager;
    // Se crea una variable para guardar el prefab del plano
    [SerializeField] private GameObject planePrefab;

    // Se crea una lista de ARPlane para guardar los planos detectados
    private List<ARPlane> planes = new List<ARPlane>();
    // Se crea una variable para guardar el modelo 3D
    private GameObject model3D;

    // Esta función se ejecuta cuando el script se habilita
    private void OnEnable()
    {
        // Se suscribe a la función PLanesFound para que se ejecute cuando se detecten planos
        planeManager.planesChanged += PLanesFound;
    }

    // Esta función se ejecuta cuando el script se deshabilita
    private void OnDisable() {
        // Se desuscribe de la función PLanesFound
        planeManager.planesChanged -= PLanesFound;
    }

    // Esta función se ejecuta cuando se detectan planos
    private void PLanesFound(ARPlanesChangedEventArgs planeData)
    {   
        // Si planeData.added no es nulo y tiene elementos se agregan a la lista de planes
        if (planeData.added != null && planeData.added.Count > 0)
        {   
            // Se agregan los planos detectados a la lista de planes
            planes.AddRange(planeData.added);
        }

        // Se recorren los planos detectados
        foreach (var plane in planes)
        {   
            // Si el área del plano es mayor o igual a 0.4 y el modelo 3D no ha sido instanciado
            if (plane.extents.x * plane.extents.y >= 0.4 && model3D == null){
                // Se instancia el modelo 3D en la posición del plano
                model3D = Instantiate(planePrefab);
                // Se crea un offset para que el modelo 3D no esté dentro del plano detectado
                // Para lograr esto se toma la mitad de la escala en Y del modelo 3D
                float yOffset = model3D.transform.localScale.y / 2;
                // Se asigna la posición del modelo 3D en el centro del plano detectado con el offset en Y
                model3D.transform.position = new UnityEngine.Vector3(plane.center.x, plane.center.y + yOffset, plane.center.z);
                // Se asigna la rotación del modelo 3D para que mire en la dirección del plano detectado y no hacia arriba
                model3D.transform.forward = plane.normal;
                // Se detiene la detección de planos para no instanciar más modelos 3D
                StopPlaneDetection();
            }
        }
    }

    // Esta función para la detección de planos
    public void StopPlaneDetection()
    {
        planeManager.requestedDetectionMode = PlaneDetectionMode.None;
        foreach (var plane in planes)
        {
            plane.gameObject.SetActive(false);
        }
    }
}
