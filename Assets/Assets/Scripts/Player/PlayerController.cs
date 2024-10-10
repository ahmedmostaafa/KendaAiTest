using KabreetGames.SceneReferences;
using KendaAi.Agents.InputSystem;
using KendaAi.Agents.Planer;
using KendaAi.BlackboardSystem;
using KendaAi.Inventory.Interface;
using KendaAi.TestProject.PlayerController.States;
using Spine.Unity;
using UnityEngine;

namespace KendaAi.TestProject.PlayerController
{
    public class PlayerController : ValidatedMonoBehaviour, IAgent
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float hitSpeedMultiplier = 0.5f;
        [SerializeField] private LayerMask layerMask;
        public IPlaner Planer { get; set; }
        public Blackboard Blackboard { get; private set; }
        [field: SerializeField, Child] public SkeletonAnimation Animator { get; private set; }

        private StateMachine stateMachine;
        public bool IsAlive => true;

        [SerializeField] private InputReader inputReader;

        private void OnEnable()
        {
            inputReader.EnablePlayerActions();
            Planer = stateMachine = new StateMachine(inputReader, this);
            Blackboard = new Blackboard();
            stateMachine.SetState<MoveState>();
        }

        private void OnDisable()
        {
            inputReader.DisablePlayerActions();
        }

        private void Update()
        {
            Planer.Update();
        }

        private void FixedUpdate()
        {
            Planer.FixedUpdate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out IInteractable collectable) && collectable.CanInteract(this))
            {
                collectable.Interact(this);
            }
        }

        public void Move(float input)
        {
            var deltaPosition = (speed * Time.deltaTime);
            var moveDirection = transform.forward;
            transform.position += (moveDirection + input * transform.right).normalized * deltaPosition;
        }

        public void MoveHit()
        {
            var deltaPosition = speed * Time.deltaTime *hitSpeedMultiplier ;
            var moveDirection = transform.forward;
            transform.position += moveDirection * deltaPosition;
        }

        public void ChangePlane<T>() where T : IState, new()
        {
            stateMachine.ChangeState<T>();
        }

        private void OnDrawGizmos()
        {
            Planer?.DrawGizmos();
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward);
        }
        
    }
}