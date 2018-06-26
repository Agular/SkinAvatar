using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AvatarMapper : MonoBehaviour
{
    public GameObject BodyManager;
    public GameObject canvas;
    public List<GameObject> models;

    private int bodyCount = 6;
    private Dictionary<int, GameObject> Avatars = new Dictionary<int, GameObject>();
    private BodyManager _BodyManager;
    private GameObject modelMenu;

    void Awake()
    {
        modelMenu = canvas.transform.Find("Model Menu").gameObject;
        for (int i = 1; i <= bodyCount; i++)
        {
            Avatars[i] = new GameObject();
            Avatars[i].name = "Avatar " + i;
            Avatars[i].AddComponent<AvatarController>();
            Avatars[i].GetComponent<AvatarController>().SetIndex(i);
            Avatars[i].GetComponent<AvatarController>().AssignModel(models[0]);
        }
        SetupModelMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Canvas _canvas = canvas.GetComponent<Canvas>();
            _canvas.enabled = !_canvas.enabled;
        }

        if (BodyManager == null)
        {
            return;
        }

        _BodyManager = BodyManager.GetComponent<BodyManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Avatar.Body[] data = _BodyManager.GetAvatarBodies();
        if (data == null)
        {
            return;
        }
        List<ulong> trackedIds = new List<ulong>();

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        Dictionary<int, ulong> knownAvatarIds =  GetAvatarTrackingIds();

        foreach (KeyValuePair<int, ulong> avatarIdPair in knownAvatarIds)
        {
            if (!trackedIds.Contains(avatarIdPair.Value))
            {
                Avatars[avatarIdPair.Key].GetComponent<AvatarController>().RemoveFromTracking();
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }
            if (body.IsTracked)
            {
                int avaIdx = 0;
                if (!knownAvatarIds.ContainsValue(body.TrackingId))
                {
                    //mappimine
                    avaIdx = MapBodyToAvatar(body.TrackingId, knownAvatarIds);
                    SetupAvatarRendering(avaIdx);
                }
                if (avaIdx == 0)
                {
                    avaIdx = knownAvatarIds.FirstOrDefault(a => a.Value == body.TrackingId).Key;
                }
                Avatars[avaIdx].GetComponent<AvatarController>().RefreshAvatarBodyData(body);
            }
        }
        UpdateModelMenu();
    }



    private int MapBodyToAvatar(ulong trackingId, Dictionary<int, ulong> knownAvatarIds)
    {
        for (int i = 1; i <= bodyCount; i++)
        {
            if (!knownAvatarIds.ContainsKey(i))
            {
                Avatars[i].GetComponent<AvatarController>().AddToTracking(trackingId);
                return i;
            }
        }
        return -1;
    }

    private Dictionary<int, ulong> GetAvatarTrackingIds()
    {
        Dictionary<int, ulong> avatarTrackingIds = new Dictionary<int, ulong>();
        foreach (KeyValuePair<int, GameObject> avatar in Avatars)
        {
            AvatarController avaController = avatar.Value.GetComponent<AvatarController>();
            if (avaController.TrackingId != 0)
            {
                avatarTrackingIds[avatar.Key] = avaController.TrackingId;
            }
        }
        return avatarTrackingIds;
    }

    private void SetupModelMenu()
    {
        for (int i = 1; i <= bodyCount; i++)
        {
            Dropdown dropdown = modelMenu.transform.Find("Avatar " + i + " Dropdown").GetComponent<Dropdown>();
            dropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (GameObject model in models)
            {
                if (!options.Contains(model.name))
                {
                    options.Add(model.name);
                }
            }
            
            dropdown.AddOptions(options);
            string avaModelName = Avatars[i].GetComponent<AvatarController>().GetAvatarModelName();

            for (int j = 0; j < dropdown.options.Count; j++)
            {
                if (dropdown.options[j].text == avaModelName)
                {
                    dropdown.value = j;
                    break;
                }
            }

            dropdown.onValueChanged.AddListener(delegate {
                ChangeAvatarModelFromDropdown(dropdown);
            });
            Toggle avaModelToggle = modelMenu.transform.Find("Avatar " + i + " Model Toggle").GetComponent<Toggle>();
            Toggle avaSkeletonToggle = modelMenu.transform.Find("Avatar " + i + " Skeleton Toggle").GetComponent<Toggle>();

            avaModelToggle.onValueChanged.AddListener( delegate {
                ChangeAvaModelRenderingFromToggle(avaModelToggle);
            });
            avaSkeletonToggle.onValueChanged.AddListener(delegate
            {
                ChangeAvaSkeletonRenderingFromToggle(avaSkeletonToggle);
            });

        }
    }


    private void UpdateModelMenu()
    {
        for (int i = 1; i <= bodyCount; i++)
        {
            Text avatarTrackingText = modelMenu.transform.Find("Avatar " + i + " Tracking Label").GetComponent<Text>();
            if (Avatars[i].GetComponent<AvatarController>().TrackingId != 0)
            {
                avatarTrackingText.text = "Tracking";
                avatarTrackingText.color = Color.green;
            }
            else
            {
                avatarTrackingText.text = "Not Tracking";
                avatarTrackingText.color = Color.red;
            }
        }
    }

    private void SetupAvatarRendering(int avaIdx)
    {
        Toggle avaModelToggle = modelMenu.transform.Find("Avatar " + avaIdx + " Model Toggle").GetComponent<Toggle>();
        Toggle avaSkeletonToggle = modelMenu.transform.Find("Avatar " + avaIdx + " Skeleton Toggle").GetComponent<Toggle>();
        Avatars[avaIdx].GetComponent<AvatarController>().SetModelRendering(avaModelToggle.isOn);
        Avatars[avaIdx].GetComponent<AvatarController>().SetSkeletonRendering(avaSkeletonToggle.isOn);
    }

    private void ChangeAvatarModelFromDropdown(Dropdown dropdown)
    {
        GameObject model = models.Find(x => x.name.Equals(dropdown.options[dropdown.value].text));
        int avatarIndex = Convert.ToInt32(dropdown.transform.name.Split(' ')[1]);
        bool renderModel = modelMenu.transform.Find("Avatar " + avatarIndex + " Model Toggle").GetComponent<Toggle>().isOn;
        Avatars[avatarIndex].GetComponent<AvatarController>().AssignModel(model.gameObject, renderModel);
    }

    private void ChangeAvaSkeletonRenderingFromToggle(Toggle avaSkeletonToggle)
    {
        int avaIdx = Int32.Parse(avaSkeletonToggle.name.Split(' ')[1]);
        if (Avatars[avaIdx].GetComponent<AvatarController>().TrackingId != 0)
        {
            Avatars[avaIdx].GetComponent<AvatarController>().SetSkeletonRendering(avaSkeletonToggle.isOn);
        }
    }

    private void ChangeAvaModelRenderingFromToggle(Toggle avaModelToggle)
    {
        int avaIdx = Int32.Parse(avaModelToggle.name.Split(' ')[1]);
        if (Avatars[avaIdx].GetComponent<AvatarController>().TrackingId != 0)
        {
            Avatars[avaIdx].GetComponent<AvatarController>().SetModelRendering(avaModelToggle.isOn);
        }
    }
}
