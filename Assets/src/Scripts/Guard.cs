using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
public class Guard : MonoBehaviour {
    public enum GuardTask {
        FollowIntruder,
        RepairCamera,
        GotoDestination,
        GoNearestDest,
        GoToCam
    }
    public Transform taskTargetPoint;
    [SerializeField] private float _maxDetectRange = 20f;
    [SerializeField] private float _maxPursueTime = 5f;
    [SerializeField] private float _maxRepairCamera = 5f;
    [SerializeField] private Transform[] _targetsDest;

    private NavMeshAgent _meshAgent;
    private int _targetIndex = 0;
    private int _indexDir = 1;
    private int _layerMask;
    private float _taskTimer;
    private float _actualTaskTimer;
    private GuardTask _currentTask;
    private bool _nextDest = false;

    private void Awake() {
        this._meshAgent = this.GetComponent<NavMeshAgent>();
        this._layerMask = LayerMask.GetMask("raycastable");
        SetTask(GuardTask.GoNearestDest, -1);
    }

    // Update is called once per frame
    void Update() {
        if (this._taskTimer < 0) {
            DoTask();
        } else {
            this._actualTaskTimer += Time.deltaTime;
            if (this._actualTaskTimer >= this._taskTimer) {
                if (this._currentTask == GuardTask.FollowIntruder) {
                    this._meshAgent.speed = 3.5f;
                    SetTask(GuardTask.GoNearestDest, -1);
                } else if (this._currentTask == GuardTask.RepairCamera) {
                    foreach (GameObject cam in GameObject.FindGameObjectsWithTag("Cam")) {
                        cam.GetComponent<SecurityCam>().RepairCam();
                    }
                    this._meshAgent.speed = 3.5f;
                    SetTask(GuardTask.GoNearestDest, -1);
                }
            } else
                DoTask();
        }
    }

    private void DoTask() {
        if (this._currentTask == GuardTask.FollowIntruder)
            searchPlayer();
        else if (this._currentTask == GuardTask.GotoDestination)
            GotoDestination();
        else if (this._currentTask == GuardTask.GoToCam)
            GotoTask();
    }

    public void SetTask(GuardTask task, float timer) {
        if (this._currentTask == task)
            return;
        this._currentTask = task;
        this._taskTimer = timer;
        this._actualTaskTimer = 0f;
        if (task == GuardTask.FollowIntruder) {
            this._meshAgent.speed = 7f;
            searchPlayer();
        } else if (task == GuardTask.GotoDestination) {
            CalculateNextDestination();
        } else if (task == GuardTask.GoNearestDest) {
            this._currentTask = GuardTask.GotoDestination;
            CalculateNearestPoint();
        } else if (task == GuardTask.GoToCam) {
            GoToTaskPoint();
        } else if (task == GuardTask.RepairCamera) {

        }
    }

    IEnumerator LaunchNextDestination() {
        yield return new WaitForSeconds(4f);
        this._meshAgent.isStopped = false;
        CalculateNextDestination();
        this._nextDest = false;
    }

    private void GoToTaskPoint() {
        if (this.taskTargetPoint != null) {
            this._meshAgent.SetDestination(this.taskTargetPoint.position);
        }
    }

    public void searchPlayer() {
        this._meshAgent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
    }

    public void GotoTask() {
        float dist = Mathf.Sqrt(Mathf.Pow((this.transform.position.x - this.taskTargetPoint.position.x), 2) + Mathf.Pow((this.transform.position.z - this.taskTargetPoint.position.z), 2) + Mathf.Pow((this.transform.position.y - this.taskTargetPoint.position.y), 2));
        if (dist < 2f) {
            this._meshAgent.isStopped = true;
            SetTask(GuardTask.RepairCamera, this._maxRepairCamera);
        }
    }

    public void GotoDestination() {
        float dist = Mathf.Sqrt(Mathf.Pow((this.transform.position.x - this._targetsDest[this._targetIndex].position.x), 2) + Mathf.Pow((this.transform.position.z - this._targetsDest[this._targetIndex].position.z), 2) + Mathf.Pow((this.transform.position.y - this._targetsDest[this._targetIndex].position.y), 2));
        if (dist < 1f) {
            if (this._nextDest == false) {
                this._meshAgent.isStopped = true;
                this._nextDest = true;
                Sequence seq = DOTween.Sequence();
                seq.Append(this.transform.DORotate(Vector3.up * 45f, 1f));
                seq.Append(this.transform.DORotate(Vector3.up, 1f));
                seq.Append(this.transform.DORotate(Vector3.up * -45, 1f));
                seq.Append(this.transform.DORotate(Vector3.up, 1f));
                StartCoroutine(LaunchNextDestination());
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && IsVisibleToGuard(other.transform.position) && this._currentTask != GuardTask.FollowIntruder) {
            SetTask(GuardTask.FollowIntruder, this._maxPursueTime);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player" && IsVisibleToGuard(collision.transform.position)) {
            collision.gameObject.transform.GetComponent<GrabChicken>().DropChicken();
            collision.gameObject.transform.position = new Vector3(-9, 0, -40);
            MenuManager.instance.DeleteOneLife();
            SetTask(GuardTask.GoNearestDest, -1);
        }
    }

    private bool IsVisibleToGuard(Vector3 target) {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, (target - this.transform.position), out hit,  this._maxDetectRange, this._layerMask) 
            && hit.transform.tag == "Player") {
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
