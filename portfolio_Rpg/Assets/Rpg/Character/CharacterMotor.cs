using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Character
{
    public class CharacterMotor : Character
    {

        public UnityEvent OnStaminaEnd;
        public UnityEvent OnDead;
        public UnityEvent OnTakeDamage;

        public enum LocomotionType
        {
            FreeWithStrafe,
            OnlyStrafe,
            OnlyFree
        }
        public LocomotionType locomotionType = LocomotionType.FreeWithStrafe;
        protected bool freeLocomotionConditions
        {
            get
            {
                if (locomotionType.Equals(LocomotionType.OnlyStrafe)) isStrafing = true;
                return !isStrafing && !landHigh && !locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.OnlyFree);
            }
        }
        public bool customAction;
        public bool AttackAction;

        [HideInInspector]
        public bool actions
        {
            get
            {
                return landHigh || customAction || AttackAction;
            }
        }

        [HideInInspector]
        public PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;
        [HideInInspector]
        public AnimatorStateInfo baseLayerInfo, fullBodyInfo;
        [SerializeField]
        protected float groundMinDistance = 0.2f;
        [SerializeField]
        protected float groundMaxDistance = 0.5f;

        public LayerMask autoCrouchLayer = 1 << 0;
        public float headDetect = 0.95f;

        [HideInInspector]
        public Vector2 input;
        [HideInInspector]
        public new Rigidbody rigidbody;
        [HideInInspector]
        public float speed, direction, verticalVelocity;

        // movement bools
        [HideInInspector]
        public bool
            isGrounded,
            isCrouching,
            inCrouchArea,
            isStrafing,
            isSprinting,
            isSliding,
            stopMove,
            autoCrouch;

        // action bools
        [HideInInspector]
        public bool
            isJumping,
            landHigh;

        [Header("--- Movement Speed ---")]
        public bool useRootMotion = true;
        public bool freeWalkByDefault = false;
        public bool strafeWalkByDefault = true;
        public float freeWalkSpeed = 1f;
        public float freeRunningSpeed = 3f;
        public float freeSprintSpeed = 3f;
        public float freeCrouchSpeed = 1f;
        public float strafeWalkSpeed = 3f;
        public float strafeRunningSpeed = 3f;
        public float strafeSprintSpeed = 3f;
        public float strafeCrouchSpeed = 3f;

        [SerializeField]
        public float freeRotationSpeed = 10f;
        [SerializeField]
        public float meleeAttackRotationSpeed = 50f;
        public RaycastHit groundHit;
        protected float groundDistance;

        [SerializeField]
        protected float slopeLimit = 45f;
        [SerializeField]
        protected float extraGravity = -10f;
        [HideInInspector]
        public CapsuleCollider capsuleCollider;
        [HideInInspector]
        public Vector3 colliderCenter;
        [HideInInspector]
        public float colliderRadius, colliderHeight;

        [HideInInspector]
        public float velocity;
        public float stepSmooth = 4f;
        public LayerMask groundLayer = 1 << 0;
        public float stepOffsetEnd = 0.45f;
        public float stepOffsetStart = 0.05f;

        [HideInInspector]
        public bool lockMovement;

        [HideInInspector]
        public float jumpCounter;
        public float jumpTimer = 0.3f;
        public float jumpHeight = 4f;
        public bool jumpAirControl = true;
        public float jumpForward = 3f;

        #region Direction Variables
        [HideInInspector]
        public Vector3 attackDirection;

        [HideInInspector]
        public Vector3 targetDirection;
        protected Quaternion targetRotation;
        [HideInInspector]
        public float strafeInput;
        [HideInInspector]
        public Quaternion freeRotation;
        [HideInInspector]
        public bool keepDirection;
        [HideInInspector]
        public Vector2 oldInput;
        #endregion

        [Tooltip("Speed of the rotation while strafe movement")]
        public float strafeRotationSpeed = 10f;

        [SerializeField]
        public bool rotateByWorld = false;

        protected bool jumpFwdCondition
        {
            get
            {
                Vector3 p1 = transform.position + capsuleCollider.center + Vector3.up * -capsuleCollider.height * 0.5F;
                Vector3 p2 = p1 + Vector3.up * capsuleCollider.height;
                return Physics.CapsuleCastAll(p1, p2, capsuleCollider.radius * 0.5f, transform.forward, 0.6f, groundLayer).Length == 0;
            }
        }
        public float stopMoveHeight = 0.65f;

        public override void Init()
        {
            base.Init();
            animator = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody>();

            capsuleCollider = GetComponent<CapsuleCollider>();
            colliderCenter = GetComponent<CapsuleCollider>().center;
            colliderRadius = GetComponent<CapsuleCollider>().radius;
            colliderHeight = GetComponent<CapsuleCollider>().height;

            animator.applyRootMotion = true;
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            frictionPhysics = new PhysicMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

            // prevents the collider from slipping on ramps
            maxFrictionPhysics = new PhysicMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

            // air physics 
            slippyPhysics = new PhysicMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

            currentHealth = maxHealth;

        }
        public virtual void UpdateMotor()
        {
            CheckHealth();
            CheckGround();
            CheckAttackDirection();

            ControlCapsuleHeight();
            ControlJumpBehaviour();
            ControlLocomotion();
        }

        void ControlLocomotion()
        {
            if (lockMovement || currentHealth <= 0)
                return;

            if (freeLocomotionConditions)
                FreeMovement();
            else
                StrafeMovement();
        }

        void CheckHealth()
        {
            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
                OnDead.Invoke();
                if (onDead != null)
                    onDead.Invoke(gameObject);
            }
        }

        void ControlCapsuleHeight()
        {
            if (isCrouching || landHigh)
            {
                capsuleCollider.center = colliderCenter / 2f;
                capsuleCollider.height = colliderHeight / 2f;
            }
            else
            {
                // back to the original values
                capsuleCollider.center = colliderCenter;
                capsuleCollider.radius = colliderRadius;
                capsuleCollider.height = colliderHeight;
            }
        }

        public virtual float GroundAngle()
        {
            var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            return groundAngle;
        }

        void CheckGround()
        {
            CheckGroundDistance();

            if (isDead || customAction)
            {
                isGrounded = true;
                return;
            }

            capsuleCollider.material = (isGrounded && GroundAngle() <= slopeLimit + 1) ? frictionPhysics : slippyPhysics;

            if (isGrounded && input == Vector2.zero)
                capsuleCollider.material = maxFrictionPhysics;
            else if (isGrounded && input != Vector2.zero)
                capsuleCollider.material = frictionPhysics;
            else
                capsuleCollider.material = slippyPhysics;

            var magVel = (float)System.Math.Round(new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).magnitude, 2);
            magVel = Mathf.Clamp(magVel, 0, 1);

            var groundCheckDistance = groundMinDistance;
            if (magVel > 0.25f)
                groundCheckDistance = groundMaxDistance;

            var onStep = StepOffset();

            if (groundDistance <= 0.05f)
            {
                rigidbody.velocity = Vector3.zero;
                isGrounded = true;
                Sliding();
            }
            else
            {
                if (groundDistance >= groundCheckDistance)
                {
                    isGrounded = false;
                    verticalVelocity = rigidbody.velocity.y;
                    if (!onStep && !isJumping)
                        rigidbody.AddForce(transform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
                }
                else if (!onStep && !isJumping)
                    rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
            }
        }

        void CheckGroundDistance()
        {
            if (isDead)
                return;

            if (capsuleCollider != null)
            {
                float radius = capsuleCollider.radius * 0.9f;
                var dist = 10f;

                Vector3 pos = transform.position + Vector3.up * (capsuleCollider.radius);

                Ray ray1 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);

                Ray ray2 = new Ray(pos, -Vector3.up);

                if (Physics.Raycast(ray1, out groundHit, colliderHeight / 2f + 2, groundLayer))
                    dist = (transform.position.y) - groundHit.point.y;

                if (Physics.SphereCast(ray2, radius, out groundHit, capsuleCollider.radius + 2f, groundLayer))
                {
                    if (dist > (groundHit.distance - capsuleCollider.radius * 0.1f))
                        dist = (groundHit.distance - capsuleCollider.radius * 0.05f);
                }
                groundDistance = (float)System.Math.Round(dist, 2);
            }
        }

        protected void ControlJumpBehaviour()
        {
            if (!isJumping)
                return;

            jumpCounter -= Time.deltaTime;
            if (jumpCounter <= 0)
            {
                jumpCounter = 0;
                isJumping = false;
            }

            var vel = rigidbody.velocity;
            vel.y = jumpHeight;
            rigidbody.velocity = vel;
        }

        public void AirControl()
        {
            if (isGrounded)
                return;
            if (!jumpFwdCondition)
                return;

            var velY = transform.forward * jumpForward * speed;
            velY.y = rigidbody.velocity.y;
            var velX = transform.right * jumpForward * direction;
            velX.x = rigidbody.velocity.x;
            EnableGravityAndCollision(0f);
            if (jumpAirControl)
            {
                if (isStrafing)
                {
                    rigidbody.velocity = new Vector3(velX.x, velY.y, rigidbody.velocity.z);
                    var vel = transform.forward * (jumpForward * speed) + transform.right * (jumpForward * direction);
                    rigidbody.velocity = new Vector3(vel.x, rigidbody.velocity.y, vel.z);
                }
                else
                {
                    var vel = transform.forward * (jumpForward * speed);
                    rigidbody.velocity = new Vector3(vel.x, rigidbody.velocity.y, vel.z);
                }
            }
            else
            {
                var vel = transform.forward * (jumpForward);
                rigidbody.velocity = new Vector3(vel.x, rigidbody.velocity.y, vel.z);
            }
        }

        public void EnableGravityAndCollision(float normalizedTime)
        {
            if (baseLayerInfo.normalizedTime >= normalizedTime)
            {
                capsuleCollider.isTrigger = false;
                rigidbody.useGravity = true;
            }
        }

        bool StepOffset()
        {
            if (input.sqrMagnitude < 0.1 || !isGrounded)
            {
                return false;
            }
            var _hit = new RaycastHit();
            var _movementDirection = isStrafing && input.magnitude > 0 ? (transform.right * input.x + transform.forward * input.y).normalized : transform.forward;
            Ray rayStep = new Ray((transform.position + new Vector3(0, stepOffsetEnd, 0) + _movementDirection * ((capsuleCollider).radius + 0.05f)), Vector3.down);

            if (Physics.Raycast(rayStep, out _hit, stepOffsetEnd - stepOffsetStart, groundLayer))
            {
                if (_hit.point.y >= (transform.position.y) && _hit.point.y <= (transform.position.y + stepOffsetEnd))
                {
                    var _speed = isStrafing ? Mathf.Clamp(input.magnitude, 0, 1) : speed;
                    var velocityDirection = isStrafing ? (_hit.point - transform.position) : (_hit.point - transform.position).normalized;
                    rigidbody.velocity = velocityDirection * stepSmooth * (_speed * (velocity > 1 ? velocity : 1));
                    return true;
                }
            }
            return false;
        }

        public void ControlSpeed(float velocity)
        {
            if (Time.deltaTime == 0)
                return;

            if (useRootMotion && !actions && !customAction)
            {
                this.velocity = velocity;
                var deltaPosition = new Vector3(animator.deltaPosition.x, transform.position.y, animator.deltaPosition.z);
                Vector3 v = (deltaPosition * (velocity > 0 ? velocity : 1f)) / Time.deltaTime;
                v.y = rigidbody.velocity.y;
                rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, v, 20f * Time.deltaTime);
            }
            else if (actions || customAction || AttackAction)
            {
                this.velocity = velocity;
                Vector3 v = Vector3.zero;
                v.y = rigidbody.velocity.y;
                rigidbody.velocity = v;
                transform.position = animator.rootPosition;
            }
            else
            {
                var velY = transform.forward * velocity * speed;
                velY.y = rigidbody.velocity.y;
                var velX = transform.right * velocity * direction;
                velX.x = rigidbody.velocity.x;

                if (isStrafing)
                {
                    Vector3 v = (transform.TransformDirection(new Vector3(input.x, 0, input.y)) * (velocity > 0 ? velocity : 1f));
                    v.y = rigidbody.velocity.y;
                    rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, v, 20f * Time.deltaTime);
                }
                else
                {
                    rigidbody.velocity = velY;
                    rigidbody.AddForce(transform.forward * (velocity * speed) * Time.deltaTime, ForceMode.VelocityChange);
                }
            }
        }

        public virtual void CheckAttackDirection()
        {
            if (AttackAction)
            {
                Vector3 lookDirection = attackDirection.normalized;
                freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
                var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
                var eulerY = transform.eulerAngles.y;
                if (isGrounded || (!isGrounded && jumpAirControl))
                {
                    if (diferenceRotation < 0 || diferenceRotation > 0)
                        eulerY = freeRotation.eulerAngles.y;
                    var euler = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(euler), meleeAttackRotationSpeed * Time.deltaTime);
                }
            }
        }

        public virtual void FreeMovement()
        {
            speed = Mathf.Abs(input.x) + Mathf.Abs(input.y);
            if (freeWalkByDefault)
                speed = Mathf.Clamp(speed, 0, 0.5f);
            else
                speed = Mathf.Clamp(speed, 0, 1f);

            if (stopMove)
                speed = 0f;

            animator.SetFloat("InputMagnitude", speed, .2f, Time.deltaTime);

            var conditions = (!actions);

            if (input != Vector2.zero && targetDirection.magnitude > 0.1f && conditions)
            {
                Vector3 lookDirection = targetDirection.normalized;
                freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
                var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
                var eulerY = transform.eulerAngles.y;

                if (isGrounded || (!isGrounded && jumpAirControl))
                {
                    if (diferenceRotation < 0 || diferenceRotation > 0)
                        eulerY = freeRotation.eulerAngles.y;
                    var euler = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(euler), freeRotationSpeed * Time.deltaTime);
                }

                if (!keepDirection)
                    oldInput = input;

                if (Vector2.Distance(oldInput, input) > 0.9f && keepDirection)
                    keepDirection = false;
            }
        }

        public virtual void StrafeMovement()
        {
            if (strafeWalkByDefault)
                StrafeLimitSpeed(0.5f);
            else
                StrafeLimitSpeed(1f);

            if (isSprinting)
                strafeInput += 0.5f;
            if (stopMove)
                strafeInput = 0f;
            animator.SetFloat("InputMagnitude", strafeInput, .2f, Time.deltaTime);
        }

        void StrafeLimitSpeed(float value)
        {
            var _speed = Mathf.Clamp(input.y, -1f, 1f);
            var _direction = Mathf.Clamp(input.x, -1f, 1f);
            speed = _speed;
            direction = _direction;
            var newInput = new Vector2(speed, direction);
            strafeInput = Mathf.Clamp(newInput.magnitude, 0, value);
        }

        void Sliding()
        {
            var onStep = StepOffset();
            var groundAngleTwo = 0f;
            RaycastHit hitinfo;
            Ray ray = new Ray(transform.position, -transform.up);

            if (Physics.Raycast(ray, out hitinfo, 1f, groundLayer))
                groundAngleTwo = Vector3.Angle(Vector3.up, hitinfo.normal);

            if (GroundAngle() > slopeLimit + 1f && GroundAngle() <= 85 && groundAngleTwo > slopeLimit + 1f && groundAngleTwo <= 85 && groundDistance <= 0.05f && !onStep)
            {
                isSliding = true;
                isGrounded = false;
                var slideVelocity = (GroundAngle() - slopeLimit) * 2f;
                slideVelocity = Mathf.Clamp(slideVelocity, 0, 10);
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, -slideVelocity, rigidbody.velocity.z);
            }
            else
            {
                isSliding = false;
                isGrounded = true;
            }
        }

        #region Health & Stamina

        public override void TakeDamage(Damage damage, bool hitReaction = true)
        {
            if (currentHealth <= 0)
                return;

            currentHealth -= damage.damageValue;
            currentHealthRecoveryDelay = healthRecoveryDelay;

            var hitReactionConditions = !actions || !customAction;
            if (hitReactionConditions && currentHealth > 0)
            {
                animator.SetInteger("HitDirection", (int)transform.HitAngle(damage.sender.position));
                if (hitReaction)
                {
                    animator.SetInteger("ReactionID", damage.reaction_id);
                    animator.SetTrigger("TriggerReaction");
                }
                else
                {
                    animator.SetInteger("RecoilID", damage.recoil_id);
                    animator.SetTrigger("TriggerRecoil");
                }
            }

            OnTakeDamage.Invoke();
            onReceiveDamage.Invoke(damage);

            if (damage.activeRagdoll)
                onActiveRagdoll.Invoke();
        }
        #endregion

        public void DeathBehaviour()
        {
            lockMovement = true;

            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            if (deathBy == DeathBy.Animation || deathBy == DeathBy.AnimationWithRagdoll)
                animator.SetBool("isDead", true);
        }

        protected void RemoveComponents()
        {
            if (capsuleCollider != null)
                Destroy(capsuleCollider);
            if (rigidbody != null)
                Destroy(rigidbody);
            if (animator != null)
                Destroy(animator);
            var comps = GetComponents<MonoBehaviour>();
            for (int i = 0; i < comps.Length; i++)
            {
                Destroy(comps[i]);
            }
        }

        protected void StopMove()
        {
            if (input.sqrMagnitude < 0.1 || !isGrounded) return;

            RaycastHit hitinfo;
            Ray ray = new Ray(transform.position + new Vector3(0, stopMoveHeight, 0), targetDirection.normalized);

            if (Physics.Raycast(ray, out hitinfo, 1f, groundLayer))
            {
                var hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);
                if (hitAngle >= slopeLimit + 1f && hitAngle <= 85)
                    stopMove = true;
            }
            else
                stopMove = false;
        }

        #region Camera Methods

        public virtual void RotateToTarget(Transform target)
        {
            if (target)
            {
                Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
                var newPos = new Vector3(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
                targetRotation = Quaternion.Euler(newPos);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newPos), strafeRotationSpeed * Time.deltaTime);
            }
        }
        /// <summary>
        /// Update the targetDirection variable using referenceTransform or just input.Rotate by word  the referenceDirection
        /// </summary>
        /// <param name="referenceTransform"></param>
        public virtual void UpdateTargetDirection(Transform referenceTransform = null)
        {
            if (referenceTransform && !rotateByWorld)
            {
                var forward = keepDirection ? referenceTransform.forward : referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0;

                forward = keepDirection ? forward : referenceTransform.TransformDirection(Vector3.forward);
                forward.y = 0;

                var right = keepDirection ? referenceTransform.right : referenceTransform.TransformDirection(Vector3.right);
                targetDirection = input.x * right + input.y * forward;
            }
            else
            {
                targetDirection = keepDirection ? targetDirection : new Vector3(input.x, 0, input.y);
            }
        }
        #endregion
    }
}

