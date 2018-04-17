using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public void OnClickStartBtn()
    {
        //SceneManager.LoadScene("Level1");
        //SceneManager.LoadScene("Play", LoadSceneMode.Additive);
        SceneManager.LoadScene("SceneLoader");
    }
}
