using KabreetGames.PathSystem;
using KabreetGames.SceneReferences;
using KabreetGames.TimeSystem;
using Spine.Unity;
using UnityEngine;

namespace KendaAi.Scripts.Enemy
{
    public class Crab : ValidatedMonoBehaviour
    {
        [SerializeField, Child] private SkeletonAnimation animator;
        [SerializeField, Child] private Path path;
        [SerializeField] private float speed = 1f;
        [SerializeField] private float idleTime = 0.1f;
        private CountdownTimer idleTimer;
        private Vector3 targetPosition;

        private void OnEnable()
        {
            path.UsePath(this);
            GetNextPoint();
            idleTimer = new CountdownTimer(idleTime);
            idleTimer.OnTimerStart += SetIdleState;
            idleTimer.OnTimerStop += SetMoveState;
        }

        private void OnDisable()
        {
            path.UnUsePath(this);
            idleTimer.OnTimerStart -= SetIdleState;
            idleTimer.OnTimerStop -= SetMoveState;
        }

        private void SetMoveState()
        {
        }

        private void SetIdleState()
        {
        }

        private void Update()
        {
            if (path == null || !animator.gameObject.activeSelf) return;
            if (!idleTimer.IsRunning)
            {
                HandleMove();
            }
        }

        private void HandleMove()
        {
            var body = animator.transform;
            var dir = new Vector3(targetPosition.x, body.position.y, targetPosition.z) - body.position;
            dir = dir.normalized;

            var nextPos = body.position + dir * (speed * Time.deltaTime);
            body.position = nextPos;

            var desiredForward = Vector3.Cross(Vector3.up, dir).normalized;
            var targetRotation = Quaternion.LookRotation(desiredForward, Vector3.up);
            var sqrMagnitude = (body.position - targetPosition).sqrMagnitude;
            body.rotation = targetRotation;
            if (!(sqrMagnitude < 0.01f)) return;
            GetNextPoint();
            idleTimer.Reset(idleTime);
            idleTimer.Start();
        }

        private void GetNextPoint()
        {
            var body = animator.transform;
            if (path == null) return;
            var nextPoint = path.GetNextPoint(this);
            targetPosition = new Vector3(nextPoint.x, body.position.y, nextPoint.z);
        }
    }
}