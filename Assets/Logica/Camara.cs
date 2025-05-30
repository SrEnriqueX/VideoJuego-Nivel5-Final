using UnityEngine;

public class Camara : MonoBehaviour
{
    public GameObject personaje;
    
    void Update()
    {
        Vector3 position = transform.position;
        position.x = personaje.transform.position.x;
        transform.position = position;
    }
}
