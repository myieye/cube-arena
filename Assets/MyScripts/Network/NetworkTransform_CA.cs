// Decompiled with JetBrains decompiler
// Type: UnityEngine.Networking.NetworkTransform
// Assembly: UnityEngine.Networking, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8B34E19C-EF53-416E-AE36-35C45BAFD2DE
// Assembly location: C:\Users\Blake\sandbox\unity\test-project\Library\UnityAssemblies\UnityEngine.Networking.dll

using System;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine.AI;
//using UnityEditor;

namespace UnityEngine.Networking {
  /// <summary>
  ///   <para>A component to synchronize the position of networked objects.</para>
  /// </summary>
  [DisallowMultipleComponent]
  [AddComponentMenu ("Network/NetworkTransform")]
  public class NetworkTransform_CA : NetworkBehaviour {
    [SerializeField]
    private float m_SendInterval = 0.1f;
    [SerializeField]
    private NetworkTransform_CA.AxisSyncMode m_SyncRotationAxis = NetworkTransform_CA.AxisSyncMode.AxisXYZ;
    [SerializeField]
    private float m_MovementTheshold = 1f / 1000f;
    [SerializeField]
    private float m_SnapThreshold = 5f;
    [SerializeField]
    private float m_InterpolateRotation = 1f;
    [SerializeField]
    private float m_InterpolateMovement = 1f;
    private bool m_Grounded = true;
    private const float k_LocalMovementThreshold = 1E-05f;
    private const float k_LocalRotationThreshold = 1E-05f;
    private const float k_LocalVelocityThreshold = 1E-05f;
    private const float k_MoveAheadRatio = 0.1f;
    [SerializeField]
    private NetworkTransform_CA.TransformSyncMode m_TransformSyncMode;
    [SerializeField]
    private NetworkTransform_CA.CompressionSyncMode m_RotationSyncCompression;
    [SerializeField]
    private bool m_SyncSpin;
    [SerializeField]
    private NetworkTransform_CA.ClientMoveCallback3D m_ClientMoveCallback3D;
    [SerializeField]
    private NetworkTransform_CA.ClientMoveCallback2D m_ClientMoveCallback2D;
    private Rigidbody m_RigidBody3D;
    private Rigidbody2D m_RigidBody2D;
    private CharacterController m_CharacterController;
    private Vector3 m_TargetSyncPosition;
    private Vector3 m_TargetSyncVelocity;
    private Vector3 m_FixedPosDiff;
    private Quaternion m_TargetSyncRotation3D;
    private Vector3 m_TargetSyncAngularVelocity3D;
    private float m_TargetSyncRotation2D;
    private float m_TargetSyncAngularVelocity2D;
    private float m_LastClientSyncTime;
    private float m_LastClientSendTime;
    private Vector3 m_PrevPosition;
    private Quaternion m_PrevRotation;
    private float m_PrevRotation2D;
    private float m_PrevVelocity;
    private NetworkWriter m_LocalTransformWriter;

    /// <summary>
    ///   <para>What method to use to sync the object's position.</para>
    /// </summary>
    public NetworkTransform_CA.TransformSyncMode transformSyncMode {
      get {
        return this.m_TransformSyncMode;
      }
      set {
        this.m_TransformSyncMode = value;
      }
    }

    /// <summary>
    ///   <para>The sendInterval controls how often state updates are sent for this object.</para>
    /// </summary>
    public float sendInterval {
      get {
        return this.m_SendInterval;
      }
      set {
        this.m_SendInterval = value;
      }
    }

    /// <summary>
    ///   <para>Which axis should rotation by synchronized for.</para>
    /// </summary>
    public NetworkTransform_CA.AxisSyncMode syncRotationAxis {
      get {
        return this.m_SyncRotationAxis;
      }
      set {
        this.m_SyncRotationAxis = value;
      }
    }

    /// <summary>
    ///   <para>How much to compress rotation sync updates.</para>
    /// </summary>
    public NetworkTransform_CA.CompressionSyncMode rotationSyncCompression {
      get {
        return this.m_RotationSyncCompression;
      }
      set {
        this.m_RotationSyncCompression = value;
      }
    }

    public bool syncSpin {
      get {
        return this.m_SyncSpin;
      }
      set {
        this.m_SyncSpin = value;
      }
    }

    /// <summary>
    ///   <para>The distance that an object can move without sending a movement synchronization update.</para>
    /// </summary>
    public float movementTheshold {
      get {
        return this.m_MovementTheshold;
      }
      set {
        this.m_MovementTheshold = value;
      }
    }

    /// <summary>
    ///   <para>If a movement update puts an object further from its current position that this value, it will snap to the position instead of moving smoothly.</para>
    /// </summary>
    public float snapThreshold {
      get {
        return this.m_SnapThreshold;
      }
      set {
        this.m_SnapThreshold = value;
      }
    }

    /// <summary>
    ///   <para>Enables interpolation of the synchronized rotation.</para>
    /// </summary>
    public float interpolateRotation {
      get {
        return this.m_InterpolateRotation;
      }
      set {
        this.m_InterpolateRotation = value;
      }
    }

    /// <summary>
    ///   <para>Enables interpolation of the synchronized movement.</para>
    /// </summary>
    public float interpolateMovement {
      get {
        return this.m_InterpolateMovement;
      }
      set {
        this.m_InterpolateMovement = value;
      }
    }

    /// <summary>
    ///   <para>A callback that can be used to validate on the server, the movement of client authoritative objects.</para>
    /// </summary>
    public NetworkTransform_CA.ClientMoveCallback3D clientMoveCallback3D {
      get {
        return this.m_ClientMoveCallback3D;
      }
      set {
        this.m_ClientMoveCallback3D = value;
      }
    }

    /// <summary>
    ///   <para>A callback that can be used to validate on the server, the movement of client authoritative objects.</para>
    /// </summary>
    public NetworkTransform_CA.ClientMoveCallback2D clientMoveCallback2D {
      get {
        return this.m_ClientMoveCallback2D;
      }
      set {
        this.m_ClientMoveCallback2D = value;
      }
    }

