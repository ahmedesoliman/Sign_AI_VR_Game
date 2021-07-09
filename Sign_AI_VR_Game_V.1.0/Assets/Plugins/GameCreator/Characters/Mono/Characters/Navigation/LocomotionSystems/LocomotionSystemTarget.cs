namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.AI;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    public class LocomotionSystemTarget : ILocomotionSystem
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        private bool move = false;
        private bool usingNavmesh;
        private NavMeshPath path;

        private Vector3 targetPosition;
        private TargetRotation targetRotation;

        private float stopThreshold = STOP_THRESHOLD;
        private UnityAction onFinishCallback;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override CharacterLocomotion.LOCOMOTION_SYSTEM Update()
        {
            base.Update();
            if (!this.move)
            {
                if (this.usingNavmesh)
                {
                    this.characterLocomotion.navmeshAgent.enabled = false;
                    this.usingNavmesh = false;
                }

                Vector3 defaultDirection = Vector3.up * this.characterLocomotion.verticalSpeed;
                this.characterLocomotion.characterController.Move(defaultDirection * Time.deltaTime);

                Transform characterTransform = this.characterLocomotion.character.transform;
                Vector3 forward = characterTransform.TransformDirection(Vector3.forward);

                Quaternion rotation = this.UpdateRotation(forward);
                this.characterLocomotion.character.transform.rotation = rotation;

                return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;

                /*
                if (!this.usingNavmesh)
                {
					Vector3 defaultDirection = Vector3.up * this.characterLocomotion.verticalSpeed;
                    this.characterLocomotion.characterController.Move(defaultDirection * Time.deltaTime);
                    return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
                }

                this.characterLocomotion.navmeshAgent.enabled = true;
                return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;
                */
            }

            if (this.usingNavmesh)
            {
                NavMeshAgent agent = this.characterLocomotion.navmeshAgent;
                agent.enabled = true;

                CharacterController controller = this.characterLocomotion.characterController;
                if (agent.pathPending) return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;

                if (!agent.hasPath || agent.pathStatus != NavMeshPathStatus.PathComplete)
                {
                    float distance = Mathf.Min(
                        Vector3.Distance(agent.pathEndPosition, agent.transform.position),
                        agent.remainingDistance
                    );

                    if (!agent.hasPath && distance < STOP_THRESHOLD)
                    {
                        this.Stopping();
                    }

                    return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;
                }


                float remainingDistance = agent.remainingDistance;
                bool isGrounded = agent.isOnOffMeshLink;
                agent.speed = this.CalculateSpeed(controller.transform.forward, isGrounded);
                agent.angularSpeed = this.characterLocomotion.angularSpeed;

                agent.isStopped = false;
                agent.updateRotation = true;

                if (remainingDistance <= this.stopThreshold)
                {
                    agent.updateRotation = true;
                    this.Stopping();
                }
                else if (remainingDistance <= this.stopThreshold + SLOW_THRESHOLD)
                {
                    this.Slowing(remainingDistance);
                }
                else
                {
                    this.Moving();
                }

                this.UpdateNavmeshAnimationConstraints();
                return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;
            }
            else
            {
                if (this.characterLocomotion.navmeshAgent != null &&
                    this.characterLocomotion.navmeshAgent.enabled)
                {
                    this.characterLocomotion.navmeshAgent.enabled = false;
                }

                CharacterController controller = this.characterLocomotion.characterController;
                Vector3 targetPos = Vector3.Scale(this.targetPosition, HORIZONTAL_PLANE);
                targetPos += Vector3.up * controller.transform.position.y;
                Vector3 targetDirection = (targetPos - controller.transform.position).normalized;

                float speed = this.CalculateSpeed(targetDirection, controller.isGrounded);
                Quaternion targetRot = this.UpdateRotation(targetDirection);

                this.UpdateAnimationConstraints(ref targetDirection, ref targetRot);

                targetDirection = Vector3.Scale(targetDirection, HORIZONTAL_PLANE) * speed;
                targetDirection += Vector3.up * this.characterLocomotion.verticalSpeed;

                controller.Move(targetDirection * Time.deltaTime);
                controller.transform.rotation = targetRot;

                float remainingDistance = (Vector3.Distance(
                    Vector3.Scale(controller.transform.position, HORIZONTAL_PLANE),
                    Vector3.Scale(this.targetPosition, HORIZONTAL_PLANE)
                ));

                if (remainingDistance <= this.stopThreshold)
                {
                    this.Stopping();
                }
                else if (remainingDistance <= this.stopThreshold + SLOW_THRESHOLD)
                {
                    this.Slowing(remainingDistance);
                }

                return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
            }
        }

		public override void OnDestroy() {}

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Stopping()
        {
            if (this.characterLocomotion.navmeshAgent != null &&
                this.characterLocomotion.navmeshAgent.enabled)
            {
                this.characterLocomotion.navmeshAgent.isStopped = true;
            }

            this.FinishMovement();
            this.move = false;

            if (this.targetRotation.hasRotation &&
                this.characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.MovementDirection)
            {
                this.characterLocomotion.character.transform.rotation = this.targetRotation.rotation;
            }
        }

        private void Slowing(float distanceToDestination)
        {
            float tDistance = 1f - (distanceToDestination / (this.stopThreshold + SLOW_THRESHOLD));
            
            Transform characterTransform = this.characterLocomotion.character.transform;
            Quaternion desiredRotation = this.UpdateRotation(characterTransform.TransformDirection(Vector3.forward));

            if (this.targetRotation.hasRotation &&
                this.characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.MovementDirection)
            {
                desiredRotation = this.targetRotation.rotation;
            }

            characterTransform.rotation = Quaternion.Lerp(
                characterTransform.rotation,
                desiredRotation,
                tDistance
            );
        }

        private void Moving()
        {
            Quaternion desiredRotation = this.UpdateRotation(
                this.characterLocomotion.navmeshAgent.desiredVelocity
            );

            this.characterLocomotion.character.transform.rotation = desiredRotation;
        }

        private void UpdateNavmeshAnimationConstraints()
        {
            NavMeshAgent agent = this.characterLocomotion.navmeshAgent;
            if (this.characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_MOVEMENT)
            {
                if (agent.velocity == Vector3.zero)
                {
                    agent.Move(agent.transform.forward * agent.speed * Time.deltaTime);
                }
            }

            if (this.characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_POSITION)
            {
                agent.isStopped = true;
            }
        }

        private void FinishMovement()
        {
            if (this.onFinishCallback != null)
            {
                this.onFinishCallback.Invoke();
                this.onFinishCallback = null;
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        private RaycastHit[] hitBuffer = new RaycastHit[1];

        public void SetTarget(Ray ray, LayerMask layerMask, TargetRotation rotation,
            float stopThreshold, UnityAction callback = null)
        {
            QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;
            int hitCount = Physics.RaycastNonAlloc(
                ray, this.hitBuffer, Mathf.Infinity,
                layerMask, queryTrigger
            );

            if (hitCount > 0)
            {
				this.SetTarget(this.hitBuffer[0].point, rotation, stopThreshold, callback);
            }
        }

        public void SetTarget(Vector3 position, TargetRotation rotation,
            float stopThreshold, UnityAction callback = null)
        {
            this.move = true;
            this.usingNavmesh = false;

            this.stopThreshold = Mathf.Max(stopThreshold, STOP_THRESHOLD);
            this.onFinishCallback = callback;

            if (this.characterLocomotion.canUseNavigationMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas)) position = hit.position;

                this.path = new NavMeshPath();
                bool pathFound = NavMesh.CalculatePath(
                    this.characterLocomotion.characterController.transform.position,
                    position,
                    NavMesh.AllAreas,
                    this.path
                );

                if (pathFound)
                {
                    Debug.DrawLine(position, position + Vector3.up, Color.green, 0.1f);

                    this.usingNavmesh = true;
                    this.characterLocomotion.navmeshAgent.enabled = true;

                    this.characterLocomotion.navmeshAgent.updatePosition = true;
                    this.characterLocomotion.navmeshAgent.updateUpAxis = true;

                    this.characterLocomotion.navmeshAgent.isStopped = false;
                    this.characterLocomotion.navmeshAgent.SetPath(this.path);
                }
            }

            this.targetPosition = position;
            this.targetRotation = rotation ?? new TargetRotation();
        }

        public void Stop(TargetRotation rotation = null, UnityAction callback = null)
        {
            this.SetTarget(
                this.characterLocomotion.characterController.transform.position,
                rotation,
                0f,
                callback
            );
        }
    }
}
