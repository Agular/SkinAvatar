using UnityEngine;

public class AvatarController : MonoBehaviour {

    public int Index { get; set; }
    public ulong TrackingId { get; set; }
    public int MaxFilterSamples { get; set; }

    private GameObject sign;
    private AvatarModelController ModelCont;
    private SkeletonViewController SkeletonCont;
    private Filter filter;
    private bool skeletonIsRendered = false;
    private bool modelIsRendered = false;

	// Use this for initialization
	void Awake () {
        Index = 0;
        TrackingId = 0;
        MaxFilterSamples = 10;

        GameObject skeleton = new GameObject();
        skeleton.name = "Skeleton";
        skeleton.transform.parent = this.transform;
        SkeletonCont = skeleton.AddComponent<SkeletonViewController>();

        filter = new MovingAverageFilter(MaxFilterSamples);

        sign = new GameObject("Avatar sign");
        sign.transform.rotation = Camera.main.transform.rotation;
        sign.transform.parent = this.transform;
        TextMesh tm = sign.AddComponent<TextMesh>();
        tm.text = Index.ToString();
        tm.color = new Color(1f, 1f, 1f);
        tm.fontStyle = FontStyle.Bold;
        tm.alignment = TextAlignment.Center;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.fontSize = 12;
        sign.GetComponent<MeshRenderer>().enabled = false;
    }

    public void AssignModel(GameObject model, bool renderingMode = false)
    {
        DestroyOldModel();
        GameObject avaModel = Instantiate(model, Vector3.zero, Quaternion.identity);
        avaModel.name = "Model";
        avaModel.transform.parent = this.transform;
        ModelCont = avaModel.GetComponent<AvatarModelController>();
        if (!renderingMode)
        {
            ModelCont.DisableRendering();
        }
        else{
            ModelCont.EnableRendering();
        }
    }

    private void DestroyOldModel()
    {
        if (ModelCont == null)
        {
            return;
        }
        Destroy(ModelCont.gameObject);
        ModelCont = null;
    }

    public void RefreshAvatarBodyData(Avatar.Body bodyData)
    {
        Avatar.Body filteredBody = filter.GetFilteredData(bodyData);
        if (skeletonIsRendered)
        {
            SkeletonCont.RefreshSkeletonData(filteredBody);
        }
        if (modelIsRendered)
        {
            ModelCont.RefreshModelData(filteredBody);
        }
        UpdateAvatarSignPosition();
    }

    public void AddToTracking(ulong trackingId)
    {
        TrackingId = trackingId;
    }

    public void RemoveFromTracking()
    {
        TrackingId = 0;
        SkeletonCont.DisableRendering();
        ModelCont.DisableRendering();
        sign.GetComponent<MeshRenderer>().enabled = false;
        filter.Reset();
    }

    public void SetModelRendering(bool value)
    {
        if (value)
        {
            ModelCont.EnableRendering();
            modelIsRendered = true;
            if (!skeletonIsRendered)
            {
                sign.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        else
        {
            ModelCont.DisableRendering();
            filter.Reset();
            modelIsRendered = false;
            if (!skeletonIsRendered)
            {
                sign.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public void SetSkeletonRendering(bool value)
    {
        if (value)
        {
            SkeletonCont.EnableRendering();
            skeletonIsRendered = true;
            if (!modelIsRendered)
            {
                sign.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        else
        {
            SkeletonCont.DisableRendering();
            filter.Reset();
            skeletonIsRendered = false;
            if (!modelIsRendered)
            {
                sign.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public string GetAvatarModelName()
    {
        return ModelCont.ModelName;
    }

    private void UpdateAvatarSignPosition()
    {
        if (modelIsRendered)
        {
            sign.transform.position = transform.Find("Model").Find("Armature").Find("Head_Controller")
                .transform.position + Vector3.up * 3f;
        }
        else if(skeletonIsRendered)
        {
            sign.transform.position = transform.Find("Skeleton").Find("Head").transform.position + Vector3.up * 3f;
        }
    }

    public int GetIndex()
    {
        return Index;
    }

    public void SetIndex(int index)
    {
        Index = index;
        sign.GetComponent<TextMesh>().text = index.ToString();
    }
}
