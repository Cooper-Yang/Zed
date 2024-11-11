using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen; // Reference to the loading screen UI
    public Slider progressBar; // Reference to the progress bar UI element
    public TextMeshProUGUI progressText; // Reference to the progress text UI element

    // Method to start loading a new scene
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    // Coroutine to load the scene asynchronously
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Show the loading screen
        loadingScreen.SetActive(true);

        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // While the scene is loading, update the progress bar and text
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";

            yield return null;
        }

        // Hide the loading screen when done
        loadingScreen.SetActive(false);
    }
}