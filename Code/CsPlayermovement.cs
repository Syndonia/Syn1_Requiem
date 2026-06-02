using Sandbox;
using Sandbox.Citizen;

public sealed class CsPlayermovement : Component
{
	//Movement Properties 
	[Property] public float GroundControl { get; set; } = 4.0f;
	[Property] public float AirControl { get; set; } = 0.1f;
	[Property] public float MaxForce { get; set; } = 50f;
	[Property] public float Speed { get; set; } = 160f;
	[Property] public float RunSpeed { get; set; } = 290f;
	[Property] public float CrouchSpeed { get; set; } = 90f;
	[Property] public float JumpForce { get; set; } = 400f;

	//Object References 
	[Property] public GameObject Head { get; set; }
	[Property] public GameObject Body { get; set; }

	//Member Variables 
	public Vector3 WishVelocity = Vector3.Zero;
	public bool IsCrouching = false;
	public bool IsSprinting = false;
	private CharacterController characterController;
	private CitizenAnimationHelper animationHelper;

	protected override void OnAwake()
	{
		characterController = Components.Get<CharacterController>();
		animationHelper = Components.Get<CitizenAnimationHelper>();
	}

	protected override void OnUpdate()
	{
		// Set our Sprinting and Crouching states 
		UpdateCrouch();
		IsSprinting = Input.Down( "Run" );
		if ( Input.Pressed( "Jump" ) ) Jump();

		RotateBody();
		UpdateAnimations();
	}

	protected override void OnFixedUpdate()
	{
		BuildWishVelocity();
		Move();
	}

	void BuildWishVelocity()
	{
		WishVelocity = 0;

		var rot = Head.WorldRotation;

		//Inputs 
		if ( Input.Down( "Forward" ) ) WishVelocity += rot.Forward;
		if ( Input.Down( "Backward" ) ) WishVelocity += rot.Backward;
		if ( Input.Down( "Left" ) ) WishVelocity += rot.Left;
		if ( Input.Down( "Right" ) ) WishVelocity += rot.Right;

		WishVelocity = WishVelocity.WithZ( 0 );
		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( IsCrouching ) WishVelocity *= CrouchSpeed;
		else if ( IsSprinting ) WishVelocity *= RunSpeed;
		else WishVelocity *= Speed;
	}

	void Move()
	{
		//Get Gravuty from our Scene 
		var gravity = Scene.PhysicsWorld.Gravity;

		if ( characterController.IsOnGround )
		{
			//Apply Friction/Acceleration 
			characterController.Velocity = characterController.Velocity.WithZ( 0 );
			characterController.Accelerate( WishVelocity );
			characterController.ApplyFriction( GroundControl );
		}
		else
		{
			//Apply Air Control/Gravity
			characterController.Velocity += gravity * Time.Delta * 0.5f;
			characterController.Accelerate( WishVelocity.ClampLength( MaxForce ) );
			characterController.ApplyFriction( AirControl );
		}

		//Move the CharacterController 
		characterController.Move();

		//Apply the Second Half of Gravity after Movement 
		if ( !characterController.IsOnGround )
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
		}
		else
		{
			characterController.Velocity = characterController.Velocity.WithZ( 0 );
		}

	}
	void RotateBody()
	{
		if ( Body is null ) return;

		var targetAngle = new Angles( 0, Head.WorldRotation.Yaw(), 0 ).ToRotation();
		float rotateDiffrence = Body.WorldRotation.Distance( targetAngle );

		if ( rotateDiffrence > 50f || characterController.Velocity.Length > 10f )
		{
			Body.WorldRotation = Rotation.Lerp( Body.WorldRotation, targetAngle, Time.Delta * 2f );
		}
	}

	void Jump()
	{
		if ( !characterController.IsOnGround ) return;

		characterController.Punch( Vector3.Up * JumpForce );
		animationHelper?.TriggerJump();
	}

	void UpdateCrouch()
	{
		if(characterController is null) return;

		if(Input.Pressed("Duck") && !IsCrouching)
		{
			IsCrouching = true;
			characterController.Height /= 2f; 
		}

		if(Input.Released("Duck") && IsCrouching)
		{
			IsCrouching = false;
			characterController.Height *= 2f; 
		}
	}

	void UpdateAnimations()
	{
		if ( animationHelper is null ) return;

		animationHelper.WithWishVelocity( WishVelocity ); 
		animationHelper.WithVelocity ( characterController.Velocity );
		animationHelper.AimAngle = Head.WorldRotation;
		animationHelper.IsGrounded = characterController.IsOnGround;
		animationHelper.WithLook( Head.WorldRotation.Forward, 1f, 0.75f, 0.5f );
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Run;
		animationHelper.DuckLevel = IsCrouching ? 1f : 0f; 
	}
}
