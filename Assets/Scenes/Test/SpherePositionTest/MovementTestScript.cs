using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTestScript : MonoBehaviour {

    [SerializeField] Transform earth;
    public bool child;

    private void Start() {
        if (!child) {
            SetupLocation();
            GetComponent<SpawnChildTestScript>().SpawnChild();
        }
    }

    void FixedUpdate() {
        AlignOrganism();
        //GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, 50 * Time.fixedDeltaTime);
        //if (Input.GetKey(KeyCode.LeftArrow)) {
        //    GetModelTransform().Rotate(-Vector3.up * 5f);
        //} else if (Input.GetKey(KeyCode.RightArrow)) {
        //    GetModelTransform().Rotate(Vector3.up * 5f);
        //} else if (RunFromOrgamisnms()) {
        //} else if (!Input.GetKey(KeyCode.UpArrow)) {
        //    //GetModelTransform().Rotate(-Vector3.up * Random.Range(-100, 100) * 2 * Time.fixedDeltaTime);
        //}
    }

    public void SetupLocation () {
        transform.parent.SetParent(earth.GetChild(0));
        Vector3 previousSize = transform.parent.localScale;
        transform.parent.localScale = new Vector3(1, 1, 1);
        transform.parent.localPosition = new Vector3(0, .5f, 0);
        transform.localScale = new Vector3(transform.localScale.x * previousSize.x, transform.localScale.y * previousSize.y, transform.localScale.z * previousSize.z);
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, Random.Range(-360, 360));
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().forward, Random.Range(-360, 360));
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().up, Random.Range(-360, 360));
        GetModelTransform().localEulerAngles = new Vector3(0, Random.Range(-360, 360), 0);
    }

    public void SetupChildLocation(Vector3 position, float range) {
        transform.parent.localScale = new Vector3(1, 1, 1);
        transform.localScale = new Vector3(1, 1, 5);

        transform.parent.SetParent(earth.GetChild(0));
        Vector3 previousSize = transform.parent.localScale;
        transform.parent.localScale = new Vector3(1, 1, 1);
        transform.parent.localPosition = position;
        transform.localScale = new Vector3(transform.localScale.x * previousSize.x, transform.localScale.y * previousSize.y, transform.localScale.z * previousSize.z);

        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, Random.Range(-range, range));
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().forward, Random.Range(-range, range));
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().up, Random.Range(-range, range));
        GetModelTransform().localEulerAngles = new Vector3(0, Random.Range(-360, 360), 0);
    }

    bool RunFromOrgamisnms() {
        float range = 30;
        foreach (var organism in GetAllOrganismsInRange(range)) {
            LookAwayFromPoint(organism.position);
            return true;
        }
        return false;
    }

    public void LookAwayFromPoint(Vector3 _point) {
        GetModelTransform().rotation = Quaternion.LookRotation(GetModelTransform().position - _point);
        AlignOrganism();
    }

    public void LookAtPoint(Vector3 _point) {
        GetModelTransform().rotation = Quaternion.LookRotation(_point - GetModelTransform().position);
        AlignOrganism();
    }

    List<Transform> GetAllOrganismsInRange(float _range) {
        List<Transform> organisms = new List<Transform>();
        foreach (var organism in GetAllOtherOrganisms()) {
            if (Vector3.Distance(GetModelTransform().position,organism.position) <= _range) {
                organisms.Add(organism);
            }
        }
        return organisms;
    }

    List<Transform> GetAllOtherOrganisms() {
        List<Transform> organisms = new List<Transform>();
        for (int i = 0; i < GetRotationTransform().parent.childCount; i++) {
            Transform targetOrganism = GetRotationTransform().parent.GetChild(i).GetChild(0);
            if (targetOrganism != transform) {
                organisms.Add(targetOrganism);
            }
        }
        return organisms;
    }

    void AlignOrganism() {
        GetModelTransform().localEulerAngles = new Vector3(0, GetModelTransform().localEulerAngles.y, 0);

    }

    public Transform GetModelTransform() {
        return transform;
    }
    
    public Transform GetRotationTransform() {
        return transform.parent;
    }
}
