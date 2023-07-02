using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BzKovSoft.ObjectSlicer.Samples;
using BzKovSoft.ObjectSlicer;
using UnityEngine.UI;
using DG.Tweening;

public class ObjectEditorManager : MonoBehaviour
{
    [Header("Bounding Box Setting")]
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;
    [SerializeField] private Transform up;
    [SerializeField] private Transform down;

    private GameObject editableObject;

    [Header("Rotation Setting")]
    [SerializeField] private CustomSlider scrollbar;


    [Header("View Buttons Setting")]
    [SerializeField] private Button topDown;
    [SerializeField] private Button rightSide;
    [SerializeField] private Button forwardSide;

    private void Start()
    {
        scrollbar.onSliderValueChanged += Scrollbar_onSliderValueChanged;
        topDown.onClick.AddListener(OnClickTopDown);
        rightSide.onClick.AddListener(OnClickRightSide);
        forwardSide.onClick.AddListener(OnClickForward);
    }

    void OnClickTopDown()
    {
        editableObject.transform.DORotate(new Vector3(-90, 0, 0), 0.1f);
    }

    void OnClickRightSide()
    {
        editableObject.transform.DORotate(new Vector3(0,0,90),0.1f);
    }

    void OnClickForward()
    {
        editableObject.transform.DORotate(new Vector3(0, 0, 0), 0.1f);
    }

    private void Scrollbar_onSliderValueChanged(float value)
    {
        Vector3 eulerAngles = editableObject.transform.eulerAngles;
        eulerAngles.y = value;
        editableObject.transform.eulerAngles = eulerAngles;
    }

    public void Slice()
    {
        Crop(left, Camera.main.transform.up);
        Crop(right, -Camera.main.transform.up);
        Crop(up, Camera.main.transform.right);
        Crop(down, -Camera.main.transform.right);
    }

    void Crop(Transform side, Vector3 camDir)
    {
        Ray ray = Camera.main.ScreenPointToRay(side.position);//(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

        var sliceId = SliceIdProvider.GetNewSliceId();

        for (int i = 0; i < hits.Length; i++)
        {
            var sliceableA = hits[i].transform.GetComponentInParent<IBzSliceableNoRepeat>();

            Vector3 direction = Vector3.Cross(ray.direction, camDir);
            Plane plane = new Plane(direction, ray.origin);

            if (sliceableA != null)
                sliceableA.Slice(plane, sliceId, OnCropped);
        }
    }

    void OnCropped(BzSliceTryResult result)
    {
        if (result.sliced)
        {
            //result.outObjectPos.SetActive(false);
            Destroy(result.outObjectPos);
        }

    }

    public void SetEditableObject(GameObject editableObject)
    {
        this.editableObject = editableObject;
    }
}
