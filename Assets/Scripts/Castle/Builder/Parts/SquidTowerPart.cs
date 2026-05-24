using DG.Tweening;
using UnityEngine;

public class SquidTowerPart : BuildingPart
{
    [SerializeField]
    Animator _squidTowerAnimator;

    [SerializeField]
    GameObject _inkBall;

    [SerializeField]
    Transform _firePosition;

    [SerializeField]
    LayerMask _hittableLayers;

    [SerializeField]
    float _currentAttackTimer;
    [SerializeField]
    float _attackTimer;

    [SerializeField]
    float _inkBallTravelTime = .45f;

    RaycastHit[] _hitTargets;

    public override void InitializePart(GridCell partsCell)
    {
        _attackTimer = BuildingStats.AttackTime;
        //get the spots that the tower should be able to attak in
        base.InitializePart(partsCell);
    }

    void Update()
    {
        _hitTargets = Physics.BoxCastAll(transform.position, Vector3.one * 5 / 2, Vector3.left, Quaternion.identity, GridManager.Instance.CellSize * NumberOfCellsInFrontThatAreHittable, _hittableLayers, QueryTriggerInteraction.Collide);

        if (_hitTargets.Length > 0)
            AttackTimer();
        else
            _currentAttackTimer = 0;
    }

    void AttackTimer()
    {
        if (_currentAttackTimer <= _attackTimer)
            _currentAttackTimer += Time.deltaTime;
        else
        {
            _currentAttackTimer = 0;
            Attack();
        }
    }

    void Attack()
    {
        //Play attack animation here

        Sequence attackSequence = DOTween.Sequence();

        attackSequence.AppendCallback(() =>
        {
            _squidTowerAnimator.SetTrigger("Attack");
        });
        attackSequence.AppendInterval(.45f).AppendCallback(() =>
        {
            GameObject inkBall = Instantiate(_inkBall, _firePosition.position, Quaternion.identity);

            inkBall.transform.DOMove(_hitTargets[0].collider.transform.position, _inkBallTravelTime).OnComplete(() =>
            {
                _hitTargets[0].collider.GetComponent<BaseEnemy>().OnHealthChangeEvent(-CurrentDamage);
                Destroy(inkBall);
            });
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(transform.position, Vector3.left * GridManager.Instance.CellSize * NumberOfCellsInFrontThatAreHittable);
    }
}