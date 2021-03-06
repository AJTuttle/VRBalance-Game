using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace unityutilities
{
	/// <summary>
	/// Adds several movement techniques while maintaining compatibility with many rig setups.
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class Movement : MonoBehaviour
	{
		public Rig rig;

		[Header("Features")] public bool grabWalls;
		public bool grabAir = true;
		public bool stickBoostBrake = true;
		public bool handBoosters = true;
		public bool yaw = true;
		public bool pitch;
		public bool roll;
		public bool slidingMovement;
		public bool teleportingMovement;

		[Header("Rotation")] 
		[Tooltip("Not used if pitch is enabled.")]
		public Side turnInput = Side.Either;
		public bool continuousRotation;
		public float continuousRotationSpeed = 100f;
		public float snapRotationAmount = 30f;
		public float turnNullZone = .1f;
		
		
		[Header("Tuning")]
		public float mainBoosterMaxSpeed = 5f;
		public float mainBrakeDrag = 2f;
		public float maxHandBoosterSpeed = 4f;
		public float mainBoosterForce = 1f;
		public float handBoosterAccel = 1f;
		public float mainBoosterCost = 1f;
		private float mainBoosterBudget = 1f;
		private float normalDrag;
		public float slidingAccel = 1f;
		public float slidingSpeed = 3f;
		public float minVel = .1f;
		

		public delegate void GrabEvent(Transform obj, Side side);

		public event GrabEvent OnGrab;
		public event GrabEvent OnRelease;

		[HideInInspector]
		public Transform leftHandGrabbedObj;
		[HideInInspector]
		public Transform rightHandGrabbedObj;
		private Transform lastleftHandGrabbedObj;
		private Transform lastrightHandGrabbedObj;
		private GameObject leftHandGrabPos;
		private GameObject rightHandGrabPos;

		private readonly Vector3[] lastVels = new Vector3[5];
		private int lastVelsIndex;

		private CopyTransform cpt;

		private Side grabbingSide = Side.None;
		
		
		// teleporting vars
		private LineRenderer lineRenderer;
		[HideInInspector]
		public Side currentTeleportingSide = Side.None;
		private RaycastHit teleportHit;
		
		/// <summary>
		/// The current chosen spot to teleport to
		/// </summary>
		[Serializable]
		public class Teleporter
		{
			// inspector values
			public bool rotateOnTeleport;
			public float maxTeleportableSlope = 20f;
			public float lineRendererWidth = .01f;
			public float teleportCurve = .01f;
			public float smoothTeleportTime = .1f;
			public GameObject teleportMarkerOverride;
			public Material lineRendererMaterialOverride;
			
			[HideInInspector]
			public GameObject teleportMarkerInstance;
			
			
			public Teleporter()
			{
				Active = false;
			}
			
			public Teleporter(Vector3 pos, Vector3 dir)
			{
				Pos = pos;
				Dir = dir;
				Active = true;
			}

			private bool active;
			
			private Vector3 pos;
			private Vector3 dir;

			public bool Active
			{
				get { return active; }
				set
				{
					if (active != value)
					{
						if (teleportMarkerInstance != null)
						{
							teleportMarkerInstance.SetActive(value);
						}
						else
						{
							if (teleportMarkerOverride != null)
							{
								teleportMarkerInstance = Instantiate(teleportMarkerOverride);
							}
							else
							{
								teleportMarkerInstance = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
								Destroy(teleportMarkerInstance.GetComponent<Collider>());
								teleportMarkerInstance.transform.localScale = new Vector3(.1f,.01f,.1f);
								teleportMarkerInstance.GetComponent<MeshRenderer>().material.color = Color.black;
							}
						}
					}

					active = value;
				}
			}

			public Vector3 Pos
			{
				get { return pos; }
				set
				{
					pos = value;
					if (teleportMarkerInstance != null)
					{
						teleportMarkerInstance.transform.position = pos;
					}
				}
			}

			public Vector3 Dir
			{
				get { return dir; }
				set
				{
					dir = value;
					if (teleportMarkerInstance != null)
					{
						teleportMarkerInstance.transform.rotation = Quaternion.LookRotation(dir);
					}
				}
			}
		}

		public Teleporter teleporter = new Teleporter();


		void Start()
		{
			cpt = null;
			if (cpt == null)
			{
				cpt = gameObject.AddComponent<CopyTransform>();
			}

			cpt.followPosition = true;
			cpt.positionFollowType = CopyTransform.FollowType.Velocity;
			normalDrag = rig.rb.drag;

			
		}

		void Update()
		{
            // turn
            if (!InputMan.Grip(Side.Left) && !InputMan.Grip(Side.Right)) Turn();
			
			// grab walls and air
			if (grabWalls && !grabAir)
			{
				if (leftHandGrabbedObj != null || grabbingSide == Side.Left)
				{
					GrabMove(ref rig.leftHand, ref leftHandGrabPos, Side.Left, leftHandGrabbedObj);
				}

				if (rightHandGrabbedObj != null || grabbingSide == Side.Right)
				{
					GrabMove(ref rig.rightHand, ref rightHandGrabPos, Side.Right, rightHandGrabbedObj);
				}
			}
			else if (grabAir)
			{
				GrabMove(ref rig.leftHand, ref leftHandGrabPos, Side.Left);
				GrabMove(ref rig.rightHand, ref rightHandGrabPos, Side.Right);
			}

			Boosters();

			if (slidingMovement)
			{
				SlidingMovement();
			}

			// update lastVels
			lastVels[lastVelsIndex] = rig.rb.velocity;
			lastVelsIndex = ++lastVelsIndex % 5;
			
			// update last frame's grabbed objs
			lastleftHandGrabbedObj = leftHandGrabbedObj;
			lastrightHandGrabbedObj = rightHandGrabbedObj;

			if (teleportingMovement)
			{
				Teleporting();
			}

		}

		private void Teleporting()
		{
			// check for start of teleports
			if (InputMan.Up(Side.Left))
			{
				currentTeleportingSide = Side.Left;
			}

			if (InputMan.Up(Side.Right))
			{
				currentTeleportingSide = Side.Right;
			}

			// if the teleport laser is visible
			if (currentTeleportingSide != Side.None)
			{

				// check for end of teleport
				if (currentTeleportingSide == Side.Left && InputMan.ThumbstickIdle(Side.Left) ||
				    currentTeleportingSide == Side.Right && InputMan.ThumbstickIdle(Side.Right))
				{
					// complete the teleport
					TeleportTo(teleporter);

					currentTeleportingSide = Side.None;

					// delete the line renderer
					Destroy(lineRenderer);
					teleporter.Active = false;
				}
				else
				{

					// add a new linerenderer if needed
					if (lineRenderer == null)
					{
						lineRenderer = gameObject.AddComponent<LineRenderer>();
						lineRenderer.widthMultiplier = teleporter.lineRendererWidth;
						if (teleporter.lineRendererMaterialOverride != null)
						{
							lineRenderer.material = teleporter.lineRendererMaterialOverride;
						}
						else
						{
							lineRenderer.material.shader = Shader.Find("Unlit/Color");
							lineRenderer.material.color = Color.black;
						}
					}

					// simulate the curved ray
					Vector3 lastPos;
					Vector3 lastDir;
					List<Vector3> points = new List<Vector3>();

					if (currentTeleportingSide == Side.Left)
					{
						lastPos = rig.leftHand.position;
						lastDir = rig.leftHand.forward;
					}
					else
					{
						lastPos = rig.rightHand.position;
						lastDir = rig.rightHand.forward;
					}

					const float segmentLength = .25f;
					const float numSegments = 100f;

					// the teleport line will stop at a max distance
					for (int i = 0; i < numSegments; i++)
					{
						if (Physics.Raycast(lastPos, lastDir, out teleportHit, segmentLength, ~(1 << 2)))
						{
							points.Add(teleportHit.point);

							// if the hit point is valid
							if (Vector3.Angle(teleportHit.normal, Vector3.up) < teleporter.maxTeleportableSlope)
							{
								// define the point as a good teleportable point
								teleporter.Pos = teleportHit.point;
								Vector3 dir = rig.head.forward;
								dir = Vector3.ProjectOnPlane(dir, Vector3.up);
								if (teleporter.rotateOnTeleport)
								{
									Vector3 thumbstickDir = new Vector3(
										InputMan.ThumbstickX(currentTeleportingSide),
										0,
										InputMan.ThumbstickY(currentTeleportingSide));
									thumbstickDir.Normalize();
									float angle = -Vector3.SignedAngle(-Vector3.forward, thumbstickDir, Vector3.up);
									dir = Quaternion.Euler(0, angle, 0) * dir;
								}

								teleporter.Dir = dir;


								teleporter.Active = true;

							}
							else
							{
								// if the hit point is close enough to the last valid point
								teleporter.Active = !(Vector3.Distance(teleporter.Pos, teleportHit.point) > .1f);
							}


							break;
						}
						else
						{
							// add the point to the line renderer
							points.Add(lastPos);

							// calculate the next ray
							lastPos += lastDir * segmentLength;
							lastDir = Vector3.RotateTowards(lastDir, Vector3.down, teleporter.teleportCurve, 0);
						}
					}


					lineRenderer.positionCount = points.Count;
					lineRenderer.SetPositions(points.ToArray());
				}
			}
		}
		
		/// <summary>
		/// May not actually teleport if goal is not active
		/// </summary>
		/// <param name="goal">The target pos</param>
		private void TeleportTo(Teleporter goal)
		{
			if (goal.Active)
			{
				TeleportTo(goal.Pos, goal.Dir);
			}
		}

		public void TeleportTo(Vector3 position, Vector3 direction)
		{
			TeleportTo(position, Quaternion.LookRotation(direction));
		}
		
		public void TeleportTo(Vector3 position, Quaternion rotation)
		{
			float headRotOffset = Vector3.SignedAngle(transform.forward, Vector3.ProjectOnPlane(rig.head.transform.forward,Vector3.up), Vector3.up);
			rotation = Quaternion.Euler(0,-headRotOffset,0) * rotation;
			Quaternion origRot = transform.rotation;
			transform.rotation = rotation;
		
			Vector3 headPosOffset = transform.position - rig.head.transform.position;
			headPosOffset.y = 0;
			//transform.position = position + headPosOffset;
			
			transform.rotation = origRot;
			
			StartCoroutine(DoSmoothTeleport(position + headPosOffset, rotation, teleporter.smoothTeleportTime));
		}

		IEnumerator DoSmoothTeleport(Vector3 position, Quaternion rotation, float time)
		{
			float distance = Vector3.Distance(transform.position, position);
			float angle;
			Vector3 axis;
			rotation.ToAngleAxis(out angle, out axis);
			
			transform.rotation = rotation;
			
			for (float i = 0; i < time; i+=Time.deltaTime)
			{
				transform.position = Vector3.MoveTowards(transform.position, position, (Time.deltaTime / time)*distance);
				//transform.RotateAround(rig.head.position, axis,angle*(Time.deltaTime/time));
				yield return null;
			}

			transform.rotation = rotation;
			transform.position = position;
		}

		private void RoundVelToZero()
		{
			if (rig.rb.velocity.magnitude < minVel)
			{
				rig.rb.velocity = Vector3.zero;
			}
		}

		private void SlidingMovement()
		{
			bool useForce = false;
			Vector3 forward = -rig.head.forward;
			forward.y = 0;
			forward.Normalize();

			Vector3 right = new Vector3(-forward.z, 0, forward.x);


			if (useForce)
			{
				Vector3 forwardForce = Time.deltaTime * InputMan.ThumbstickY(Side.Left) * forward * 1000f;
				if (Mathf.Abs(Vector3.Dot(rig.rb.velocity, rig.head.forward)) < slidingSpeed)
				{
					rig.rb.AddForce(forwardForce);
				}

				Vector3 rightForce = Time.deltaTime * InputMan.ThumbstickX(Side.Left) * right * 1000f;
				if (Mathf.Abs(Vector3.Dot(rig.rb.velocity, rig.head.right)) < slidingSpeed)
				{
					rig.rb.AddForce(rightForce);
				}
			}
			else
			{
				Vector3 currentSpeed = rig.rb.velocity;
				Vector3 forwardSpeed = InputMan.ThumbstickY(Side.Left) * forward;
				Vector3 rightSpeed = InputMan.ThumbstickX(Side.Left) * right;
				Vector3 speed = forwardSpeed + rightSpeed;
				rig.rb.velocity = slidingSpeed * speed + (currentSpeed.y * rig.rb.transform.up);
			}
		}

		private void Boosters()
		{
			// update timers
			mainBoosterBudget += Time.deltaTime;
			mainBoosterBudget = Mathf.Clamp01(mainBoosterBudget);

			// use main booster
			if (stickBoostBrake && InputMan.ThumbstickPressDown(Side.Left) && InputMan.ThumbstickIdle(Side.Left))
			{
				// add timeout
				if (mainBoosterBudget - mainBoosterCost > 0)
				{
					// TODO speed can be faster by spherical Pythagorus
					// limit max speed
					if (Vector3.Dot(rig.rb.velocity, rig.head.forward) < mainBoosterMaxSpeed)
					{
						// add the force
						rig.rb.AddForce(rig.head.forward * mainBoosterForce * 100f);
					}

					mainBoosterBudget -= mainBoosterCost;
				}
			}

			// use main brake
			if (stickBoostBrake && InputMan.PadClick(Side.Right))
			{
				// add a bunch of drag
				rig.rb.drag = mainBrakeDrag;
			}
			else if (InputMan.PadClickUp(Side.Right))
			{
				rig.rb.drag = normalDrag;
				RoundVelToZero();
			}

			if (handBoosters)
			{
				if (InputMan.SecondaryMenuButton(Side.Left))
				{
					// TODO speed can be faster by spherical Pythagorus
					// limit max speed
					if (Vector3.Dot(rig.rb.velocity, rig.leftHand.forward) < maxHandBoosterSpeed)
					{
						// add the force
						rig.rb.AddForce(rig.leftHand.forward * handBoosterAccel * Time.deltaTime * 100f);
					}
				}

				if (InputMan.SecondaryMenuButton(Side.Right))
				{
					// TODO speed can be faster by spherical Pythagorus
					// limit max speed
					if (Vector3.Dot(rig.rb.velocity, rig.rightHand.forward) < maxHandBoosterSpeed)
					{
						// add the force
						rig.rb.AddForce(rig.rightHand.forward * handBoosterAccel * Time.deltaTime * 100f);
					}
				}
			}
		}

		private void Turn()
		{
			// don't turn if currently teleporting
			if (currentTeleportingSide != Side.None)
			{
				return;
			}
			
			Vector3 pivot = rig.head.position;
            if (grabbingSide == Side.Left) {
                pivot = rig.leftHand.position;
            }
            else if (grabbingSide == Side.Right) {
                pivot = rig.rightHand.position;
            }

            if (continuousRotation)
			{
				Side turnInputLocal = turnInput;
				if (roll)
				{
					turnInputLocal = Side.Right;
				}
				if (yaw && Mathf.Abs(InputMan.ThumbstickX(turnInputLocal)) > turnNullZone)
				{
					rig.rb.transform.RotateAround(pivot, rig.rb.transform.up,
						InputMan.ThumbstickX(turnInputLocal) * Time.deltaTime * continuousRotationSpeed * 2);
				}
				else if (pitch && Mathf.Abs(InputMan.ThumbstickY(turnInputLocal)) > turnNullZone)
				{
					rig.rb.transform.RotateAround(pivot, rig.head.right,
						InputMan.ThumbstickY(turnInputLocal) * Time.deltaTime * continuousRotationSpeed * 2);
				}
				else if (roll && Mathf.Abs(InputMan.ThumbstickX(Side.Left)) > turnNullZone)
				{
					rig.rb.transform.RotateAround(pivot, rig.head.forward,
						InputMan.ThumbstickX(Side.Left) * Time.deltaTime * continuousRotationSpeed * 2);
				}
			}
			else
			{
				Side turnInputLocal = turnInput;
				if (roll)
				{
					turnInputLocal = Side.Right;
				}
				if (yaw && InputMan.Left(turnInputLocal))
				{
					rig.rb.transform.RotateAround(pivot, rig.rb.transform.up, -snapRotationAmount);
				}
				else if (yaw && InputMan.Right(turnInputLocal))
				{
					rig.rb.transform.RotateAround(pivot, rig.rb.transform.up, snapRotationAmount);
				}
				else if (pitch && InputMan.Up(turnInputLocal))
				{
					rig.rb.transform.RotateAround(pivot, rig.head.transform.forward, -snapRotationAmount);
				}
				else if (pitch && InputMan.Down(turnInputLocal))
				{
					rig.rb.transform.RotateAround(pivot, rig.head.transform.forward, snapRotationAmount);
				}
				else if (roll && InputMan.Left(Side.Left))
				{
					rig.rb.transform.RotateAround(pivot, rig.head.transform.right, -snapRotationAmount);
				}
				else if (roll && InputMan.Right(Side.Left))
				{
					rig.rb.transform.RotateAround(pivot, rig.head.transform.right, snapRotationAmount);
				}
			}
		}

		public void ResetOrientation()
		{
			rig.rb.transform.localRotation = Quaternion.identity;
		}

		private void GrabMove(ref Transform hand, ref GameObject grabPos, Side side, Transform parent = null)
		{
			if (InputMan.GripDown(side) || (InputMan.Grip(side) && 
                ((side == Side.Left && leftHandGrabbedObj != null && lastleftHandGrabbedObj == null) || 
                (side == Side.Right && rightHandGrabbedObj != null && lastrightHandGrabbedObj == null))))
			{
				grabbingSide = side;

				if (grabPos != null)
				{
					Destroy(grabPos.gameObject);
				}

				grabPos = new GameObject(side + " Hand Grab Pos");
				grabPos.transform.position = hand.position;
				grabPos.transform.SetParent(parent);
				cpt.target = grabPos.transform;
				cpt.positionOffset = rig.rb.position - hand.position;
				
				InputMan.Vibrate(side, 1);

				// if event has subscribers, execute
				OnGrab?.Invoke(parent, side);
			}
			else if (side == grabbingSide)
			{
				if (InputMan.Grip(side))
				{
					cpt.positionOffset = rig.rb.position - hand.position;
				}
				else
				{
					if (side == grabbingSide)
					{
						grabbingSide = Side.None;
					}

					if (grabPos != null)
					{
						OnRelease?.Invoke(grabPos.transform, side);
						Destroy(grabPos.gameObject);
						cpt.target = null;
						//rig.rb.velocity = MedianAvg(lastVels);
						rig.rb.velocity = -transform.TransformVector(InputMan.LocalControllerVelocity(side));
						RoundVelToZero();
					}
				}
			}
		}

		public void SetGrabbedObj(Transform obj, Side side)
		{
			if (side == Side.Left)
			{
				if (obj == null)
				{
					leftHandGrabbedObj = null;
				}
				else
				{
					leftHandGrabbedObj = obj;
				}
			}
			else if (side == Side.Right)
			{
				if (obj == null)
				{
					rightHandGrabbedObj = null;
				}
				else
				{
					rightHandGrabbedObj = obj;
				}
			}
		}

		Vector3 MedianAvg(Vector3[] inputArray)
		{
			List<Vector3> list = new List<Vector3>(inputArray);
			list = list.OrderBy(x => x.magnitude).ToList();
			list.RemoveAt(0);
			list.RemoveAt(list.Count - 1);
			Vector3 result = new Vector3(
				list.Average(x => x.x),
				list.Average(x => x.y),
				list.Average(x => x.z));
			return result;
		}
	}
}