using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private float tiempoEsperaEntreGuardados = 2f;
    private float proximoGuardadoPermitido = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= proximoGuardadoPermitido)
        {
            proximoGuardadoPermitido = Time.time + tiempoEsperaEntreGuardados;
            EventManager.TriggerCheckpointReached(); 
            StartCoroutine(FeedbackVisual());

            Debug.Log("<color=green>[SISTEMA]: Datos persistidos en el JSON nuevamente.</color>"); 
        }
    }

    private System.Collections.IEnumerator FeedbackVisual()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Color colorOriginal = rend.material.color;
            rend.material.color = Color.yellow; 

            yield return new WaitForSeconds(0.5f);

            rend.material.color = Color.green;
        }
    }
}