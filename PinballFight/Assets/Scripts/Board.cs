
using UnityEngine;
using UnityEngine.EventSystems;


public class Board : MonoBehaviour {

    public int player_id;
    
    // Params
    private float touch_dt;
    private float v_up;
    private float v_down;
    private float dy;
    private float dy2;
    private float v2 ;

    // Cache
    private bool is_active = false;  // activation status of bounce
    // private bool is_controllable = true;  (maybe not controllable when moving updown?)
    private int moving_status = 0;
    private GameController controller;
    private float touch_start;
    private float y0;
    private float width;
    private float bound = 2.8125f;

    private void Awake() {
        width = GetComponent<BoxCollider2D>().size.x;        

        var trigger = GetComponent<EventTrigger>();

        var et_down = new EventTrigger.Entry();
        et_down.eventID = EventTriggerType.PointerDown;
        et_down.callback.AddListener((d) => {on_pointdown((PointerEventData)d);});

        var et_up = new EventTrigger.Entry();
        et_up.eventID = EventTriggerType.PointerUp;
        et_up.callback.AddListener((d) => {on_pointup((PointerEventData)d);});

        var et_drag = new EventTrigger.Entry();
        et_drag.eventID = EventTriggerType.Drag;
        et_drag.callback.AddListener((d) => {on_pointdrag((PointerEventData)d);});

        trigger.triggers.Add(et_down);
        trigger.triggers.Add(et_up);
        trigger.triggers.Add(et_drag);

        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

    }

    private void Start() {
        y0 = transform.position.y;
    }

    public void set_param(LevelParam param){
        touch_dt = param.touch_dt;
        v_up = param.bounce_dy / param.bounce_dt;
        v_down = param.bounce_dy / param.bounce_dt2;
        dy = param.bounce_dy;
        dy2 = param.hit_dy;
        v2 = param.hit_dy / param.hit_dt;
    }

    private void on_pointdown(PointerEventData data){
        touch_start = Time.time;
    }

    private void on_pointup(PointerEventData data){
        if (Time.time - touch_start < touch_dt){
            if (moving_status == 0){
                controller.board_touched(player_id);
            }
        }
    }

    private void on_pointdrag(PointerEventData data){
        controller.board_dragged(Camera.main.ScreenToWorldPoint(data.position), player_id);
    }

    public void activate(){
        is_active = true;
        moving_status = 1;
    }

    public void move_horizontal(float x){
        float target = Mathf.Clamp(x, -bound + width/2, bound - width/2);
        transform.position = new Vector3(
            target, transform.position.y, transform.position.z
        );
    }

    public void hitten(){
        if (is_active){
            controller.board_deactivated(player_id);
            is_active = false;
        }
        moving_status = 3;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Ball"){
            int other_id = other.gameObject.GetComponent<Ball>().player_id;
            if (other_id != player_id){
                controller.board_attacked(player_id);
                hitten();
                Destroy(other.gameObject);
            }
            else if (other_id == player_id && is_active){
                other.gameObject.GetComponent<Ball>().ignite();
            }
        }
    }

    public void set_active_length(float length){
        GetComponent<RectHPBar>().set_hp(length);
    }

    private void Update() {

        if (moving_status == 0) return;
        
        float sgn = player_id == 0 ? 1.0f : -1.0f;

        if (moving_status == 1) {
            if (sgn * (transform.position.y - y0) - dy < 0){
                transform.position += new Vector3(0, sgn * v_up * Time.deltaTime, 0);
            }
            else{
                moving_status = 2;
                is_active = false;
            }
        }
        else if (moving_status == 2){
            if (sgn * (transform.position.y - y0) > 0){
                transform.position -= new Vector3(0, sgn * v_down * Time.deltaTime, 0);
            }
            else{
                moving_status = 0;
                transform.position = new Vector3(transform.position.x, y0, transform.position.z);
                controller.board_deactivated(player_id);
            }
        }
        else if (moving_status == 3){
            if (sgn * (transform.position.y - y0) + dy2 > 0){
                transform.position -= new Vector3(0, sgn * v2 * Time.deltaTime, 0);
            }
            else{
                moving_status = 4;
                is_active = false;
            }
        }
        else if (moving_status == 4){
            if (sgn * (transform.position.y - y0) < 0){
                transform.position += new Vector3(0, sgn * v2 * Time.deltaTime, 0);
            }
            else{
                moving_status = 0;
                transform.position = new Vector3(transform.position.x, y0, transform.position.z);
            }
        }
    }
}