using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CityGenerator;

public class DebugBuilding : MonoBehaviour {

    public BuildingTextureAtlas atlas;
    public string file;
    private List<Vector3> buildingLot = new List<Vector3>() {
        Vector3.zero, Vector3.left * 10, Vector3.left * 10  + Vector3.forward * 10, Vector3.forward * 10
    };

	// Use this for initialization
	void Start () {


        FacadeGenerator fg = new FacadeGenerator(file);
        IWallComponent primaryFront = fg.GenerateComponent('*');
        IWallComponent doorComponent = fg.GenerateComponent('D');
        List<IWallComponent> primaryTemplate = primaryFront.Evaluate();

        //string list = BuildingCreator.CreateBuildingString(BuildingCreator.RandomBuildingheight(BuildingCreator.BuildingType.RESIDENTIAL));
        string list = "EESE";
        bool roomTrim = UnityEngine.Random.Range(0, 100) > 75; // 25% chance to have room trim
        MeshCreator.BuildingProperties.Roof roofType = MeshCreator.BuildingProperties.Roof.FLAT;
        if(buildingLot.Count <= 6) { // if it's one of the more common building type, allow different ones
            roofType = BuildingCreator.RandomRoofType();
        }
        roofType = MeshCreator.BuildingProperties.Roof.RAISED;
        MeshCreator.BuildingProperties props = new MeshCreator.BuildingProperties(1.5f, list, roomTrim, roofType);
        MeshCreator.WallMesh wallmesh =  BuildingCreator.CreateBuilding(buildingLot, primaryTemplate, doorComponent, props);
        MeshCreator.AssignMeshesToGameObjects(wallmesh.meshes, atlas.RandomMaterialList(BuildingCreator.BuildingType.RESIDENTIAL), gameObject);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
