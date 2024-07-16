using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    private string apiUrl = "https://jsonplaceholder.typicode.com/todos/1";

    void Start()
    {
        StartCoroutine(GetData());
    }

    IEnumerator GetData()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            // Parse the response
            TodoItem todo = JsonUtility.FromJson<TodoItem>(request.downloadHandler.text);
            Debug.Log($"Title: {todo.title}, Completed: {todo.completed}");
        }
    }
}

[System.Serializable]
public class TodoItem
{
    public int userId;
    public int id;
    public string title;
    public bool completed;
}
