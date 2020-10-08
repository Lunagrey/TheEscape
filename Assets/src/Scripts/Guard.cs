using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Guard : MonoBehaviour {
    [SerializeField] private float _maxDetectRange = 20f;
    [SerializeField] private float _maxPursueTime = 5f;
    [SerializeField] private float _maxCd = 5f;
    [SerializeField] private Transform[] _targetsDest;

    private NavMeshAgent _meshAgent;
    private Vector3 _basePos;
    private int _targetIndex = 0;
    private int _indexDir = 1;
    private int _layerMask;
    private bool _foundIntruder = false;
    private float _actualPursueTime = 0f;
    private float _actualCd = 0f;

    private void Awake() {
        this._meshAgent = this.GetComponent<NavMeshAgent>();
        CalculateNextDestination();
        this._layerMask = LayerMask.GetMask("raycastable");
    }

    // Update is called once per frame
    void Update() {
        if (this._foundIntruder) {
            this._actualPursueTime += Time.deltaTime;
            if (this._actualPursueTime >= this._maxPursueTime) {
                this._foundIntruder = false;
                this._meshAgent.speed = 3.5f;
                this.CalculateNearestPoint();
            } else {
                this._meshAgent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
            }
        } else {
            if (this.transform.position.x == this._targetsDest[this._targetIndex].position.x
                && this.transform.position.z == this._targetsDest[this._targetIndex].position.z) {
                CalculateNextDestination();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && IsVisibleToGuard(other.transform.position) && this._foundIntruder == false) {
            this._foundIntruder = true;
            this._actualPursueTime = 0f;
            this._meshAgent.speed = 7f;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player" && IsVisibleToGuard(collision.transform.position)) {
            Debug.Log("INTRUDER CAUGHT");
            this._foundIntruder = false;
            CalculateNearestPoint();
            // respawn player outside here
        }
    }

    private bool IsVisibleToGuard(Vector3 target) {
        if (Physics.Raycast(this.transform.position, (target - this.transform.position), this._maxDetectRange, this._layerMask)) {
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

    private void CalculateNearestPoint() {
        float distance = 999999f;
        for (int i = 0; i < this._targetsDest.Length; i++) {
            float dist = Mathf.Sqrt(Mathf.Pow((this.transform.position.x - this._targetsDest[i].position.x), 2) + Mathf.Pow((this.transform.position.z - this._targetsDest[i].position.z), 2) + Mathf.Pow((this.transform.position.y - this._targetsDest[i].position.y), 2));
            if (dist < distance) {
                this._targetIndex = i;
                distance = dist;
            }
        }
        this._meshAgent.SetDestination(this._targetsDest[this._targetIndex].position);
    }

    #endregion
}
