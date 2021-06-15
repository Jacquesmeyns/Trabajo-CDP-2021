using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Inspector propio para los objetos de tipo CompositeBehavior
[CustomEditor(typeof(CompositeBehavior))]
public class CompositeBehaviorEditor : Editor
{
    //Comportamiento a añadir
    private FlockBehavior adding;

    //Método que se encarga de la interfaz en el inspector
    public override void OnInspectorGUI()
    {
        // Preparando el inspector
        //Target es el objeto que observa el inspector
        CompositeBehavior actual = (CompositeBehavior) target;
        
        //Comienzo una nueva subdivisión horizontal en el inspector
        EditorGUILayout.BeginHorizontal();

        // Si no hay comportamientos, muestro un mensaje de warning
        if (actual.behaviors == null || actual.behaviors.Length == 0)
        {
            EditorGUILayout.HelpBox("No behaviors attached.", MessageType.Warning);
            EditorGUILayout.EndHorizontal();    //Cierro la subdivisión horizontal
        }
        //Si hay comportamientos
        else
        {
            //Dos etiquetas, Comportamientos y Pesos
            EditorGUILayout.LabelField("Behaviors");
            EditorGUILayout.LabelField("Weights");
            EditorGUILayout.EndHorizontal();    //Cierro la subdivisión actual (contiene o el warning o el encabezado)
            
            //Por cada comportamiento muestro un botón para borrarlo y un slider para ajustar el peso
            for (var i = 0; i < actual.behaviors.Length; i++)
            {
                //Empiezo una nueva subdivisión para cada comportamiento
                EditorGUILayout.BeginHorizontal();
                
                //Si el comportamiento actual es null o pulso su botón, lo borro del array de comportamientos
                if (GUILayout.Button("Remove") || actual.behaviors[i] == null)
                {
                    actual.behaviors = Remove(i, actual.behaviors);     //Borro el comportamiento
                    actual.weights = RemoveWeight(i, actual.weights);   //Borro su peso asociado
                    break;
                }

                //Actualizo la lista de comportamientos que se muestra en el editor
                
                //Muestro el comportamiento actual y admito objetos de la escena
                actual.behaviors[i] =
                    (FlockBehavior) EditorGUILayout.ObjectField(actual.behaviors[i], typeof(FlockBehavior), false);
                //Espaciado vertical entre los comportamientos del inspector
                EditorGUILayout.Space(30);
                //El slider para el peso
                actual.weights[i] = EditorGUILayout.Slider(actual.weights[i], 0, 1);
                //Cierro la subdivisión del comportamiento actual
                EditorGUILayout.EndHorizontal();
            }
        }

        
        EditorGUILayout.EndHorizontal();
        
        //La parte de añadir nuevos comportamientos
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Add behaviour..."); //Encabezado
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        //Dejo un espacio de objeto para añadir, y admito que sea de la escena
        adding = (FlockBehavior) EditorGUILayout.ObjectField(adding, typeof(FlockBehavior), false);
        
        //Si el comportamiento a añadir es distinto de null, lo añado al array de comportamientos
        if (adding != null)
        {
            //Copia de los comportamientos y pesos actuales
            FlockBehavior[] oldBehaviours = actual.behaviors;
            float[] oldWeights = actual.weights;
            //Aumento en 1 el tamaño del nuevo array para comportamientos y pesos
            if (oldBehaviours == null)
            {
                actual.behaviors = new FlockBehavior[1];
                actual.weights = new float[1];
            }
            else
            {
                actual.behaviors = new FlockBehavior[oldBehaviours.Length + 1];
                actual.weights = new float[oldWeights.Length + 1];
            }
            
            //Copio todas las variables a los arrays actuales
            for (var i = 0; i < oldBehaviours.Length; i++)
            {
                actual.behaviors[i] = oldBehaviours[i];
                actual.weights[i] = oldWeights[i];
            }

            //Y añado el nuevo comportamiento y peso
            actual.behaviors[oldBehaviours.Length] = adding;
            actual.weights[oldWeights.Length] = 1;
            //poniendo a null el comportamiento que acabo de añadir
            adding = null;
        }
    }
    
    // Para borrar un comportamiento
    private FlockBehavior[] Remove(int index, FlockBehavior[] old)
    {
        //Nuevo array con los comportamientos después de borrar el indicado (tiene 1 menos que el array original)
        FlockBehavior[] current = new FlockBehavior[old.Length - 1];

        //Añado al array todos los comportamientos del array original cuyo índice
        //  no coincidan con el que quiero borrar
        var newIndex = 0;
        for (var oldIndex = 0; oldIndex < old.Length; oldIndex++)
        {
            if (oldIndex != index)
            {
                current[newIndex] = old[oldIndex];
                newIndex++;
            }
        }

        return current;
    }
    
    // Para actualizar la lista de pesos al borrar un comportamiento
    private float[] RemoveWeight(int index, float[] old)
    {
        //Nuevo array con los pesos después de borrar el indicado (tiene 1 menos que el array original)
        float[] current = new float[old.Length - 1];

        //Añado al array todos los pesos del array original cuyo índice
        //  no coincidan con el que quiero borrar
        var newIndex = 0;
        for (var oldIndex = 0; oldIndex < old.Length; oldIndex++)
        {
            if (oldIndex != index)
            {
                current[newIndex] = old[oldIndex];
                newIndex++;
            }
        }

        return current;
    }
}
