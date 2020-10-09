

using UnityEngine;
using UnityEngine.AI;

public class Chicken : MonoBehaviour
{
    [System.Serializable]
    public enum State
    {
        Idle,
        RandomLook,
        Eat,
        Walk,
        Took,
        Launch
    };
    [SerializeField]
    NavMeshAgent _agent;
    public NavMeshAgent agent { get => this._agent; private set => this._agent = value; }
    [SerializeField]
    Rigidbody _rigidbody;
    public Rigidbody rigidbody { get => this._rigidbody; private set => this._rigidbody = value; }
    [SerializeField]
    Animator _animator;
    public Animator animator { get => this._animator; private set => this._animator = value; }
    bool _took = false;
    public bool took { get => this._took; set => this.setTook(value); }
    [SerializeField]
    State _currentState = State.Idle;
    State currentState { get => this._currentState; set => setState(value); }
    float beforeChange = 2.0f;
    [SerializeField]
    public float walkRadius = 5.0f;
    float generateWaitTime() => Random.value * 3.0f + 2.0f;

    void setState(State state) {
        this._currentState = state;
        this.animator.Play(state.ToString(), 0, 0.0f);
        if (state == State.Idle) {
            this.beforeChange = this.generateWaitTime();
        } else if (state == State.RandomLook || state == State.Eat) {
            AnimatorTransitionInfo info = this.animator.GetAnimatorTransitionInfo(0);
            this.beforeChange = this.animator.GetCurrentAnimatorStateInfo(0).length + this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        } else if (state == State.Walk) {
            Vector3 randomDirection = Random.insideUnitSphere * this.walkRadius + transform.position;
            NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, walkRadius, 1);
            this.agent.SetDestination(hit.position);
        }
    }

    void setTook(bool value)
    {
        if (this._took == value)
            return;
        if (value) {
            this.currentState = State.Took;
            if (this.agent.isOnNavMesh) {
                this.agent.ResetPath();
            }
            this.agent.enabled = false;
            this.rigidbody.isKinematic = true;
        }
        else {
            this.currentState = State.Launch;
            this.rigidbody.isKinematic = false;
        }
        this._took = value;
    }

    private void Awake()
    {
        Debug.Assert(this.agent != null);
        Debug.Assert(this.rigidbody != null);
        Debug.Assert(this.animator != null);
    }

    private void Update()
    {
        if (this.currentState == State.Idle) {
            this.beforeChange -= Time.deltaTime;
            if (this.beforeChange <= 0.0f) {
                this.currentState = State.RandomLook;
            }
        } else if (this.currentState == State.RandomLook) {
            this.beforeChange -= Time.deltaTime;
            if (this.beforeChange <= 0.0f)
            {
                if (this.agent.isOnNavMesh) {
                    this.currentState = Random.value > 0.5f ? State.Walk : State.Eat;
                } else {
                    this.currentState = State.Eat;
                }
            }
        } else if (this.currentState == State.Eat) {
            this.beforeChange -= Time.deltaTime;
            if (this.beforeChange <= 0.0f)
            {
                    this.currentState = State.Idle;
            }
        } else if (this.currentState == State.Walk) {
            if (this.agent.remainingDistance <= this.agent.stoppingDistance) {
                this.currentState = State.Idle;
            }
        } else if (this.currentState == State.Launch) {
            if (Physics.Raycast(this.transform.position + Vector3.up * 0.0001f, Vector3.down, 0.3f, LayerMask.GetMask("Default"))) {
                this.agent.enabled = true;
                this.currentState = State.Idle;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.tag);
        if (collision.transform.tag == "Grass")
            Destroy(this.gameObject);
    }
}
