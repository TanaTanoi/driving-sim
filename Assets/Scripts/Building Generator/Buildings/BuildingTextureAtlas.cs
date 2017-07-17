using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CityGenerator/BuildingTextureAtlas", order = 2)]
public class BuildingTextureAtlas : ScriptableObject {
    public BuildingMaterial[] structuralMaterials;
    public BuildingMaterial[] secondaryStructuralMaterials;
    public BuildingMaterial[] windowMaterials;
    public BuildingMaterial[] doorMaterials;
    public BuildingMaterial[] roofMaterials;

    public List<Material> RandomMaterialList(BuildingCreator.BuildingType type) {
        List<Material> mats = new List<Material>();
        mats.Add(RandomStructuralMaterial(type));
        mats.Add(RandomSecondaryStructuralMaterial(type));
        mats.Add(RandomRoofMaterial(type));
        mats.Add(RandomWindowMaterial(type));
        mats.Add(RandomDoorMaterial(type));
        return mats;
    }

    public Material RandomStructuralMaterial(BuildingCreator.BuildingType type) {
        return RandomMaterial(type, structuralMaterials);
    }

   public Material RandomSecondaryStructuralMaterial(BuildingCreator.BuildingType type) {
        return RandomMaterial(type, secondaryStructuralMaterials);
    }
    public Material RandomWindowMaterial(BuildingCreator.BuildingType type) {
        return RandomMaterial(type, windowMaterials);
    }
    public Material RandomDoorMaterial(BuildingCreator.BuildingType type) {
        return RandomMaterial(type, doorMaterials);
    }
    public Material RandomRoofMaterial(BuildingCreator.BuildingType type) {
        return RandomMaterial(type, roofMaterials);
    }
    private Material RandomMaterial(BuildingCreator.BuildingType type, BuildingMaterial[] materials) {
        List<BuildingMaterial> mats = new List<BuildingMaterial>();
        mats.AddRange(materials.Where(x => x.SupportsType(type)));
        return mats[Random.Range(0, mats.Count)].mat;
    }

    [System.Serializable]
    public struct BuildingMaterial {
        public Material mat;
        public bool residential;
        public bool skyscraper;

        public bool SupportsType(BuildingCreator.BuildingType type) {
            switch(type) {
                case BuildingCreator.BuildingType.RESIDENTIAL:
                    return residential;
                case BuildingCreator.BuildingType.SKYSCRAPER:
                    return skyscraper;
                default:
                    return false;
            }
        }
    }

}
