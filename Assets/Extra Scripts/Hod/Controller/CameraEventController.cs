using UnityEngine;
using System.Collections;

public class CameraEventController : MonoBehaviour
{
    public Transform cameraTransform;
    public float cameraSpeed = 5f;
    public Vector3 chaseEndPosition;
    public GameObject leftBoundary;
    public GameObject rightBoundary;

    private bool _isChaseActive = false;

    private void Start()
    {
        // Desativar bloqueios inicialmente
        leftBoundary.SetActive(false);
        rightBoundary.SetActive(false);
    }

    public void StartCameraChase()
    {
        _isChaseActive = true;
        // Ativar bloqueios
        leftBoundary.SetActive(true);
        rightBoundary.SetActive(true);
        // Pode adicionar outras configurações iniciais aqui
    }

    private void Update()
    {
        if (_isChaseActive)
        {
            // Mover a câmera para a direita até o ponto final da perseguição
            cameraTransform.position = Vector3.MoveTowards(cameraTransform.position, chaseEndPosition, cameraSpeed * Time.deltaTime);

            // Verificar se a câmera alcançou o ponto final
            if (cameraTransform.position == chaseEndPosition)
            {
                EndCameraChase();
            }
        }
    }

    private void EndCameraChase()
    {
        _isChaseActive = false;
        // Desativar bloqueios
        leftBoundary.SetActive(false);
        rightBoundary.SetActive(false);
        // Retomar controle normal da câmera, se aplicável
    }
}
