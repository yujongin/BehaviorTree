using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviorAI : MonoBehaviour
{
    [SerializeField] float detectRange = 10f;
    [SerializeField] float attackRange = 5f;
    [SerializeField] float moveSpeed = 3f;

    Vector3 originPosition;
    BehaviorTreeRunner btRunner;
    Transform detectedPlayer;
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        originPosition = transform.position;

        btRunner = new BehaviorTreeRunner(SettingAttackBT());
    }

    private void Update()
    {
        btRunner.Operate();
    }

    INode SettingAttackBT()
    {
        INode root = new SelectorNode(new List<INode> { 
            new SequenceNode(new List<INode>() { 
                new ActionNode(CheckIsAttacking), new ActionNode(CheckInAttackRange), new ActionNode(DoAttack) }), new SequenceNode(new List<INode>() { 
                    new ActionNode(CheckDetectEnemy), new ActionNode(MoveToDetectedEnemy) }), new ActionNode(ReturnToOrigin) });
        return root;
    }

    NodeState CheckIsAttacking()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("SphereAttack"))
        {
            return NodeState.Running;
        }

        return NodeState.Success;
    }

    NodeState CheckInAttackRange()
    {
        if (detectedPlayer != null)
        {
            if (Vector3.Magnitude(detectedPlayer.position - transform.position) < attackRange)
            {
                return NodeState.Success;
            }
        }

        return NodeState.Failure;
    }

    NodeState DoAttack()
    {
        if (detectedPlayer != null)
        {
            animator.SetTrigger("Attack");
            return NodeState.Success;
        }
        return NodeState.Failure;
    }

    NodeState CheckDetectEnemy()
    {
        Collider[] overlaps = Physics.OverlapSphere(transform.position, detectRange, LayerMask.GetMask("Player"));
        if (overlaps != null && overlaps.Length > 0)
        {
            detectedPlayer = overlaps[0].transform;

            return NodeState.Success;
        }

        detectedPlayer = null;

        return NodeState.Failure;
    }

    NodeState MoveToDetectedEnemy()
    {
        if (detectedPlayer != null)
        {
            if (Vector3.Distance(detectedPlayer.position, transform.position) < attackRange)
            {
                return NodeState.Success;
            }
            transform.position = Vector3.MoveTowards(transform.position, detectedPlayer.position, Time.deltaTime * moveSpeed);
            return NodeState.Running;
        }

        return NodeState.Failure;
    }

    NodeState ReturnToOrigin()
    {
        if (Vector3.Distance(originPosition, transform.position) < float.Epsilon)
        {
            return NodeState.Success;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, originPosition, Time.deltaTime * moveSpeed);
            return NodeState.Running;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

    }
}
