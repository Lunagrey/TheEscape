using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class SecurityCam : MonoBehaviour {
    [SerializeField] private GameObject _generator;
    [SerializeField] private MeshRenderer _camState;
    [SerializeField] private GameObject _zone;
    [SerializeField] private float _maxDetectRange = 10f;

    private int _layerMask;
    private Sequence _seq;

    void Start() {
        this._layerMask = LayerMask.GetMask("raycastable");
        Sequence();
    }


    public void BreakCam()
    {
        float distance = 999999f;
        GameObject nearest = null;
        foreach (GameObject guard in GameObject.FindGameObjectsWithTag("Guard")) {
            float dist = Mathf.Sqrt(Mathf.Pow((this._generator.transform.position.x - guard.transform.position.x), 2) + Mathf.Pow((this._generator.transform.position.z - guard.transform.position.z), 2) + Mathf.Pow((this._generator.transform.position.y - guard.transform.position.y), 2));
            if (dist < distance) {
                nearest = guard;
                distance = dist;
            }
        }
        if (nearest != null) {
            nearest.GetComponent<Guard>().taskTargetPoint = this._generator.transform;
            nearest.GetComponent<Guard>().SetTask(Guard.GuardTask.GoToCam, -1);
        }
        this._seq.Pause();
        this._zone.SetActive(false);
        this._camState.material.color = new Color(1, 0, 0);

    }

    public void RepairCam() {
        this._zone.SetActive(true);
        this._seq.Play();
        this._camState.material.color = new Color(0, 1, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && IsVisibleToGuard(other.transform.position)) {
            foreach (GameObject guard in GameObject.FindGameObjectsWithTag("Guard")) {
                guard.GetComponent<Guard>().SetTask(Guard.GuardTask.FollowIntruder, 10f);
            }
        }
    }

    private bool IsVisibleToGuard(Vector3 target)
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, (target - this.transform.position), out hit, this._maxDetectRange, this._layerMask)
            && hit.transform.tag == "Player")
        {
            return true;
        }
        return false;
    }

    private void Sequence() {
        this._seq = DOTween.Sequence();
        this._seq.Append(this.transform.DORotate(this.transform.rotation.eulerAngles + Vector3.up * 45f, 3f));
        this._seq.Append(this.transform.DORotate(this.transform.rotation.eulerAngles + Vector3.up, 3f));
        this._seq.Append(this.transform.DORotate(this.transform.rotation.eulerAngles + Vector3.up * -45, 3f));
        this._seq.Append(this.transform.DORotate(this.transform.rotation.eulerAngles + Vector3.up, 3f));
        this._seq.OnComplete(() => { this._seq.Restart(); });
    }
}
