
using System.Collections.Generic;
using UnityEngine;

public class BotManager {

    int player_id;
    Bot bot;

    float ball_r;
    float wall_x;
    float board_y;  // the frontier of board.
    float board_thick;
    float board_halfsize;
    float buffer;
    float self_buffer;
    float bounce_dy;

    class BallUnit {
        public GameObject ball;
        public Rigidbody2D rb;
        public float hit_pos;
        public float hit_time;
        public float leave_time;

        public BallUnit(GameObject ball) {
            this.ball = ball;
            this.rb = ball.GetComponent<Rigidbody2D>();
        }
    }

    float last_refresh_time = 0.0f;
    float refresh_freq = 0.5f;

    List<BallUnit> queue = new List<BallUnit>();
    List<BallUnit> self_queue = new List<BallUnit>();

    public void initialize(GameObject board, GameObject ball, int player_id){
        bot = board.AddComponent<Bot>();
        var cc = board.GetComponent<CapsuleCollider2D>();
        board_thick = cc.size.y;
        board_halfsize = cc.size.x / 2;
        board_y = board.transform.position.y - board_thick/2;
        ball_r = ball.GetComponent<CircleCollider2D>().radius;

        wall_x = Mathf.Abs(GameObject.Find("wallright").transform.position.x);
        this.player_id = player_id;
    }

    public void reload(LevelParam param){
        bot.set_speed(5);
        buffer = 0.2f;
        self_buffer = 0.2f;
        bounce_dy = param.bounce_dy * 0.8f;
    }

    public void add_new_ball(GameObject ball){

        var bu = new BallUnit(ball);
        update_hit_pos(bu);
        Debug.Log("New ball discovered: " + bu.rb.position.ToString());
        Debug.Log(System.String.Format("hit_pos={0}, hit_time={1}, leave_time={2}", bu.hit_pos, bu.hit_time, bu.leave_time));

        if (bu.hit_time < 0){
            return;
        }
        
        if (ball.GetComponent<Ball>().player_id == player_id){
            self_queue.Add(bu);
            self_queue.Sort((x,y)=>x.hit_time.CompareTo(y.hit_time));
        }
        else{
            queue.Add(bu);
            queue.Sort((x,y)=>x.hit_time.CompareTo(y.hit_time));    // A heap would be better
        }

        last_refresh_time = Time.time;
    }

    public void response(){
        // update queue; remove already passed; choose the closest one
        // avoid hit position

        if (Time.time - last_refresh_time > refresh_freq){
            foreach (var bu in queue){
                update_hit_pos(bu);
            }
            foreach (var bu in self_queue){
                update_hit_pos(bu);
            }
            queue.Sort((a,b)=>a.hit_time.CompareTo(b.hit_time));
            self_queue.Sort((a,b)=>a.hit_time.CompareTo(b.hit_time));
            last_refresh_time = Time.time;
        }

        for(int i = 0; i < queue.Count;i++){
            if (queue[i].leave_time < 0 || queue[i].ball.gameObject == null){
                queue.RemoveAt(i);
            }
        }
        for(int i = 0; i < self_queue.Count;i++){
            if (self_queue[i].leave_time < 0 || self_queue[i].ball.gameObject == null || self_queue[i].rb == null){
                self_queue.RemoveAt(i);
            }
        }

        bool do_move = false;
        if (queue.Count != 0){

            var ret = get_target_pos(queue[0]);
            if (queue.Count > 1){
                ret = update_target_pos(ret, get_target_pos(queue[1]));
            }


            if (ret.Item1 == 1 || ret.Item1 == 3){
                bot.set_target(ret.Item2[0]);
            }
            else if (ret.Item1 == 2 || ret.Item1 == 4){
                bot.set_target(ret.Item2[1]);
            }
            do_move = ret.Item1 != 0;

        }

        if (self_queue.Count > 0){

            float hit_left = self_queue[0].hit_pos - board_halfsize + self_buffer;
            float hit_right = self_queue[0].hit_pos + board_halfsize - self_buffer;
            float x = bot.get_pos();

            if ( !do_move || queue[queue.Count < 3 ? queue.Count - 1 : 2].hit_time > self_queue[0].hit_time){
                bot.set_target(
                    Mathf.Abs(x - hit_left) < Mathf.Abs(x - hit_right) ? hit_left : hit_right
                );
            }

            if (self_queue[0].rb != null && x > hit_left && x < hit_right){
                var dy = (board_y - self_queue[0].rb.position.y) * (player_id == 1 ? 1.0f : -1.0f);
                if (dy > 0 && dy < bounce_dy){
                    bot.bounce();
                }
            }
        }

    }

