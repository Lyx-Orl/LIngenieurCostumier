using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // L'objet que la caméra doit suivre
    public float distance = 5.0f; // Distance initiale entre la caméra et la cible
    public float zoomSpeed = 2.0f; // Vitesse de zoom
    public float minDistance = .0001f; // Distance minimale
    public float maxDistance = 100.0f; // Distance maximale
    
    public float xSpeed = 120.0f; // Vitesse de rotation horizontale
    public float ySpeed = 120.0f; // Vitesse de rotation verticale
    
    public float yMinLimit = -20f; // Angle vertical minimum
    public float yMaxLimit = 80f; // Angle vertical maximum
    
    private float x = 0.0f; // Angle horizontal actuel
    private float y = 0.0f; // Angle vertical actuel

    void Start()
    {
        // Initialiser les angles de la caméra
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        
        // Si la cible n'est pas définie, essayer de trouver un objet avec le tag "Player"
        if (target == null)
        {
            GameObject temp = GameObject.FindGameObjectWithTag("Player");
            if (temp != null) target = temp.transform;
        }
    }

    void LateUpdate()
    {
        if (target)
        {
            // Rotation de la caméra avec le clic de souris
            if (Input.GetMouseButton(0)) // 0 = clic gauche, 1 = clic droit, 2 = clic milieu
            {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                
                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }
            
            // Zoom avec la molette
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minDistance, maxDistance);
            
            // Calculer la nouvelle position de la caméra
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
            
            // Appliquer la rotation et la position
            transform.rotation = rotation;
            transform.position = position;
        }
    }
    
    // Fonction pour limiter l'angle de la caméra
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}