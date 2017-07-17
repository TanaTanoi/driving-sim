using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CityGenerator;

public class BuildingCreator {
    // Count is not a building type, it is simply used for the enum
    public enum BuildingType { RESIDENTIAL, SKYSCRAPER, Count }

    public static float FLOOR_HEIGHT = 4f;


    private static Range RESIDENTIAL_HEIGHT_RANGE = new Range(1, 5);
    private static Range SKYSCRAPER_HEIGHT_RANGE = new Range(4, 12);

    public struct Range{
      int lowerBound;
      int upperBound;
      public Range(int lb, int ub){
        lowerBound = lb;
        upperBound = ub;
      }

      public int RandomInRange(){
        return UnityEngine.Random.Range(lowerBound, upperBound);
      }
    }

    public static MeshCreator.WallMesh CreateBuilding(List<Vector3> buildingLot, List<IWallComponent> template, IWallComponent door, MeshCreator.BuildingProperties properties) {

        MeshCreator.WallMesh wallmesh = new MeshCreator.WallMesh();
        wallmesh = MeshCreator.CreateBuildingMesh(template, door, buildingLot, properties);
        
        wallmesh.RecalculateNormals();
        return wallmesh;
    }

    public static MeshCreator.WallMesh CreateBuilding(List<Vector3> buildingLot, BuildingType type) {
                FacadeGenerator fg = new FacadeGenerator("Assets/GoodSystem.txt");
        IWallComponent primaryFront = fg.GenerateComponent('*');
        IWallComponent doorComponent = fg.GenerateComponent('D');
        List<IWallComponent> primaryTemplate = primaryFront.Evaluate();

        string list = CreateBuildingString(RandomBuildingheight(type));
        bool roomTrim = UnityEngine.Random.Range(0, 100) > 75; // 25% chance to have room trim
        MeshCreator.BuildingProperties.Roof roofType = MeshCreator.BuildingProperties.Roof.FLAT;
        if(buildingLot.Count <= 6) { // if it's one of the more common building type, allow different ones
            roofType = RandomRoofType();
        }
        MeshCreator.BuildingProperties props = new MeshCreator.BuildingProperties(FLOOR_HEIGHT, list, roomTrim, roofType);
        return CreateBuilding(buildingLot, primaryTemplate, doorComponent, props);
    }


    public static MeshCreator.BuildingProperties.Roof RandomRoofType() {
        MeshCreator.BuildingProperties.Roof[] roofs = new MeshCreator.BuildingProperties.Roof[] {
            MeshCreator.BuildingProperties.Roof.FLAT,
            MeshCreator.BuildingProperties.Roof.BORDERED, MeshCreator.BuildingProperties.Roof.OFFSETSQUARE,
            MeshCreator.BuildingProperties.Roof.POINTED, MeshCreator.BuildingProperties.Roof.RAISED
        };
        return roofs[UnityEngine.Random.Range(0, roofs.Length)];
    }

    public static int RandomBuildingheight(BuildingType type){
      switch(type){
        case BuildingType.RESIDENTIAL:
          return RESIDENTIAL_HEIGHT_RANGE.RandomInRange();
        case BuildingType.SKYSCRAPER:
          return SKYSCRAPER_HEIGHT_RANGE.RandomInRange();
        default:
          return 1;
      }
    }

    public static string CreateBuildingString(int height){
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      for(int i = 0; i < height; i++){
        sb.Append(RandomGrowInstruction());
      }
      return sb.ToString();
    }

    public static string RandomGrowInstruction(){
      string[] values = new string[] { "E", "E", "E", "SE"};
      return values[UnityEngine.Random.Range(0, values.Length)];
    }

    public static GameObject CreateBuildingObject(List<Vector3> floorplan, BuildingType type, BuildingTextureAtlas atlas) {
        return CreateBuildingObject(floorplan, type,
            atlas.RandomStructuralMaterial(type), atlas.RandomSecondaryStructuralMaterial(type),
            atlas.RandomRoofMaterial(type), atlas.RandomWindowMaterial(type), atlas.RandomDoorMaterial(type));
    }

    public static GameObject CreateBuildingObject(List<Vector3> floorplan, BuildingType type, Material structMat, Material structMat2, Material roofMat, Material windowMat, Material doorMat) {
        //MeshCreator.WallMesh wallmesh = BuildingCreator.CreateBuilding(floorplan, type);
        Building building = new GameObject("Building").AddComponent<Building>();
        building.SetParams(floorplan, type, new List<Material>() { structMat, structMat2, roofMat, windowMat, doorMat });
        building.Build();
        return building.gameObject;
    }

    private static BuildingType RandomBuildingType() {
        return (BuildingType)UnityEngine.Random.Range(0, (int)BuildingType.Count);
    }

    private static IWallComponent GenerateGroundFloorTemplate(IWallComponent frontWallTree, IWallComponent doorWallTree, System.Random rand) {
        Array values = Enum.GetValues(typeof(Split.SplitType));
        Split.SplitType type = (Split.SplitType)values.GetValue(rand.Next(values.Length - 1));
        return new Split(type, doorWallTree, frontWallTree); // Don't include Mix for door stuff
    }
}