    /// <summary>
    ///   <para>Cached CharacterController.</para>
    /// </summary>
    public CharacterController characterContoller {
      get {
        return this.m_CharacterController;
      }
    }

    /// <summary>
    ///   <para>Cached Rigidbody.</para>
    /// </summary>
    public Rigidbody rigidbody3D {
      get {
        return this.m_RigidBody3D;
      }
    }

    /// <summary>
    ///   <para>Cached Rigidbody2D.</para>
    /// </summary>
    public Rigidbody2D rigidbody2D {
      get {
        return this.m_RigidBody2D;
      }
    }

    /// <summary>
    ///   <para>The most recent time when a movement synchronization packet arrived for this object.</para>
    /// </summary>
    public float lastSyncTime {
      get {
        return this.m_LastClientSyncTime;
      }
    }

    /// <summary>
    ///   <para>The target position interpolating towards.</para>
    /// </summary>
    public Vector3 targetSyncPosition {
      get {
        return this.m_TargetSyncPosition;
      }
    }

    /// <summary>
    ///   <para>The velocity send for synchronization.</para>
    /// </summary>
    public Vector3 targetSyncVelocity {
      get {
        return this.m_TargetSyncVelocity;
      }
    }

    /// <summary>
    ///   <para>The target position interpolating towards.</para>
    /// </summary>
    public Quaternion targetSyncRotation3D {
      get {
        return this.m_TargetSyncRotation3D;
      }
    }

    /// <summary>
    ///   <para>The target rotation interpolating towards.</para>
    /// </summary>
    public float targetSyncRotation2D {
      get {
        return this.m_TargetSyncRotation2D;
      }
    }

    /// <summary>
    ///   <para>Tells the NetworkTransform that it is on a surface (this is the default). </para>
    /// </summary>
    public bool grounded {
      get {
        return this.m_Grounded;
      }
      set {
        this.m_Grounded = value;
      }
    }

    private bool initialized; // EDIT
    private NavMeshAgent agent;
    private int navMeshMissCount;
    private Vector3 startPosition;
    private Quaternion startRotation;

    protected virtual void CustomAwake () {
      agent = GetComponent<NavMeshAgent> ();
      //rb = GetComponent<Rigidbody> ();

      startPosition = transform.position;
      startRotation = transform.rotation;

      if (agent && transformSyncMode == TransformSyncMode.SyncAgent) {
        agent.enabled = false;
      }
    }

    private void Start () {
      InvokeRepeating ("TryInit", 0, 0.1f);
    }

    private void TryInit () {
      if (Init ()) {
        CancelInvoke ("TryInit");
      }
    }

    protected bool Init () {
      if (!TransformUtil.IsInitialized) return false;

      switch (transformSyncMode) {
        case TransformSyncMode.SyncAgent:
          //agent.enabled = false;
          agent.Warp (startPosition.ToLocal ());
          transform.rotation = startRotation.ToLocal ();
          agent.enabled = true;
          break;
        default:
          transform.MoveToLocal ();
          break;
      }

      return initialized = true;
    }

    private void Awake () {
      this.CustomAwake (); // EDIT
      this.m_RigidBody3D = this.GetComponent<Rigidbody> ();
      this.m_RigidBody2D = this.GetComponent<Rigidbody2D> ();
      this.m_CharacterController = this.GetComponent<CharacterController> ();
      this.m_PrevPosition = this.transform.position;
      this.m_PrevRotation = this.transform.rotation;
      this.m_PrevVelocity = 0.0f;
      if (!this.localPlayerAuthority)
        return;
      this.m_LocalTransformWriter = new NetworkWriter ();
    }

    private void OnValidate () {
      if (this.m_TransformSyncMode < NetworkTransform_CA.TransformSyncMode.SyncNone || this.m_TransformSyncMode > NetworkTransform_CA.TransformSyncMode.SyncCharacterController)
        this.m_TransformSyncMode = NetworkTransform_CA.TransformSyncMode.SyncTransform;
      if ((double) this.m_SendInterval < 0.0)
        this.m_SendInterval = 0.0f;
      if (this.m_SyncRotationAxis < NetworkTransform_CA.AxisSyncMode.None || this.m_SyncRotationAxis > NetworkTransform_CA.AxisSyncMode.AxisXYZ)
        this.m_SyncRotationAxis = NetworkTransform_CA.AxisSyncMode.None;
      if ((double) this.m_MovementTheshold < 0.0)
        this.m_MovementTheshold = 0.0f;
      if ((double) this.m_SnapThreshold < 0.0)
        this.m_SnapThreshold = 0.01f;
      if ((double) this.m_InterpolateRotation < 0.0)
        this.m_InterpolateRotation = 0.01f;
      if ((double) this.m_InterpolateMovement >= 0.0)
        return;
      this.m_InterpolateMovement = 0.01f;
    }

    public override void OnStartServer () {
      this.m_LastClientSyncTime = 0.0f;
    }

    public override bool OnSerialize (NetworkWriter writer, bool initialState) {
      if (!initialState) {
        if ((int) this.syncVarDirtyBits == 0) {
          writer.WritePackedUInt32 (0U);
          return false;
        }
        writer.WritePackedUInt32 (1U);
      }
      switch (this.transformSyncMode) {
        case NetworkTransform_CA.TransformSyncMode.SyncNone:
          return false;
        case NetworkTransform_CA.TransformSyncMode.SyncAgent:
        case NetworkTransform_CA.TransformSyncMode.SyncTransform:
          this.SerializeModeTransform (writer);
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncRigidbody2D:
          this.SerializeMode2D (writer);
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncRigidbody3D:
          this.SerializeMode3D (writer);
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncCharacterController:
          this.SerializeModeCharacterController (writer);
          break;
      }
      return true;
    }

