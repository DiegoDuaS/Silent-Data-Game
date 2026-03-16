using UnityEngine;
using System.Collections;

public class USB : Pickup
{
    [SerializeField] private float duracionMarcado = 15f;

    public override void Use()
    {

        ClassifiedDocs[] todosLosDocs = FindObjectsByType<ClassifiedDocs>(FindObjectsSortMode.None);

        ClassifiedDocs masCercano = null;
        float distanciaMinima = Mathf.Infinity;
        Vector3 posicionActual = transform.position;

        // Encontrar el mas cercano
        foreach (ClassifiedDocs doc in todosLosDocs)
        {
            float distancia = Vector3.Distance(posicionActual, doc.transform.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                masCercano = doc;
            }
        }

        if (masCercano != null)
        {
            StartCoroutine(MarcarDocumento(masCercano.gameObject));
        }
    }

    private IEnumerator MarcarDocumento(GameObject doc)
    {
        Transform indicador = doc.transform.Find("Indicator");

        if (indicador != null)
        {
            indicador.gameObject.SetActive(true);

            yield return new WaitForSeconds(duracionMarcado);

            if (indicador != null)
                indicador.gameObject.SetActive(false);
        }
    }
}