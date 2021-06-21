using UnityEngine;

/// <summary>
/// Controla las madrigueras y la cantidad de conejos que pueden ocultarse en ellas. Hay un máximo de 3 conejos por
/// madriguera.
/// </summary>
public class BurrowScript : MonoBehaviour
{
    private int maxCapacity = 3;

    private int currentRabbits;

    private void Awake()
    {
        currentRabbits = 0;
    }

    /// <summary>
    /// Devuelve true si puede entrar otro conejo más.
    /// </summary>
    /// <returns></returns>
    public bool EnterBurrow()
    {
        //Para controlar que sea Thread-Safe
        lock ("current")
        {
            if (currentRabbits < maxCapacity)
            {
                currentRabbits++;
                return true;
            }

            return false;
        }
    }
    
    /// <summary>
    /// Disminuye en 1 la cantidad de conejos que hay dentro.
    /// </summary>
    /// <returns></returns>
    public void ExitBurrow()
    {
        //Para controlar que sea Thread-Safe
        lock("current")
            currentRabbits--;
    }

    /// <summary>
    /// Cuando entra un conejo, si hay sitio se oculta.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Rabbit") || other.CompareTag("FleeingRabbit")) && EnterBurrow())
        {
            other.transform.GetComponent<FlockAgentRabbit>().safe = true;
            other.transform.GetComponent<FlockAgentRabbit>().predator = null;
            other.transform.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// El conejo sale de la madriguera
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rabbit") || other.CompareTag("FleeingRabbit"))
        {
            other.transform.GetComponent<FlockAgentRabbit>().safe = false;
            other.transform.GetComponentInChildren<MeshRenderer>().enabled = true;
            ExitBurrow();
        }
    }
}
