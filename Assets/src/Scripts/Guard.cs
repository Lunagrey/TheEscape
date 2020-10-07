using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Guard : MonoBehaviour {
    [SerializeField] private float _maxDetectRange = 10f;
    [SerializeField] private float _maxPursueTime = 5f;
    [SerializeField] private Transform[] _targetsDest;

    private NavMeshAgent _meshAgent;
    private Vector3 _basePos;
    private int _targetIndex = 0;
    private int _indexDir = 1;
    private int _layerMask;
    private bool _foundIntruder = false;
    private float _actualPursueTime = 0f;

    private void Awake() {
        this._meshAgent = this.GetComponent<NavMeshAgent>();
        CalculateNextDestination();
        this._layerMask = LayerMask.GetMask("raycastable");
    }

    // Update is called once per frame
    void Update() {
        if (this._foundIntruder) {

        } else {
            if (this.transform.position.x == this._targetsDest[this._targetIndex].position.x
                && this.transform.position.z == this._targetsDest[this._targetIndex].position.z) {
                CalculateNextDestination();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("triggering: " + other.tag);
        if (other.tag == "Player" && IsVisibleToGuard(other.transform.position)) {
            this._foundIntruder = true;
            Debug.Log("VISIBLE BY GUARD");
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            this._foundIntruder = false;
            // respawn player outside here
        }
    }

    private bool IsVisibleToGuard(Vector3 target) {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, (target - this.transform.position), out hit, this._maxDetectRange, this._layerMask)) {
            return true;
        }
        return false;
    }

    #region PathCalculation
    private void CalculateNextDestination() {
        PrepareNextDestination();
        this._meshAgent.SetDestination(this._targetsDest[this._targetIndex].position);
    }

    private void PrepareNextDestination() {
        this._targetIndex += this._indexDir;
        if (this._targetIndex == this._targetsDest.Length) {
            this._targetIndex -= 2;
            this._indexDir = -1;
        }
        else if (this._targetIndex < 0) {
            this._targetIndex = 0;
            this._indexDir = 1;
        }
    }

    #endregion
}
