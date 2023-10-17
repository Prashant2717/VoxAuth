using Unity.VisualScripting;
using UnityEngine;

public class VoxelGenerator : MonoBehaviour {
    
    public GameObject voxelPrefabCubeEasy;
    public GameObject voxelPrefabCubeMedium;
    public GameObject voxelPrefabCubeHard;
    public GameObject voxelPrefabPyramidEasy;
    public GameObject voxelPrefabPyramidMedium;
    public GameObject voxelPrefabPyramidHard;
    public GameObject voxelPrefabSphereEasy;
    public GameObject voxelPrefabSphereMedium;
    public GameObject voxelPrefabSphereHard;
    public GameObject voxelPrefabStarEasy;
    public GameObject voxelPrefabStarMedium;
    public GameObject voxelPrefabStarHard;

    public GameObject LoadVoxelGrid(string shape, int level, PasswordInputMode mode) {
        
        GameObject voxelGridPrefab;
        
        switch (shape) { 
            case "cube":
                switch (level) {
                    case 1:
                        voxelGridPrefab = voxelPrefabCubeEasy;
                        break;
                    case 2: 
                        voxelGridPrefab = voxelPrefabCubeMedium;
                        break;
                    case 3: 
                        voxelGridPrefab = voxelPrefabCubeHard;
                        break;
                    default: 
                        voxelGridPrefab = voxelPrefabCubeEasy;
                        break;
                }
                break;
            case "pyramid": 
                switch (level) {
                    case 1:
                        voxelGridPrefab = voxelPrefabPyramidEasy;
                        break;
                    case 2: 
                        voxelGridPrefab = voxelPrefabPyramidMedium;
                        break;
                    case 3: 
                        voxelGridPrefab = voxelPrefabPyramidHard;
                        break;
                    default: 
                        voxelGridPrefab = voxelPrefabPyramidEasy;
                        break;
                }
                break;
            case "sphere":
                switch (level) {
                    case 1:
                        voxelGridPrefab = voxelPrefabSphereEasy;
                        break;
                    case 2: 
                        voxelGridPrefab = voxelPrefabSphereMedium;
                        break;
                    case 3: 
                        voxelGridPrefab = voxelPrefabSphereHard;
                        break;
                    default: 
                        voxelGridPrefab = voxelPrefabSphereEasy;
                        break;
                }
                break;
            case "star": 
                switch (level) {
                    case 1:
                        voxelGridPrefab = voxelPrefabStarEasy;
                        break;
                    case 2: 
                        voxelGridPrefab = voxelPrefabStarMedium;
                        break;
                    case 3: 
                        voxelGridPrefab = voxelPrefabStarHard;
                        break;
                    default: 
                        voxelGridPrefab = voxelPrefabStarEasy;
                        break;
                }
                break;
            default: 
                voxelGridPrefab = voxelPrefabCubeEasy;
                break;
        }
        
        GameObject voxelGrid = Instantiate(voxelGridPrefab, transform, false);
        voxelGrid.GetComponent<VoxelGrid>().Initialize(mode);
        voxelGrid.layer = LayerMask.NameToLayer("Auth");

        return voxelGrid;
    }
}