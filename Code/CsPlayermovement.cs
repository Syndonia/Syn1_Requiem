using Sandbox;
using Sandbox.Citizen;

public sealed class CsPlayermovement : Component
{
	//Movement Properties 
	[Property] public float GroundControl { get; set; } = 4.0f;
	[Property] public float AirCOntrol { get; set; } = 0.1f;
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

	}
}