    private void SerializeModeTransform (NetworkWriter writer) {
      writer.Write (this.transform.position.ToServer ());
      if (this.m_SyncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
        NetworkTransform_CA.SerializeRotation3D (writer, this.transform.rotation.ToServer (), this.syncRotationAxis, this.rotationSyncCompression);
      this.m_PrevPosition = this.transform.position;
      this.m_PrevRotation = this.transform.rotation;
      this.m_PrevVelocity = 0.0f;
    }

    private void SerializeMode3D (NetworkWriter writer) {
      if (this.isServer && (double) this.m_LastClientSyncTime != 0.0) {
        writer.Write (this.m_TargetSyncPosition.ToServer ()); // EDIT
        NetworkTransform_CA.SerializeVelocity3D (writer, this.m_TargetSyncVelocity.ToServerDirection (), NetworkTransform_CA.CompressionSyncMode.None);
        if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
          NetworkTransform_CA.SerializeRotation3D (writer, this.m_TargetSyncRotation3D.ToServer (), this.syncRotationAxis, this.rotationSyncCompression);
      } else {
        writer.Write (this.m_RigidBody3D.position.ToServer ());
        NetworkTransform_CA.SerializeVelocity3D (writer, this.m_RigidBody3D.velocity.ToServerDirection (), NetworkTransform_CA.CompressionSyncMode.None);
        if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
          NetworkTransform_CA.SerializeRotation3D (writer, this.m_RigidBody3D.rotation.ToServer (), this.syncRotationAxis, this.rotationSyncCompression);
      }
      if (this.m_SyncSpin)
        NetworkTransform_CA.SerializeSpin3D (writer, this.m_RigidBody3D.angularVelocity.ToServerDirection (), this.syncRotationAxis, this.rotationSyncCompression);
      this.m_PrevPosition = this.m_RigidBody3D.position;
      this.m_PrevRotation = this.transform.rotation;
      this.m_PrevVelocity = this.m_RigidBody3D.velocity.sqrMagnitude;
    }

    private void SerializeModeCharacterController (NetworkWriter writer) {
      if (this.isServer && (double) this.m_LastClientSyncTime != 0.0) {
        writer.Write (this.m_TargetSyncPosition);
        if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
          NetworkTransform_CA.SerializeRotation3D (writer, this.m_TargetSyncRotation3D, this.syncRotationAxis, this.rotationSyncCompression);
      } else {
        writer.Write (this.transform.position);
        if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
          NetworkTransform_CA.SerializeRotation3D (writer, this.transform.rotation, this.syncRotationAxis, this.rotationSyncCompression);
      }
      this.m_PrevPosition = this.transform.position;
      this.m_PrevRotation = this.transform.rotation;
      this.m_PrevVelocity = 0.0f;
    }

    private void SerializeMode2D (NetworkWriter writer) {
      if (this.isServer && (double) this.m_LastClientSyncTime != 0.0) {
        writer.Write ((Vector2) this.m_TargetSyncPosition);
        NetworkTransform_CA.SerializeVelocity2D (writer, (Vector2) this.m_TargetSyncVelocity, NetworkTransform_CA.CompressionSyncMode.None);
        if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None) {
          float rot = this.m_TargetSyncRotation2D % 360f;
          if ((double) rot < 0.0)
            rot += 360f;
          NetworkTransform_CA.SerializeRotation2D (writer, rot, this.rotationSyncCompression);
        }
      } else {
        writer.Write (this.m_RigidBody2D.position);
        NetworkTransform_CA.SerializeVelocity2D (writer, this.m_RigidBody2D.velocity, NetworkTransform_CA.CompressionSyncMode.None);
        if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None) {
          float rot = this.m_RigidBody2D.rotation % 360f;
          if ((double) rot < 0.0)
            rot += 360f;
          NetworkTransform_CA.SerializeRotation2D (writer, rot, this.rotationSyncCompression);
        }
      }
      if (this.m_SyncSpin)
        NetworkTransform_CA.SerializeSpin2D (writer, this.m_RigidBody2D.angularVelocity, this.rotationSyncCompression);
      this.m_PrevPosition = (Vector3) this.m_RigidBody2D.position;
      this.m_PrevRotation = this.transform.rotation;
      this.m_PrevVelocity = this.m_RigidBody2D.velocity.sqrMagnitude;
    }

    public override void OnDeserialize (NetworkReader reader, bool initialState) {
      if (this.isServer && NetworkServer.localClientActive || !initialState && (int) reader.ReadPackedUInt32 () == 0)
        return;
      switch (this.transformSyncMode) {
        case NetworkTransform_CA.TransformSyncMode.SyncNone:
          return;
        case NetworkTransform_CA.TransformSyncMode.SyncTransform:
          this.UnserializeModeTransform (reader, initialState);
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncAgent:
          this.UnserializeModeAgent (reader, initialState);
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncRigidbody2D:
          this.UnserializeMode2D (reader, initialState);
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncRigidbody3D:
          this.UnserializeMode3D (reader, initialState);
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncCharacterController:
          this.UnserializeModeCharacterController (reader, initialState);
          break;
      }
      this.m_LastClientSyncTime = Time.time;
    }

    private void UnserializeModeTransform (NetworkReader reader, bool initialState) {
      if (this.hasAuthority) {
        reader.ReadVector3 ();
        if (this.syncRotationAxis == NetworkTransform_CA.AxisSyncMode.None)
          return;
        NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
      } else if (this.isServer && this.m_ClientMoveCallback3D != null) {
        Vector3 position = reader.ReadVector3 ().ToLocal (); // EDIT
        Vector3 zero = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
          rotation = NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
        if (!this.m_ClientMoveCallback3D (ref position, ref zero, ref rotation))
          return;
        this.transform.position = position;
        if (this.syncRotationAxis == NetworkTransform_CA.AxisSyncMode.None)
          return;
        this.transform.rotation = rotation;
      } else {
        this.transform.position = reader.ReadVector3 ().ToLocal (); // EDIT
        if (this.syncRotationAxis == NetworkTransform_CA.AxisSyncMode.None)
          return;
        this.transform.rotation = NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
      }
    }

