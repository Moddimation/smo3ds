using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_load : MonoBehaviour
{
    public void LoadResume(string sceneName)
    {
		scr_loadScene._f.nextScene = "scn_capMain0";//button?
		scr_gameInit.globalValues.focusOff();
		scr_gameInit.globalValues.dbg_enemyCount=0;
		SceneManager.LoadScene ("scn_loadRes", LoadSceneMode.Additive);
		gameObject.SetActive(false);
    }
}