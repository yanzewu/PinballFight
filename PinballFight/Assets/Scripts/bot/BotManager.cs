
using System.Collections.Generic;
using UnityEngine;

public class BotManager {

    public int player_id;
    Bot bot;

    float ball_r;
    float wall_x;
    float board_y;  // the frontier of board.
    float board_thick;
    float board_halfsize;

    float defence_buffer;
    float attack_buffer;
    float bounce_dy;
    float speed;
    int prediction;
    bool active_bounce;

    GameObject idc;

    class BallUnit {
        public GameObject ball;
        public Rigidbody2D rb;
        public float hit_pos;
        public float hit_time;
        public float leave_time;
        public float[] hit_ext;

        public BallUnit(GameObject ball) {
            this.ball = ball;
            this.rb = ball.GetComponent<Rigidbody2D>();
        }
    }

    float last_refresh_time = 0.0f;
    float refresh_freq = 0.2f;

    List<BallUnit> defence_queue = new List<BallUnit>();
    List<BallUnit> attack_queue = new List<BallUnit>();
    List<float[]> detected_segments;


    public void initialize(GameObject board, GameObject ball, int player_id){
        bot = board.AddComponent<Bot>();
        var cc = board.GetComponent<CapsuleCollider2D>();
        board_thick = cc.size.y;
        board_halfsize = cc.size.x / 2;
        board_y = player_id == 1 ? 
            board.transform.position.y - board_thick/2 : board.transform.position.y + board_thick/2;
        ball_r = ball.GetComponent<CircleCollider2D>().radius;

        wall_x = 2.69f;
        this.player_id = player_id;

#if UNITY_EDITOR
        idc = GameObject.Find("Idc");
#endif
    }

    public void reload(LevelParam param, int bot_level){

        this.defence_buffer = param.defence_buffer;
        this.attack_buffer = param.attack_buffer;
        this.bounce_dy = param.bounce_dy * param.bounce_activate_range_rel;
        this.speed = param.bot_speed[bot_level];
        this.prediction = param.bot_prediction[bot_level];
        this.active_bounce = param.does_attack[bot_level];

        bot.set_speed(speed);
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
            attack_queue.Add(bu);
            attack_queue.Sort((x,y)=>x.hit_time.CompareTo(y.hit_time));
        }
        else{
            defence_queue.Add(bu);
            defence_queue.Sort((x,y)=>x.hit_time.CompareTo(y.hit_time));    // A heap would be better
        }

