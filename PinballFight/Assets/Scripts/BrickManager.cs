
using UnityEngine;


public class BrickManager {

    int default_durability;

    public void reload(LevelParam param){
        default_durability = param.brick_durability;
    }

    public void generate_map(GameObject grid){
        foreach (Transform child in grid.transform){
            child.gameObject.GetComponent<Brick>().durability = default_durability;

            // TODO random generate other type
        }
        
    }

}