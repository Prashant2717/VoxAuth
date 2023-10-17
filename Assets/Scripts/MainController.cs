using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Util;

public class MainController : MonoBehaviour {

    public Canvas mainCanvas;
    public GameObject VoxelGenerator;
    public GameObject mainEnvironment;
    public GameObject Panel;
    public GameObject StartButton;
    public GameObject Message;
    public GameObject Finish;
    public GameObject EyeInteractor;
    private VoxelGrid _voxelGrid;

    private void Start() {
        mainEnvironment.SetActive(true);
        mainCanvas.enabled = true;

        Panel = GameObject.Find("Panel");
        StartButton = GameObject.Find("Start");
        Message = GameObject.Find("Message");
        EyeInteractor = GameObject.Find("EyeInteractor");
        Finish = GameObject.Find("Finish");
        ChangePasswordMode(false);

        // set click listeners to components
        mainCanvas.transform.Find("Start").GetComponent<Button>().onClick.AddListener(delegate {
            GlobalVars globalVars = GlobalVars.Instance;
            string order = globalVars.order[globalVars.progress];
            OpenPasswordInterface(order, globalVars.subprogress, PasswordInputMode.Test);
            // OpenPasswordInterface("cube", 1, PasswordInputMode.Test);
        });

        mainCanvas.transform.Find("Finish").GetComponent<Button>().onClick.AddListener(delegate {
            _voxelGrid.Finish();
            ChangePasswordMode(false);
        });
        
        // mainCanvas.transform.Find("StartLog").GetComponent<Button>().onClick.AddListener(delegate {
        //
        //     ShowMessage("StartLog clicked");
        //     bool hhh = EventManager.Instance.QueueEvent(new CreateXMLEvent(
        //             _specificName: true,
        //             _name: "kjhffty",
        //             _participantName: RandomString(9)
        //         )
        //     );
        //
        //     if (hhh) ShowMessage("Event added");
        //     else ShowMessage("event add failed");
        // });
        //
        // mainCanvas.transform.Find("EnterLog").GetComponent<Button>().onClick.AddListener(delegate {
        //     ShowMessage("EnterLog clicked");
        //     string[] variables = {"Volvo", "BMW", "Ford", "Mazda"};
        //     
        //     bool hhh = EventManager.Instance.QueueEvent(new CreateEntryEvent(
        //                 _participantName: "jkgjhgb",
        //                 _experimentVariables: variables, 
        //                 _category: GlobalVars.LogCategory.controlPosition, 
        //                 _value: "")
        //     );
        //     
        //     if (hhh) ShowMessage("Event added");
        //     else ShowMessage("event add failed");
        // });
        //
        // mainCanvas.transform.Find("CloseLog").GetComponent<Button>().onClick.AddListener(delegate {
        //     ShowMessage("CloseLog clicked");
        //     bool hhh = EventManager.Instance.QueueEvent(new EndLogEvent());
        //     
        //     if (hhh) ShowMessage("Event added");
        //     else ShowMessage("event add failed");
        // });
    }

    private void Update() {

        Vector2 on = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger);
        if (Mathf.Abs(on[0]) > 0.05f || Mathf.Abs(on[1]) > 0.05f) {
            float tiltAroundX = on[1] * 4;
            float tiltAroundY = on[0] * 4;
            Quaternion rotation = Quaternion.Euler(tiltAroundX, -tiltAroundY, 0);
            _voxelGrid.transform.Rotate(rotation.eulerAngles, Space.World);
        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)) {
            _voxelGrid.transform.rotation = new Quaternion();
        }
        
        if (OVRInput.GetDown(OVRInput.Button.Two)) {
            _voxelGrid.Delete();
        } else if (OVRInput.GetDown(OVRInput.Button.One)) {
            _voxelGrid.Finish();
            ChangePasswordMode(false);
        }
    }

    private void OpenPasswordInterface(string shape, int level, PasswordInputMode mode) {
        _voxelGrid = VoxelGenerator
            .GetComponent<VoxelGenerator>()
            .LoadVoxelGrid(shape, level, mode)
            .GetComponent<VoxelGrid>();

        ChangePasswordMode(true);
    }

    private void ShowMessage(string message) {
        Message.GetComponent<TMP_Text>().text = message;
    }

    private void ChangePasswordMode(bool mode) {
        Panel.SetActive(!mode);
        StartButton.SetActive(!mode);
        Message.SetActive(!mode);
        
        ShowMessage(_voxelGrid?.message);

        Finish.SetActive(mode);
        if (!mode) {
            EyeInteractor.GetComponent<EyeTrackingRay>().ClearFocus();
            Destroy(_voxelGrid?.gameObject);
        }
    }
}