    private void UnserializeModeAgent (NetworkReader reader, bool initialState) {
      if (this.hasAuthority) {
        reader.ReadVector3 ();
        if (this.syncRotationAxis == NetworkTransform_CA.AxisSyncMode.None)
          return;
        NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
      } else if (this.isServer && this.m_ClientMoveCallback3D != null) {

        Vector3 targetPosition = reader.ReadVector3 ().ToLocal (); // EDIT
        Vector3 zero = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        if (agent.isOnNavMesh) {
          navMeshMissCount = 0;

          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            rotation = NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
          if (!this.m_ClientMoveCallback3D (ref targetPosition, ref zero, ref rotation))
            return;
          agent.Move (targetPosition - transform.position);
          //this.transform.position = targetPosition;
          if (this.syncRotationAxis == NetworkTransform_CA.AxisSyncMode.None)
            return;
          this.transform.rotation = rotation;
        } else {
          navMeshMissCount++;
          if (navMeshMissCount > 5) {
            agent.Warp (targetPosition);
          }
        }

      } else {
        var targetPosition = reader.ReadVector3 ().ToLocal (); // EDIT

        if (agent.isOnNavMesh) {
          navMeshMissCount = 0;
          //this.transform.position = reader.ReadVector3 ().ToLocal (); // EDIT
          agent.Move (targetPosition - transform.position);

          if (this.syncRotationAxis == NetworkTransform_CA.AxisSyncMode.None)
            return;
          this.transform.rotation = NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
        } else {
          navMeshMissCount++;
          if (navMeshMissCount > 5) {
            agent.Warp (targetPosition);
          }
        }
      }
    }

    private void UnserializeMode3D (NetworkReader reader, bool initialState) {
      if (this.hasAuthority) {
        reader.ReadVector3 ();
        reader.ReadVector3 ();
        if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
          NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
        if (!this.syncSpin)
          return;
        NetworkTransform_CA.UnserializeSpin3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
      } else {
        if (this.isServer && this.m_ClientMoveCallback3D != null) {
          Vector3 position = reader.ReadVector3 ().ToLocal (); // EDIT
          Vector3 velocity = reader.ReadVector3 ().ToLocalDirection (); // EDIT
          Quaternion rotation = Quaternion.identity;
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            rotation = NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
          if (!this.m_ClientMoveCallback3D (ref position, ref velocity, ref rotation))
            return;
          this.m_TargetSyncPosition = position;
          this.m_TargetSyncVelocity = velocity;
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            this.m_TargetSyncRotation3D = rotation;
        } else {
          this.m_TargetSyncPosition = reader.ReadVector3 ().ToLocal ();
          this.m_TargetSyncVelocity = reader.ReadVector3 ().ToLocalDirection ();
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            this.m_TargetSyncRotation3D = NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
        }
        if (this.syncSpin)
          this.m_TargetSyncAngularVelocity3D = NetworkTransform_CA.UnserializeSpin3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
        if ((UnityEngine.Object) this.m_RigidBody3D == (UnityEngine.Object) null)
          return;
        if (this.isServer && !this.isClient) {
          this.m_RigidBody3D.MovePosition (this.m_TargetSyncPosition);
          this.m_RigidBody3D.MoveRotation (this.m_TargetSyncRotation3D);
          this.m_RigidBody3D.velocity = this.m_TargetSyncVelocity;
        } else if ((double) this.GetNetworkSendInterval () == 0.0) {
          this.m_RigidBody3D.MovePosition (this.m_TargetSyncPosition);
          this.m_RigidBody3D.velocity = this.m_TargetSyncVelocity;
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            this.m_RigidBody3D.MoveRotation (this.m_TargetSyncRotation3D);
          if (!this.syncSpin)
            return;
          this.m_RigidBody3D.angularVelocity = this.m_TargetSyncAngularVelocity3D;
        } else {
          if ((double) (this.m_RigidBody3D.position - this.m_TargetSyncPosition).magnitude > (double) this.snapThreshold) {
            this.m_RigidBody3D.position = this.m_TargetSyncPosition;
            this.m_RigidBody3D.velocity = this.m_TargetSyncVelocity;
          }
          if ((double) this.interpolateRotation == 0.0 && this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None) {
            this.m_RigidBody3D.rotation = this.m_TargetSyncRotation3D;
            if (this.syncSpin)
              this.m_RigidBody3D.angularVelocity = this.m_TargetSyncAngularVelocity3D;
          }
          if ((double) this.m_InterpolateMovement == 0.0)
            this.m_RigidBody3D.position = this.m_TargetSyncPosition;
          if (!initialState || this.syncRotationAxis == NetworkTransform_CA.AxisSyncMode.None)
            return;
          this.m_RigidBody3D.rotation = this.m_TargetSyncRotation3D;
        }
      }
    }

