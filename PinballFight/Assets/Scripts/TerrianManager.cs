
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager 
{
    private Dictionary<int, GameObject> grid_dict = new Dictionary<int, GameObject>();
    private GameObject cur_grid;
    public int layer_no_conflict = 0;

    // Load level's grid prefab
    public void preload_level(int level){
        
        var grid_prefab = ResManager.load_obj(ResManager.terrian_path(level));
        if (grid_prefab == null){
            Debug.LogError("Cannot load grid: " + ResManager.terrian_path(level));
        }
        grid_dict[level] = grid_prefab;
    }

    // Instantiate the grid
    public void instantiate_level(int level){
        cur_grid = GameObject.Instantiate(grid_dict[level]);
    }

    // Clear the grid
    public void clear_level(){
        GameObject.Destroy(cur_grid);
        cur_grid = null;
    }

    public GameObject get_grid(){
        return cur_grid;
    }

    // Get specific cell position
    public Vector3Int get_cell_pos(Vector3 world_pos){
        return cur_grid.GetComponent<Grid>().WorldToCell(world_pos);
    }

    // is this position conflict with terrian
    public bool conflicts(Vector3 world_pos){
        var cell_pos = get_cell_pos(world_pos);

        for (int i = 0; i < cur_grid.transform.childCount - layer_no_conflict; i++){
            var tile_assume = cur_grid.transform.GetChild(i).GetComponent<Tilemap>().GetTile(cell_pos);
            if (tile_assume) return true;
        }
        return false;
    }

};