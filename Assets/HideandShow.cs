using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideandShow : MonoBehaviour
{	
    private Renderer _renderer;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = gameObject.GetComponent<Renderer>();
    }
    public void onEnter(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void onExit(GameObject obj)
    {
        obj.SetActive(false);
	
    }
    public void change(GameObject obj)
    {
       obj.SetActive(true);
       gameObject.SetActive(false);

    }	
}