    private void UnserializeMode2D (NetworkReader reader, bool initialState) {
      if (this.hasAuthority) {
        reader.ReadVector2 ();
        reader.ReadVector2 ();
        if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None) {
          double num1 = (double) NetworkTransform_CA.UnserializeRotation2D (reader, this.rotationSyncCompression);
        }
        if (!this.syncSpin)
          return;
        double num2 = (double) NetworkTransform_CA.UnserializeSpin2D (reader, this.rotationSyncCompression);
      } else {
        if ((UnityEngine.Object) this.m_RigidBody2D == (UnityEngine.Object) null)
          return;
        if (this.isServer && this.m_ClientMoveCallback2D != null) {
          Vector2 position = reader.ReadVector2 ();
          Vector2 velocity = reader.ReadVector2 ();
          float rotation = 0.0f;
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            rotation = NetworkTransform_CA.UnserializeRotation2D (reader, this.rotationSyncCompression);
          if (!this.m_ClientMoveCallback2D (ref position, ref velocity, ref rotation))
            return;
          this.m_TargetSyncPosition = (Vector3) position;
          this.m_TargetSyncVelocity = (Vector3) velocity;
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            this.m_TargetSyncRotation2D = rotation;
        } else {
          this.m_TargetSyncPosition = (Vector3) reader.ReadVector2 ();
          this.m_TargetSyncVelocity = (Vector3) reader.ReadVector2 ();
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            this.m_TargetSyncRotation2D = NetworkTransform_CA.UnserializeRotation2D (reader, this.rotationSyncCompression);
        }
        if (this.syncSpin)
          this.m_TargetSyncAngularVelocity2D = NetworkTransform_CA.UnserializeSpin2D (reader, this.rotationSyncCompression);
        if (this.isServer && !this.isClient) {
          this.transform.position = this.m_TargetSyncPosition;
          this.m_RigidBody2D.MoveRotation (this.m_TargetSyncRotation2D);
          this.m_RigidBody2D.velocity = (Vector2) this.m_TargetSyncVelocity;
        } else if ((double) this.GetNetworkSendInterval () == 0.0) {
          this.transform.position = this.m_TargetSyncPosition;
          this.m_RigidBody2D.velocity = (Vector2) this.m_TargetSyncVelocity;
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            this.m_RigidBody2D.MoveRotation (this.m_TargetSyncRotation2D);
          if (!this.syncSpin)
            return;
          this.m_RigidBody2D.angularVelocity = this.m_TargetSyncAngularVelocity2D;
        } else {
          if ((double) (this.m_RigidBody2D.position - (Vector2) this.m_TargetSyncPosition).magnitude > (double) this.snapThreshold) {
            this.m_RigidBody2D.position = (Vector2) this.m_TargetSyncPosition;
            this.m_RigidBody2D.velocity = (Vector2) this.m_TargetSyncVelocity;
          }
          if ((double) this.interpolateRotation == 0.0 && this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None) {
            this.m_RigidBody2D.rotation = this.m_TargetSyncRotation2D;
            if (this.syncSpin)
              this.m_RigidBody2D.angularVelocity = this.m_TargetSyncAngularVelocity2D;
          }
          if ((double) this.m_InterpolateMovement == 0.0)
            this.m_RigidBody2D.position = (Vector2) this.m_TargetSyncPosition;
          if (!initialState)
            return;
          this.m_RigidBody2D.rotation = this.m_TargetSyncRotation2D;
        }
      }
    }

    private void UnserializeModeCharacterController (NetworkReader reader, bool initialState) {
      if (this.hasAuthority) {
        reader.ReadVector3 ();
        if (this.syncRotationAxis == NetworkTransform_CA.AxisSyncMode.None)
          return;
        NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
      } else {
        if (this.isServer && this.m_ClientMoveCallback3D != null) {
          Vector3 position = reader.ReadVector3 ().ToLocal (); // EDIT
          Quaternion rotation = Quaternion.identity;
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            rotation = NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
          if ((UnityEngine.Object) this.m_CharacterController == (UnityEngine.Object) null)
            return;
          Vector3 velocity = this.m_CharacterController.velocity;
          if (!this.m_ClientMoveCallback3D (ref position, ref velocity, ref rotation))
            return;
          this.m_TargetSyncPosition = position;
          this.m_TargetSyncVelocity = velocity;
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            this.m_TargetSyncRotation3D = rotation;
        } else {
          this.m_TargetSyncPosition = reader.ReadVector3 ().ToLocal (); // EDIT
          if (this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            this.m_TargetSyncRotation3D = NetworkTransform_CA.UnserializeRotation3D (reader, this.syncRotationAxis, this.rotationSyncCompression);
        }
        if ((UnityEngine.Object) this.m_CharacterController == (UnityEngine.Object) null)
          return;
        this.m_FixedPosDiff = (this.m_TargetSyncPosition - this.transform.position) / this.GetNetworkSendInterval () * Time.fixedDeltaTime;
        if (this.isServer && !this.isClient) {
          this.transform.position = this.m_TargetSyncPosition;
          this.transform.rotation = this.m_TargetSyncRotation3D;
        } else if ((double) this.GetNetworkSendInterval () == 0.0) {
          this.transform.position = this.m_TargetSyncPosition;
          if (this.syncRotationAxis == NetworkTransform_CA.AxisSyncMode.None)
            return;
          this.transform.rotation = this.m_TargetSyncRotation3D;
        } else {
          if ((double) (this.transform.position - this.m_TargetSyncPosition).magnitude > (double) this.snapThreshold)
            this.transform.position = this.m_TargetSyncPosition;
          if ((double) this.interpolateRotation == 0.0 && this.syncRotationAxis != NetworkTransform_CA.AxisSyncMode.None)
            this.transform.rotation = this.m_TargetSyncRotation3D;
          if ((double) this.m_InterpolateMovement == 0.0)
            this.transform.position = this.m_TargetSyncPosition;
          if (!initialState || this.syncRotationAxis == NetworkTransform_CA.AxisSyncMode.None)
            return;
          this.transform.rotation = this.m_TargetSyncRotation3D;
        }
      }
    }

    private void FixedUpdate () {
      if (!initialized) return; // EDIT

      if (this.isServer)
        this.FixedUpdateServer ();
      if (!this.isClient)
        return;
      this.FixedUpdateClient ();
    }

    private void FixedUpdateServer () {
      if ((int) this.syncVarDirtyBits != 0 || !NetworkServer.active || (!this.isServer || (double) this.GetNetworkSendInterval () == 0.0) || (double) (this.transform.position - this.m_PrevPosition).magnitude < (double) this.movementTheshold && (double) Quaternion.Angle (this.m_PrevRotation, this.transform.rotation) < (double) this.movementTheshold)
        return;
      this.SetDirtyBit (1U);
    }

    private void FixedUpdateClient () {
      if ((double) this.m_LastClientSyncTime == 0.0 || !NetworkServer.active && !NetworkClient.active || (!this.isServer && !this.isClient || ((double) this.GetNetworkSendInterval () == 0.0 || this.hasAuthority)))
        return;
      switch (this.transformSyncMode) {
        case NetworkTransform_CA.TransformSyncMode.SyncRigidbody2D:
          this.InterpolateTransformMode2D ();
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncRigidbody3D:
          this.InterpolateTransformMode3D ();
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncCharacterController:
          this.InterpolateTransformModeCharacterController ();
          break;
      }
    }

    private void InterpolateTransformMode3D () {
      if ((double) this.m_InterpolateMovement != 0.0)
        this.m_RigidBody3D.velocity = (this.m_TargetSyncPosition - this.m_RigidBody3D.position) * this.m_InterpolateMovement / this.GetNetworkSendInterval ();
      if ((double) this.interpolateRotation != 0.0)
        this.m_RigidBody3D.MoveRotation (Quaternion.Slerp (this.m_RigidBody3D.rotation, this.m_TargetSyncRotation3D, Time.fixedDeltaTime * this.interpolateRotation));
      this.m_TargetSyncPosition += this.m_TargetSyncVelocity * Time.fixedDeltaTime * 0.1f;
    }

    private void InterpolateTransformModeCharacterController () {
      if (this.m_FixedPosDiff == Vector3.zero && this.m_TargetSyncRotation3D == this.transform.rotation)
        return;
      if ((double) this.m_InterpolateMovement != 0.0) {
        int num1 = (int) this.m_CharacterController.Move (this.m_FixedPosDiff * this.m_InterpolateMovement);
      }
      if ((double) this.interpolateRotation != 0.0)
        this.transform.rotation = Quaternion.Slerp (this.transform.rotation, this.m_TargetSyncRotation3D, (float) ((double) Time.fixedDeltaTime * (double) this.interpolateRotation * 10.0));
      if ((double) Time.time - (double) this.m_LastClientSyncTime <= (double) this.GetNetworkSendInterval ())
        return;
      this.m_FixedPosDiff = Vector3.zero;
      int num2 = (int) this.m_CharacterController.Move (this.m_TargetSyncPosition - this.transform.position);
    }

    private void InterpolateTransformMode2D () {
      if ((double) this.m_InterpolateMovement != 0.0) {
        Vector2 velocity = this.m_RigidBody2D.velocity;
        Vector2 vector2 = ((Vector2) this.m_TargetSyncPosition - this.m_RigidBody2D.position) * this.m_InterpolateMovement / this.GetNetworkSendInterval ();
        if (!this.m_Grounded && (double) vector2.y < 0.0)
          vector2.y = velocity.y;
        this.m_RigidBody2D.velocity = vector2;
      }
      if ((double) this.interpolateRotation != 0.0) {
        float num1 = this.m_RigidBody2D.rotation % 360f;
        if ((double) num1 < 0.0) {
          float num2 = num1 + 360f;
        }
        this.m_RigidBody2D.MoveRotation (Quaternion.Slerp (this.transform.rotation, Quaternion.Euler (0.0f, 0.0f, this.m_TargetSyncRotation2D), Time.fixedDeltaTime * this.interpolateRotation / this.GetNetworkSendInterval ()).eulerAngles.z);
        this.m_TargetSyncRotation2D += (float) ((double) this.m_TargetSyncAngularVelocity2D * (double) Time.fixedDeltaTime * 0.100000001490116);
      }
      this.m_TargetSyncPosition += this.m_TargetSyncVelocity * Time.fixedDeltaTime * 0.1f;
    }

    private void Update () {
      if (!initialized) return; // EDIT

      if (!this.hasAuthority || !this.localPlayerAuthority || (NetworkServer.active || (double) Time.time - (double) this.m_LastClientSendTime <= (double) this.GetNetworkSendInterval ()))
        return;
      this.SendTransform ();
      this.m_LastClientSendTime = Time.time;
    }

    private bool HasMoved () {
      if ((!((UnityEngine.Object) this.m_RigidBody3D != (UnityEngine.Object) null) ? (!((UnityEngine.Object) this.m_RigidBody2D != (UnityEngine.Object) null) ? (double) (this.transform.position - this.m_PrevPosition).magnitude : (double) (this.m_RigidBody2D.position - (Vector2) this.m_PrevPosition).magnitude) : (double) (this.m_RigidBody3D.position - this.m_PrevPosition).magnitude) > 9.99999974737875E-06)
        return true;
      float num = !((UnityEngine.Object) this.m_RigidBody3D != (UnityEngine.Object) null) ? (!((UnityEngine.Object) this.m_RigidBody2D != (UnityEngine.Object) null) ? Quaternion.Angle (this.transform.rotation, this.m_PrevRotation) : Math.Abs (this.m_RigidBody2D.rotation - this.m_PrevRotation2D)) : Quaternion.Angle (this.m_RigidBody3D.rotation, this.m_PrevRotation);
      if ((double) num > 9.99999974737875E-06)
        return true;
      if ((UnityEngine.Object) this.m_RigidBody3D != (UnityEngine.Object) null)
        num = Mathf.Abs (this.m_RigidBody3D.velocity.sqrMagnitude - this.m_PrevVelocity);
      else if ((UnityEngine.Object) this.m_RigidBody2D != (UnityEngine.Object) null)
        num = Mathf.Abs (this.m_RigidBody2D.velocity.sqrMagnitude - this.m_PrevVelocity);
      return (double) num > 9.99999974737875E-06;
    }

    [Client]
    private void SendTransform () {
      if (!this.HasMoved () || ClientScene.readyConnection == null)
        return;
      //this.m_LocalTransformWriter.StartMessage ((short) 6);
      this.m_LocalTransformWriter.StartMessage (MessageIds.CustomHandleTransform_CA); // EDIT
      this.m_LocalTransformWriter.Write (this.netId);
      switch (this.transformSyncMode) {
        case NetworkTransform_CA.TransformSyncMode.SyncNone:
          return;
        case NetworkTransform_CA.TransformSyncMode.SyncTransform:
          this.SerializeModeTransform (this.m_LocalTransformWriter);
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncRigidbody2D:
          this.SerializeMode2D (this.m_LocalTransformWriter);
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncRigidbody3D:
          this.SerializeMode3D (this.m_LocalTransformWriter);
          break;
        case NetworkTransform_CA.TransformSyncMode.SyncCharacterController:
          this.SerializeModeCharacterController (this.m_LocalTransformWriter);
          break;
      }
      if ((UnityEngine.Object) this.m_RigidBody3D != (UnityEngine.Object) null) {
        this.m_PrevPosition = this.m_RigidBody3D.position;
        this.m_PrevRotation = this.m_RigidBody3D.rotation;
        this.m_PrevVelocity = this.m_RigidBody3D.velocity.sqrMagnitude;
      } else if ((UnityEngine.Object) this.m_RigidBody2D != (UnityEngine.Object) null) {
        this.m_PrevPosition = (Vector3) this.m_RigidBody2D.position;
        this.m_PrevRotation2D = this.m_RigidBody2D.rotation;
        this.m_PrevVelocity = this.m_RigidBody2D.velocity.sqrMagnitude;
      } else {
        this.m_PrevPosition = this.transform.position;
        this.m_PrevRotation = this.transform.rotation;
      }
      this.m_LocalTransformWriter.FinishMessage ();
      //NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Outgoing, (short) 6, "6:LocalPlayerTransform", 1);
      ClientScene.readyConnection.SendWriter (this.m_LocalTransformWriter, this.GetNetworkChannel ());
    }

    public static void HandleTransform_CA (NetworkMessage netMsg) {
      NetworkInstanceId netId = netMsg.reader.ReadNetworkId ();
      //NetworkDetailStats.IncrementStat(NetworkDetailStats.NetworkDirection.Incoming, (short) 6, "6:LocalPlayerTransform", 1);
      GameObject localObject = NetworkServer.FindLocalObject (netId);
      if ((UnityEngine.Object) localObject == (UnityEngine.Object) null) {
        if (!LogFilter.logError)
          return;
        Debug.LogError ((object)
          "HandleTransform no gameObject");
      } else {
        NetworkTransform_CA component = localObject.GetComponent<NetworkTransform_CA> ();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null) {
          if (!LogFilter.logError)
            return;
          Debug.LogError ((object)
            "HandleTransform null target");
        } else if (!component.localPlayerAuthority) {
          if (!LogFilter.logError)
            return;
          Debug.LogError ((object)
            "HandleTransform no localPlayerAuthority");
        } else if (netMsg.conn.clientOwnedObjects == null) {
          if (!LogFilter.logError)
            return;
          Debug.LogError ((object)
            "HandleTransform object not owned by connection");
        } else if (netMsg.conn.clientOwnedObjects.Contains (netId)) {
          switch (component.transformSyncMode) {
            case NetworkTransform_CA.TransformSyncMode.SyncNone:
              return;
            case NetworkTransform_CA.TransformSyncMode.SyncTransform:
              component.UnserializeModeTransform (netMsg.reader, false);
              break;
            case NetworkTransform_CA.TransformSyncMode.SyncRigidbody2D:
              component.UnserializeMode2D (netMsg.reader, false);
              break;
            case NetworkTransform_CA.TransformSyncMode.SyncRigidbody3D:
              component.UnserializeMode3D (netMsg.reader, false);
              break;
            case NetworkTransform_CA.TransformSyncMode.SyncCharacterController:
              component.UnserializeModeCharacterController (netMsg.reader, false);
              break;
          }
          component.m_LastClientSyncTime = Time.time;
        } else {
          if (!LogFilter.logWarn)
            return;
          Debug.LogWarning ((object) ("HandleTransform netId:" + (object) netId + " is not for a valid player"));
        }
      }
    }

    private static void WriteAngle (NetworkWriter writer, float angle, NetworkTransform_CA.CompressionSyncMode compression) {
      switch (compression) {
        case NetworkTransform_CA.CompressionSyncMode.None:
          writer.Write (angle);
          break;
        case NetworkTransform_CA.CompressionSyncMode.Low:
          writer.Write ((short) angle);
          break;
        case NetworkTransform_CA.CompressionSyncMode.High:
          writer.Write ((short) angle);
          break;
      }
    }

    private static float ReadAngle (NetworkReader reader, NetworkTransform_CA.CompressionSyncMode compression) {
      switch (compression) {
        case NetworkTransform_CA.CompressionSyncMode.None:
          return reader.ReadSingle ();
        case NetworkTransform_CA.CompressionSyncMode.Low:
          return (float) reader.ReadInt16 ();
        case NetworkTransform_CA.CompressionSyncMode.High:
          return (float) reader.ReadInt16 ();
        default:
          return 0.0f;
      }
    }

    public static void SerializeVelocity3D (NetworkWriter writer, Vector3 velocity, NetworkTransform_CA.CompressionSyncMode compression) {
      writer.Write (velocity);
    }

    public static void SerializeVelocity2D (NetworkWriter writer, Vector2 velocity, NetworkTransform_CA.CompressionSyncMode compression) {
      writer.Write (velocity);
    }

    public static void SerializeRotation3D (NetworkWriter writer, Quaternion rot, NetworkTransform_CA.AxisSyncMode mode, NetworkTransform_CA.CompressionSyncMode compression) {
      switch (mode) {
        case NetworkTransform_CA.AxisSyncMode.AxisX:
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.x, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisY:
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.y, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisZ:
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.z, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXY:
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.x, compression);
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.y, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXZ:
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.x, compression);
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.z, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisYZ:
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.y, compression);
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.z, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXYZ:
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.x, compression);
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.y, compression);
          NetworkTransform_CA.WriteAngle (writer, rot.eulerAngles.z, compression);
          break;
      }
    }

    public static void SerializeRotation2D (NetworkWriter writer, float rot, NetworkTransform_CA.CompressionSyncMode compression) {
      NetworkTransform_CA.WriteAngle (writer, rot, compression);
    }

    public static void SerializeSpin3D (NetworkWriter writer, Vector3 angularVelocity, NetworkTransform_CA.AxisSyncMode mode, NetworkTransform_CA.CompressionSyncMode compression) {
      switch (mode) {
        case NetworkTransform_CA.AxisSyncMode.AxisX:
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.x, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisY:
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.y, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisZ:
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.z, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXY:
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.x, compression);
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.y, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXZ:
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.x, compression);
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.z, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisYZ:
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.y, compression);
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.z, compression);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXYZ:
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.x, compression);
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.y, compression);
          NetworkTransform_CA.WriteAngle (writer, angularVelocity.z, compression);
          break;
      }
    }

    public static void SerializeSpin2D (NetworkWriter writer, float angularVelocity, NetworkTransform_CA.CompressionSyncMode compression) {
      NetworkTransform_CA.WriteAngle (writer, angularVelocity, compression);
    }

    public static Vector3 UnserializeVelocity3D (NetworkReader reader, NetworkTransform_CA.CompressionSyncMode compression) {
      return reader.ReadVector3 ().ToLocalDirection (); // EDIT
    }

    public static Vector3 UnserializeVelocity2D (NetworkReader reader, NetworkTransform_CA.CompressionSyncMode compression) {
      return (Vector3) reader.ReadVector2 ();
    }

    public static Quaternion UnserializeRotation3D (NetworkReader reader, NetworkTransform_CA.AxisSyncMode mode, NetworkTransform_CA.CompressionSyncMode compression) {
      Quaternion identity = Quaternion.identity;
      Vector3 zero = Vector3.zero;
      switch (mode) {
        case NetworkTransform_CA.AxisSyncMode.AxisX:
          zero.Set (NetworkTransform_CA.ReadAngle (reader, compression), 0.0f, 0.0f);
          identity.eulerAngles = zero;
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisY:
          zero.Set (0.0f, NetworkTransform_CA.ReadAngle (reader, compression), 0.0f);
          identity.eulerAngles = zero;
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisZ:
          zero.Set (0.0f, 0.0f, NetworkTransform_CA.ReadAngle (reader, compression));
          identity.eulerAngles = zero;
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXY:
          zero.Set (NetworkTransform_CA.ReadAngle (reader, compression), NetworkTransform_CA.ReadAngle (reader, compression), 0.0f);
          identity.eulerAngles = zero;
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXZ:
          zero.Set (NetworkTransform_CA.ReadAngle (reader, compression), 0.0f, NetworkTransform_CA.ReadAngle (reader, compression));
          identity.eulerAngles = zero;
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisYZ:
          zero.Set (0.0f, NetworkTransform_CA.ReadAngle (reader, compression), NetworkTransform_CA.ReadAngle (reader, compression));
          identity.eulerAngles = zero;
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXYZ:
          zero.Set (NetworkTransform_CA.ReadAngle (reader, compression), NetworkTransform_CA.ReadAngle (reader, compression), NetworkTransform_CA.ReadAngle (reader, compression));
          identity.eulerAngles = zero;
          break;
      }
      return identity.ToLocal (); // EDIT
    }

    public static float UnserializeRotation2D (NetworkReader reader, NetworkTransform_CA.CompressionSyncMode compression) {
      return NetworkTransform_CA.ReadAngle (reader, compression);
    }

    public static Vector3 UnserializeSpin3D (NetworkReader reader, NetworkTransform_CA.AxisSyncMode mode, NetworkTransform_CA.CompressionSyncMode compression) {
      Vector3 zero = Vector3.zero;
      switch (mode) {
        case NetworkTransform_CA.AxisSyncMode.AxisX:
          zero.Set (NetworkTransform_CA.ReadAngle (reader, compression), 0.0f, 0.0f);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisY:
          zero.Set (0.0f, NetworkTransform_CA.ReadAngle (reader, compression), 0.0f);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisZ:
          zero.Set (0.0f, 0.0f, NetworkTransform_CA.ReadAngle (reader, compression));
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXY:
          zero.Set (NetworkTransform_CA.ReadAngle (reader, compression), NetworkTransform_CA.ReadAngle (reader, compression), 0.0f);
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXZ:
          zero.Set (NetworkTransform_CA.ReadAngle (reader, compression), 0.0f, NetworkTransform_CA.ReadAngle (reader, compression));
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisYZ:
          zero.Set (0.0f, NetworkTransform_CA.ReadAngle (reader, compression), NetworkTransform_CA.ReadAngle (reader, compression));
          break;
        case NetworkTransform_CA.AxisSyncMode.AxisXYZ:
          zero.Set (NetworkTransform_CA.ReadAngle (reader, compression), NetworkTransform_CA.ReadAngle (reader, compression), NetworkTransform_CA.ReadAngle (reader, compression));
          break;
      }
      return zero.ToLocalDirection (); // EDIT
    }

    public static float UnserializeSpin2D (NetworkReader reader, NetworkTransform_CA.CompressionSyncMode compression) {
      return NetworkTransform_CA.ReadAngle (reader, compression);
    }

    public override int GetNetworkChannel () {
      return 1;
    }

    public override float GetNetworkSendInterval () {
      return this.m_SendInterval;
    }

    public override void OnStartAuthority () {
      this.m_LastClientSyncTime = 0.0f;
    }

    /// <summary>
    ///   <para>How to synchronize an object's position.</para>
    /// </summary>
    public enum TransformSyncMode {
      SyncNone,
      SyncTransform,
      SyncRigidbody2D,
      SyncRigidbody3D,
      SyncAgent,
      SyncCharacterController,
    }

    /// <summary>
    ///   <para>An axis or set of axis.</para>
    /// </summary>
    public enum AxisSyncMode {
      None,
      AxisX,
      AxisY,
      AxisZ,
      AxisXY,
      AxisXZ,
      AxisYZ,
      AxisXYZ,
    }

    /// <summary>
    ///   <para>How much to compress sync data.</para>
    /// </summary>
    public enum CompressionSyncMode {
      None,
      Low,
      High,
    }

    /// <summary>
    ///   <para>This is the delegate use for the movement validation callback function clientMoveCallback3D on NetworkTransforms.</para>
    /// </summary>
    /// <param name="position">The new position from the client.</param>
    /// <param name="velocity">The new velocity from the client.</param>
    /// <param name="rotation">The new rotation from the client.</param>
    public delegate bool ClientMoveCallback3D (ref Vector3 position, ref Vector3 velocity, ref Quaternion rotation);

    /// <summary>
    ///   <para>This is the delegate use for the movement validation callback function clientMoveCallback2D on NetworkTransforms.</para>
    /// </summary>
    /// <param name="position">The new position from the client.</param>
    /// <param name="velocity">The new velocity from the client.</param>
    /// <param name="rotation">The new rotation from the client.</param>
    public delegate bool ClientMoveCallback2D (ref Vector2 position, ref Vector2 velocity, ref float rotation);
  }
}