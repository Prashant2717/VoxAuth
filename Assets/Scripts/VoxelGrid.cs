using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VoxelGrid : MonoBehaviour {

    // private readonly int _voxelGridHeight = 8;
    //
    // private readonly float _voxelSize = 0.5f;
    // private readonly float _voxelSpacing = 0.02f;

    private GameObject[] _voxels;
    private PasswordInputMode _mode;
    private List<int> _password = new ();

    private const int PasswordSize = 4;

    public string message;
    private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");

    public void Initialize(PasswordInputMode mode) {

        _mode = mode;
        
        _voxels = new GameObject[gameObject.transform.childCount];

        for (int i = 0; i < gameObject.transform.childCount; i++) {

            GameObject child = transform.GetChild(i).gameObject;
            child.layer = LayerMask.NameToLayer("Auth");
            child.GetComponent<EyeInteractable>().Initialize(i, Color.blue);
            child.GetComponent<EyeInteractable>().OnClick.AddListener(OnVoxelSelect);
            
            _voxels[i] = child;
        }

        transform.localScale = new Vector3(6,6,6);

    }

    private void OnVoxelSelect(GameObject voxel) {
        int selectedVoxelId = voxel.GetComponent<EyeInteractable>()._id;

        if (_password.Contains(selectedVoxelId)) return;
        if (_password.Count >= PasswordSize) return;
        _password.Add(selectedVoxelId);
        _voxels[voxel.GetComponent<EyeInteractable>()._id]
            .GetComponent<MeshRenderer>().material.SetFloat(OutlineWidth, 1.18f);
    }

    public bool Delete() {
        if (_password.Count < 1) {
            return false;
        } else {
            int lastAddedVoxelId = _password[^1];
            _password.Remove(lastAddedVoxelId);
            _voxels[lastAddedVoxelId]
                .GetComponent<MeshRenderer>().material.SetFloat(OutlineWidth, 1f);
            return true;
        }
    }
    
    public bool Finish() {
        switch (_mode) {
            case PasswordInputMode.Create: return SavePassword();
            case PasswordInputMode.Verify: return VerifyPassword();
            case PasswordInputMode.Test: return LogPasswordEntry();
            default: return false;
        }
    }

    private bool SavePassword() {
        if (_password.Count != PasswordSize) {
            message = "Invalid password (you must select 4 voxels)";
            _password.Clear();
            return false;
        } else {
            PlayerPrefs.SetString("password", string.Join(",", _password));
            PlayerPrefs.Save();
            _password.Clear();
            message = "Password created successfully!";
            return true;
        }
    }

    private bool VerifyPassword() {
        if (string.Join(",", _password) == GetSavedPassword()) {
            message = "Password input success!";
            return true;
        } else {
            message = "Wrong password! Try again";
            _password.Clear();
            return false;
        }
    }
    
    private bool LogPasswordEntry() {
        if (_password.Count != PasswordSize) {
            message = "Invalid password (you must select 4 voxels)";
            _password.Clear();
            return false;
        }
        
        GlobalVars globalVars = GlobalVars.Instance;
        string[] variables = { string.Join(",", _password) };
        bool logged = EventManager.Instance.QueueEvent(new CreateEntryEvent(
                    _participantName: globalVars.currentParticipant,
                    _experimentVariables: variables, 
                    _category: GlobalVars.LogCategory.controlPosition, 
                    _value: "")
        );

        if (logged) {
            message = "Input saved.";
            UpdateProgress();
        } else {
            message = "Something went wrong";
        }

        return logged;
    }

    private void UpdateProgress() {
        GlobalVars globalVars = GlobalVars.Instance;

        if (globalVars.subprogress >= 3) {
            globalVars.subprogress = 1;

            if (globalVars.progress < 3) {
                globalVars.progress++;
            }
        } else {
            globalVars.subprogress++;
        }
    }

    private string GetSavedPassword() {
        return PlayerPrefs.GetString("password", "");
    }

}