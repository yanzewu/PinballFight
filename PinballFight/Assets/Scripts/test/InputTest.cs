using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputTest : MonoBehaviour
{
    GameObject obj;
    GameObject output;
    Dictionary<int, GameObject> pairs = new Dictionary<int, GameObject>();
    public bool is_enabled = false;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("Sample");
        output = GameObject.Find("Text");
    }

    // Update is called once per frame
    void Update()
    {
        if (!is_enabled) return;

        foreach (Touch touch in Input.touches){

            if (touch.phase == TouchPhase.Began){
                
                var world_pos = Camera.main.ScreenToWorldPoint(touch.position);
                world_pos.z = 0;
                output.GetComponent<Text>().text = world_pos.ToString() + ":" + touch.fingerId.ToString();
                
                pairs[touch.fingerId] = Instantiate(obj, world_pos, Quaternion.identity);
                pairs[touch.fingerId].GetComponent<InputTest>().is_enabled = false;
            }
            else if (touch.phase == TouchPhase.Moved){
                var world_pos = Camera.main.ScreenToWorldPoint(touch.position);
                world_pos.z = 0;
                output.GetComponent<Text>().text = touch.fingerId.ToString();

                if (pairs.ContainsKey(touch.fingerId)){
                    pairs[touch.fingerId].transform.position = world_pos;
                }
            }
        }
        
    }
}