    private (int, float[]) get_target_pos(BallUnit bu){
        // 0-> no need to move; 1->left; 2->right; 3->left+right, prefer left; 4->left+right, prefer right

        float hit_left = bu.hit_pos - board_halfsize - ball_r - buffer;
        float hit_right = bu.hit_pos + board_halfsize + ball_r + buffer;
        float x = bot.get_pos();

        if (x < hit_left || x > hit_right){
            return (0, new float[2]{wall_x, -wall_x});
        }

        bool left_possible = hit_left > -wall_x + board_halfsize && (x - hit_left) / bot.speed < bu.hit_time;
        bool right_possible = hit_right < wall_x - board_halfsize && (hit_right - x) / bot.speed < bu.hit_time;

        if (!(left_possible ^ right_possible)){
            if (hit_left + wall_x > wall_x - hit_right){
                return (3, new float[2]{hit_left, hit_right});   // Always far from wall. NOTICE: here target_x is guaranteed to be reachable; but 
                // new algorithm may not.
            }
            else{
                return (4, new float[2]{hit_left, hit_right});
            }
        }
        else if (left_possible){
            return (1, new float[2]{hit_left, 0});
        }
        else {
            return (2, new float[2]{0, hit_right});
        }
    }

    private (int, float[]) update_target_pos((int, float[]) preferred, (int, float[]) optional){
        if (optional.Item1 == 0){
            return preferred;
        }
        else if (optional.Item1 == 1 || optional.Item1 == 3){
            if (preferred.Item1 == 0 || preferred.Item1 == 1 || preferred.Item1 == 3){
                return (preferred.Item1 == 0 ? optional.Item1 : preferred.Item1, new float[2]{
                    Mathf.Min(preferred.Item2[0], optional.Item2[0]),
                    preferred.Item1 != 1 && optional.Item1 != 1 ? Mathf.Max(preferred.Item2[1], optional.Item2[1]) : 0
                }
                );

                // only both are optional we are possible to move to right
            }
            else if (preferred.Item1 == 4){
                return optional;
            }
            else{
                return preferred;
            }
        }
        else if (optional.Item1 == 2 || optional.Item1 == 4){
            if (preferred.Item1 == 0 || preferred.Item1 == 2 || preferred.Item1 == 4){
                return (preferred.Item1 == 0 ? optional.Item1 : preferred.Item1, new float[2]{
                    preferred.Item1 != 2 && optional.Item1 != 2 ? Mathf.Min(preferred.Item2[0], optional.Item2[0]) : 0,
                    Mathf.Max(preferred.Item2[1], optional.Item2[1])
                }
                );

                // only both are optional we are possible to move to right
            }
            else if (preferred.Item1 == 3){
                return optional;
            }
            else{
                return preferred;
            }
        }
        else{
            return (0, new float[2]{0,0});  // won't actually happen.
        }
    }

    private void update_hit_pos(BallUnit bu){
        if (bu.ball.gameObject == null){
            bu.leave_time = -1;
            return;
        }

        var v = bu.rb.velocity;
        var pos = bu.rb.position;

        bu.hit_time = (board_y - pos.y) / v.y;
        bu.leave_time = (board_y + board_thick - pos.y) / v.y;

        var wall_width = wall_x * 2 - ball_r * 2;
        var raw_x = wall_width / 2 + pos.x + bu.hit_time * v.x;
        var n = Mathf.Floor(raw_x / wall_width);
        if (n % 2 == 0){
            bu.hit_pos = raw_x - n * wall_width - wall_width/2;
        }
        else{
            bu.hit_pos = wall_width/2 - (raw_x - n * wall_width);
        }
    }

}