        last_refresh_time = Time.time;
    }

    public void response(){
        // update queue; remove already passed; choose the closest one
        // avoid hit position

        if (Time.time - last_refresh_time > refresh_freq){
            foreach (var bu in defence_queue){
                update_hit_pos(bu);
            }
            foreach (var bu in attack_queue){
                update_hit_pos(bu);
            }
            defence_queue.Sort((a,b)=>a.hit_time.CompareTo(b.hit_time));
            attack_queue.Sort((a,b)=>a.hit_time.CompareTo(b.hit_time));
            last_refresh_time = Time.time;
        }

        for(int i = 0; i < defence_queue.Count;i++){
            if (defence_queue[i].leave_time < 0 || defence_queue[i].ball.gameObject == null){
                defence_queue.RemoveAt(i);
            }
        }
        for(int i = 0; i < attack_queue.Count;i++){
            if (attack_queue[i].leave_time < 0 || attack_queue[i].ball.gameObject == null || attack_queue[i].rb == null){
                attack_queue.RemoveAt(i);
            }
        }

        var ret = get_target_pos();

        if (ret.Item1 == 1 || ret.Item1 == 3){
            bot.set_target(ret.Item2[0]);
        }
        else if (ret.Item1 == 2 || ret.Item1 == 4){
            bot.set_target(ret.Item2[1]);
        }
        else if (ret.Item1 == -1){
            bot.set_target(0);
        }

        bool do_move = ret.Item1 > 0;

        if (attack_queue.Count > 0){

            float hit_left = attack_queue[0].hit_pos - board_halfsize + attack_buffer;
            float hit_right = attack_queue[0].hit_pos + board_halfsize - attack_buffer;
            float x = bot.get_pos();

            if (!do_move && (defence_queue.Count == 0 || attack_queue[0].hit_time < defence_queue[0].hit_time)){
                var available_segments = intersect_front(attack_queue[0].hit_time);
                if (!available_segments.Item2){
                    // intersection hit_left,hit_right with avaialble_segments.item1, we got two points or nothing
                    var real_available_segments = _intersect22(available_segments.Item1, new float[2]{hit_left, hit_right});
                    
                    // if not nothing, select the nearest one.
                    if (real_available_segments != null){
                        bot.set_target(Mathf.Abs(real_available_segments[0] - x) < Mathf.Abs(real_available_segments[1] - x) ? 
                            real_available_segments[0] : real_available_segments[1]);
                    }

                }
            }

            if (active_bounce && attack_queue[0].rb != null && x > hit_left && x < hit_right){
                var dy = (board_y - attack_queue[0].rb.position.y) * (player_id == 1 ? 1.0f : -1.0f) - ball_r;
                if (dy > 0 && dy < bounce_dy){
                    bot.bounce();
                }
            }
        }

    }

    private (int, float[]) get_target_pos(){
        // 0-> no need to move; 1->left; 2->right; 3->left+right, prefer left; 4->left+right, prefer right
        // -1-> no ball in queue, prefer center



        if (defence_queue.Count > 0){
            this.detected_segments = get_sections();
        }
        else{
            this.detected_segments = new List<float[]>();
            return (-1, new float[2]{0, 0});
        }
        
        var h = get_nearest_section(detected_segments);

        float hit_left = h[0];
        float hit_right = h[1];
        float hit_time_left = h[2];
        float hit_time_right = h[3];
/*        hit_left = attack_queue[0].hit_pos - attack_queue[0].hit_ext[0] - board_halfsize - defence_buffer;
        hit_right = attack_queue[0].hit_pos + attack_queue[0].hit_ext[1] + board_halfsize + defence_buffer;
        hit_time_left = attack_queue[0].hit_time;
        hit_time_right = attack_queue[0].hit_time;*/
        

#if UNITY_EDITOR
        if (player_id == 1){
            idc.transform.position = new Vector3((hit_left + hit_right)/2, idc.transform.position.y, -0.1f);
            idc.transform.localScale = new Vector3((hit_right - hit_left - 2*board_halfsize - 2*defence_buffer) / (2*board_halfsize), 0.1f, 1.0f);    
        }
#endif

        float x = bot.get_pos();

        if (x < hit_left || x > hit_right){
            return (0, new float[2]{hit_left, hit_right});
        }

        bool left_possible = hit_left > -wall_x + board_halfsize && (x - hit_left) / speed < hit_time_left;
        bool right_possible = hit_right < wall_x - board_halfsize && (hit_right - x) / speed < hit_time_right;

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
    
    public float[] get_nearest_section(List<float[]> sections){
        // sort by distances
        float mint = Mathf.Infinity;
        float[] mintu = sections[0];
        foreach (var tu in sections){
            var t = Mathf.Min(tu[2], tu[3]);
            if (t < mint){
                mint = t;
                mintu = tu;
            }
        }
        return mintu;
    }

    public (float[], bool) intersect_front(float y){
        // returns: 
        // Not inside a segment: ([xl, xr], false)
        // Inside a segment: ([xl1, xl2, xr1, xr2], true)
        // assuming y < everything

        List<(float, float)> segments = new List<(float, float)>();

        foreach (var tu in detected_segments){
            var myl = tu[0] + (tu[2] - y) * speed;
            var myr = tu[1] - (tu[3] - y) * speed;

            if (myl < myr){
                segments.Add((myl, myr));
            }
        }

        var x = bot.get_pos();

        float left_x1 = -wall_x, left_x2 = -wall_x;
        float right_x1 = wall_x, right_x2 = wall_x;
        bool inside_segment = false;

        foreach (var s in segments){
            if (s.Item1 < x && s.Item2 > x) inside_segment = true;

            if (s.Item2 > left_x1 && s.Item2 < x) left_x1 = s.Item2;
            if (s.Item1 > left_x2 && s.Item1 < x) left_x2 = s.Item1;
            if (s.Item2 < right_x1 && s.Item2 > x) right_x1 = s.Item2;
            if (s.Item1 < right_x2 && s.Item1 > x) right_x2 = s.Item1;
        }

        if (!inside_segment){
            return (new float[2]{left_x1, right_x2}, false);
        }
        else{
            return (new float[4]{left_x1, left_x2, right_x1, right_x2}, true);
        }
    }

    private List<float[]> get_sections(){
        // returns: safepos left, safepos right; hittime left, hittime right for each element

        List<Vector2[]> triangles = new List<Vector2[]>();

        for (int i = 0; i < Mathf.Min(defence_queue.Count, prediction); i++){

            var xl = defence_queue[i].hit_pos - defence_queue[i].hit_ext[0] - board_halfsize - defence_buffer;
            var xr = defence_queue[i].hit_pos + defence_queue[i].hit_ext[1] + board_halfsize + defence_buffer;

            // expansion due to the difference between leave_time and hit_time
           /* var dt = (xr - xl)/2/speed;
            xl = (xl+xr)/2 - (xr-xl)/2 * (dt + defence_queue[i].leave_time - defence_queue[i].hit_time) / dt;
            xr = (xl+xr)/2 + (xr-xl)/2 * (dt + defence_queue[i].leave_time - defence_queue[i].hit_time) / dt;*/

            triangles.Add(new Vector2[3]{
                new Vector2((xr + xl)/2, defence_queue[i].hit_time - (xr - xl)/2/speed), 
                new Vector2(xl,  defence_queue[i].hit_time),
                new Vector2(xr, defence_queue[i].hit_time)});
        }

        if (triangles.Count == 0){
            return new List<float[]>(){new float[4]{wall_x, -wall_x, Mathf.Infinity, Mathf.Infinity}};
        }

        // detect intersection
        List<int>[] adj_list = new List<int>[triangles.Count];
        for (int i = 0; i < triangles.Count; i++){
            adj_list[i] = new List<int>();
        }

        for (int i = 0; i < triangles.Count; i++){
            for (int j = 0; j < triangles.Count; j++){
                // only consider i at bottom of j

                if (triangles[i][1].y < triangles[j][1].y && 
                    triangles[i][1].y > triangles[j][0].y){
                    var scaled_d = new Vector2(triangles[j][1].x, triangles[j][2].x) * 
                        (triangles[i][1].y - triangles[j][0].y) / (triangles[j][1].y - triangles[j][0].y);
                    if ((scaled_d.y < triangles[i][1].x) ^ (scaled_d.x < triangles[i][2].x)){
                        adj_list[j].Add(i);
                        adj_list[i].Add(j);
                    }
                }
            }
        }

        List<float[]> triangle_unions = new List<float[]>();
        bool[] mask = new bool[triangles.Count];    // default = false

        for (int i = 0; i < triangles.Count; i++){
            if (mask[i])continue;
            var m_union = new List<int>();
            _dfs_unions(i, adj_list, m_union, mask);
            triangle_unions.Add(_get_union_vertices(triangles, m_union));
        }
        return triangle_unions;
    }

    private void _dfs_unions(int i, List<int>[] adj_list, List<int> m_union, bool[] mask){
        m_union.Add(i);
        mask[i] = true;
        foreach (var j in adj_list[i]){
            if (!mask[j]){
                _dfs_unions(j, adj_list, m_union, mask);
            }
        }
    }

    private float[] _get_union_vertices(List<Vector2[]> triangles, List<int> m_union){
        float lx = Mathf.Infinity, ly = Mathf.Infinity, rx = -Mathf.Infinity, ry = Mathf.Infinity;
        foreach (var i in m_union){
            if (triangles[i][1].x < lx) {
                lx = triangles[i][1].x;
                ly = triangles[i][1].y;
            }
            if (triangles[i][2].x > rx){
                rx = triangles[i][2].x;
                ry = triangles[i][2].y;
            }
        }
        return new float[4]{lx, rx, ly, ry};
    }

    private float[] _intersect22(float[] A, float[] B){
        if (A[0] > B[0] && A[0] < B[1]){
            return new float[2]{A[0], Mathf.Min(A[1], B[1])};
        }
        else if (B[0] > A[0] && B[0] < A[1]){
            return new float[2]{B[0], Mathf.Min(A[1], B[1])};
        }
        else{
            return null;
        }
    }

    private void update_hit_pos(BallUnit bu){
        if (bu.ball.gameObject == null){
            bu.leave_time = -1;
            return;
        }

        var v = bu.rb.velocity;
        var pos = bu.rb.position;

        var t = (Mathf.Abs(board_y - pos.y)) / Mathf.Abs(v.y);
        var wall_width = wall_x * 2 - ball_r * 2;
        var raw_x = wall_width / 2 + pos.x + t * v.x;
        var n = Mathf.Floor(raw_x / wall_width);

        bu.hit_time = (Mathf.Abs(board_y - pos.y) - ball_r) / Mathf.Abs(v.y);
        bu.leave_time = (Mathf.Abs(board_y - pos.y) + board_thick + ball_r) / Mathf.Abs(v.y);

        if (n % 2 == 0){
            bu.hit_pos = raw_x - n * wall_width - wall_width/2;
        }
        else{
            bu.hit_pos = wall_width/2 - (raw_x - n * wall_width);
        }

        var r_ext = ball_r * Mathf.Abs(v.magnitude / v.y);
        var b_ext = (board_thick) * Mathf.Abs(v.x / v.y);
        bu.hit_ext = new float[2]{
            r_ext + (v.x < 0 ? b_ext : 0),
            r_ext + (v.x > 0 ? b_ext : 0)
        };
    }

}
