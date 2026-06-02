using Sandbox;
using System.Runtime.Intrinsics.X86;

public sealed class Cscameramovment : Component
{
	//Properties 
	[Property] public CsPlayermovement Player { get; set; }
	[Property] public GameObject Body { get; set; }
	[Property] public GameObject Head { get; set; }
	[Property] public float Distance { get; set; } = 0f; //Change this if we want 3rd person instead of 1st. 1st person is the default perspective 

	//Variables 
	public bool IsFirstPerson => Distance == 0f;
	private CameraComponent Camera;
	private ModelRenderer BodyRenderer; 
	protected override void OnAwake()
	{
		Camera = Components.Get<CameraComponent>();
		BodyRenderer = Body.Components.Get<ModelRenderer>(); 
	}

	protected override void OnUpdate()
	{
		//Rotate the head based on mouse movement
		var eyeAngles = Head.WorldRotation.Angles();
		eyeAngles.pitch += Input.MouseDelta.y * 0.1f;
		eyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
		eyeAngles.roll = 0f;
		eyeAngles.pitch = eyeAngles.pitch.Clamp( -89.9f, 89.9f );
		Head.WorldRotation = eyeAngles.ToRotation();

		//Set the position of the Camera 
		if ( Camera is not null )
		{
			var camPos = Head.WorldPosition; 
			if (!IsFirstPerson )
			{
				//Perform a trace backwards to see where we can safely place the camera 
				var camForward = eyeAngles.ToRotation().Forward;
				var camTrace = Scene.Trace.Ray( camPos, camPos = (camForward * Distance) )
					.WithoutTags( "player", "trigger" )
					.Run(); 

				if(camTrace.Hit)
				{
					camPos = camTrace.HitPosition + camTrace.Normal; 
				}
				else
				{
					camPos = camTrace.EndPosition;
				}

				//Show the body if we're not in first person 
				BodyRenderer.RenderType = ModelRenderer.ShadowRenderType.On; 
			}
			else
			{
				//Hide the body if we're not in the first person 
				BodyRenderer.RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;
			}

			//Set the position of the camera to our calculated position 
			Camera.WorldPosition = camPos; 
			Camera.WorldRotation = eyeAngles.ToRotation(); 
		}
	}
}
