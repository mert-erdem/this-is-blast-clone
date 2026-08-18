using System.Collections;
using Game.Scripts.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Common
{
    //Loading scene's script
    public class Router : MonoBehaviour
    {

        private void Start()
        {
            StartCoroutine(LoadLastLevel());
        }

        private IEnumerator LoadLastLevel()
        {
            yield return new WaitForSeconds(1f);

            SceneManager.LoadScene(nameof(GameScenes.Menu));
        }
    }
}