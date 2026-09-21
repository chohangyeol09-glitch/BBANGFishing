using Member.JJK._02._Scripts.Skill;
using UnityEngine;

namespace Member.JJK._02._Scripts
{
    public class TestEnemyMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private Vector3 _moveDir = Vector3.right;
        private Vector3 _originPos;

        private void Start()
        {
            _originPos = transform.position;
        }

        private void Update()
        {
            transform.position += _moveDir * moveSpeed * EnemyTime.DeltaTime;

            if (transform.position.x > _originPos.x + _moveDir.x * 5)
                _moveDir = Vector3.left;
            else if (transform.position.x < _originPos.x - _moveDir.x * 5)
                _moveDir = Vector3.right;

        }
    }
}