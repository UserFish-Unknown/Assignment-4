using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-100)]
public class BedRoutine : MonoBehaviour
{
    [Header("Markers (empty objects)")]
    public Transform bedPoint;
    public Transform standPoint;     // open floor beside the bed: where you end up standing
    public Transform lyingView;      // where your EYES are when lying in bed
    public Transform faceTarget;     // optional: you face this when standing (default: world origin)

    [Header("Sound")]
    public AudioClip shatterClip;
    public Transform soundPosition;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Behaviour")]
    public float sleepRange = 3f;
    public KeyCode sleepKey = KeyCode.E;
    public bool sleepEnabled = true;
    public bool pausePlayerScripts = true;
    public UnityEvent onSleepEnd;

    [Header("Lying pose")]
    public Vector3 lyingCamEuler = new Vector3(-75f, 0f, 0f);
    public float lyingEyeHeight = 0.4f;   // only used if Lying View is empty

    enum State { Waking, LyingWaiting, Standing, Busy, Asleep }
    State state = State.Waking;

    Transform root;
    Camera cam;
    CharacterController controller;
    CapsuleCollider capsule;
    Rigidbody rb;
    bool rbWasKinematic;
    readonly List<MonoBehaviour> frozen = new List<MonoBehaviour>();

    Renderer[] bodyRenderers;
    bool[] rendererWas;

    Transform camParent;
    Vector3 camLocalPos, camLocalScale;
    Quaternion camLocalRot;

    Vector3 lyingEye, standFeet;
    Quaternion lyingRot;
    float lyingYaw, standYaw;

    float blackAlpha = 1f;
    string hintText = "";
    GUIStyle style;

    // ---------- setup ----------
    void Awake()
    {
        if (bedPoint == null || standPoint == null)
        {
            Debug.LogError("BedRoutine: assign Bed Point and Stand Point.", this);
            enabled = false; return;
        }

        GameObject p = GameObject.Find("Player&Camera");
        if (p == null) p = GameObject.FindWithTag("Player");
        if (p == null && Camera.main != null) p = Camera.main.transform.root.gameObject;
        if (p == null) { Debug.LogError("BedRoutine: could not find the player.", this); enabled = false; return; }

        root = p.transform;
        cam = p.GetComponentInChildren<Camera>();
        if (cam == null) cam = Camera.main;
        if (cam == null) { Debug.LogError("BedRoutine: no camera found.", this); enabled = false; return; }

        controller = p.GetComponentInChildren<CharacterController>();
        capsule = p.GetComponentInChildren<CapsuleCollider>();
        rb = p.GetComponentInChildren<Rigidbody>();

        bodyRenderers = p.GetComponentsInChildren<Renderer>(true);
        rendererWas = new bool[bodyRenderers.Length];
        for (int i = 0; i < bodyRenderers.Length; i++) rendererWas[i] = bodyRenderers[i].enabled;

        if (lyingView != null) { lyingEye = lyingView.position; lyingYaw = lyingView.eulerAngles.y; }
        else { lyingEye = bedPoint.position + Vector3.up * lyingEyeHeight; lyingYaw = bedPoint.eulerAngles.y; }
        lyingRot = Quaternion.Euler(lyingCamEuler.x, lyingYaw, lyingCamEuler.z);

        Vector3 look = (faceTarget != null ? faceTarget.position : Vector3.zero) - standPoint.position;
        look.y = 0f;
        standYaw = look.sqrMagnitude > 0.01f
            ? Quaternion.LookRotation(look).eulerAngles.y
            : standPoint.eulerAngles.y;

        if (pausePlayerScripts)
        {
            foreach (var m in p.GetComponentsInChildren<MonoBehaviour>())
            {
                if (m == null || !m.enabled) continue;
                string ns = m.GetType().Namespace;
                if (ns != null && (ns.StartsWith("Unity") || ns.StartsWith("Cinemachine"))) continue;
                frozen.Add(m);
            }
        }

        Debug.Log("BedRoutine OK: root=" + root.name + " camera=" + cam.name +
                  " controller=" + (controller != null) + " capsule=" + (capsule != null) +
                  " pausedScripts=" + frozen.Count);

        Freeze();

        // Put the BODY once, standing on the real floor, facing the room
        standFeet = GroundAt(standPoint.position) + Vector3.up * 0.02f;
        PlaceFeet(standFeet, standYaw);
        Physics.SyncTransforms();

        // Detach ONLY the camera and put it in bed
        DetachCamera();
        cam.transform.SetPositionAndRotation(lyingEye, lyingRot);

        StartCoroutine(WakeIntro());
    }

    // ---------- camera detach / reattach ----------
    void DetachCamera()
    {
        camParent = cam.transform.parent;
        camLocalPos = cam.transform.localPosition;
        camLocalRot = cam.transform.localRotation;
        camLocalScale = cam.transform.localScale;
        cam.transform.SetParent(null, true);
        HideBody(true);
    }

    void ReattachCamera()
    {
        cam.transform.SetParent(camParent, false);
        cam.transform.localPosition = camLocalPos;
        cam.transform.localRotation = camLocalRot;
        cam.transform.localScale = camLocalScale;
        HideBody(false);
    }

    void HideBody(bool hide)
    {
        if (bodyRenderers == null) return;
        for (int i = 0; i < bodyRenderers.Length; i++)
            if (bodyRenderers[i] != null) bodyRenderers[i].enabled = hide ? false : rendererWas[i];
    }

    // ---------- measuring / placing the body ----------
    Vector3 FeetWorld()
    {
        if (controller != null)
        {
            Transform t = controller.transform;
            Vector3 c = t.TransformPoint(controller.center);
            return c + Vector3.down * (controller.height * 0.5f * Mathf.Abs(t.lossyScale.y));
        }
        if (capsule != null)
        {
            Transform t = capsule.transform;
            Vector3 c = t.TransformPoint(capsule.center);
            return c + Vector3.down * (capsule.height * 0.5f * Mathf.Abs(t.lossyScale.y));
        }
        return root.position;
    }

    float FacingYaw()
    {
        return cam.transform.IsChildOf(root) ? cam.transform.eulerAngles.y : root.eulerAngles.y;
    }

    void PlaceFeet(Vector3 feet, float yaw)
    {
        float d = Mathf.DeltaAngle(FacingYaw(), yaw);
        root.RotateAround(FeetWorld(), Vector3.up, d);
        root.position += feet - FeetWorld();
    }

    Vector3 GroundAt(Vector3 p)
    {
        var hits = Physics.RaycastAll(p + Vector3.up * 1f, Vector3.down, 4f, ~0, QueryTriggerInteraction.Ignore);
        float best = float.MaxValue;
        Vector3 point = p;
        foreach (var h in hits)
        {
            if (h.transform.IsChildOf(root)) continue;
            if (h.distance < best) { best = h.distance; point = h.point; }
        }
        return point;
    }

    // ---------- flow ----------
    void Update()
    {
        if (state == State.LyingWaiting)
        {
            hintText = "Press W A S D to get up";
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f ||
                Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f)
                StartCoroutine(StandUp());
        }
        else if (state == State.Standing)
        {
            Vector3 d = FeetWorld() - bedPoint.position; d.y = 0f;
            bool near = sleepEnabled && d.magnitude <= sleepRange;
            hintText = near ? "Press E to lie down" : "";
            if (Input.GetKeyDown(sleepKey))
                Debug.Log("E pressed. near=" + near + " distance=" + d.magnitude.ToString("F2"));
            if (near && Input.GetKeyDown(sleepKey)) StartCoroutine(LieDown());
        }
    }

    IEnumerator WakeIntro()
    {
        state = State.Waking;
        yield return Settle();                 // body settles onto the floor while hidden and paused
        yield return new WaitForSeconds(1f);
        yield return Fade(0f, 2.5f);
        state = State.LyingWaiting;
    }

    IEnumerator Settle()
    {
        if (controller == null) yield break;
        Physics.SyncTransforms();
        controller.enabled = true;
        for (int i = 0; i < 30; i++)
        {
            controller.Move(Vector3.down * 0.05f);
            if (controller.isGrounded) break;
            yield return null;
        }
        standFeet = FeetWorld();
    }

    IEnumerator StandUp()
    {
        state = State.Busy;
        hintText = "";

        // The camera's own original pose = exactly where the player's eyes belong
        Vector3 endPos = camParent != null ? camParent.TransformPoint(camLocalPos) : camLocalPos;
        Quaternion endRot = camParent != null ? camParent.rotation * camLocalRot : camLocalRot;

        yield return MoveCam(cam.transform.position, cam.transform.rotation, endPos, endRot, 2.5f, 1.6f);

        ReattachCamera();
        Unfreeze();
        Debug.Log("Stand done. feet=" + FeetWorld().ToString("F2") + " eye=" + cam.transform.position.ToString("F2"));
        state = State.Standing;
        StartCoroutine(AnchorGuard(standFeet));
    }

    IEnumerator LieDown()
    {
        state = State.Busy;
        hintText = "";
        Freeze();
        DetachCamera();

        yield return MoveCam(cam.transform.position, cam.transform.rotation, lyingEye, lyingRot, 2f, 1f);

        yield return new WaitForSeconds(0.5f);
        yield return Fade(1f, 3f);
        yield return new WaitForSeconds(1.5f);

        PlayShatter();
        yield return new WaitForSeconds(4f);

        state = State.Asleep;
        onSleepEnd.Invoke();
    }

    // Moves ONLY the camera, along one smooth path
    IEnumerator MoveCam(Vector3 p0, Quaternion r0, Vector3 p1, Quaternion r1, float dur, float rotLead)
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime / dur)
        {
            float s = Mathf.SmoothStep(0f, 1f, t);
            float sr = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t * rotLead));
            cam.transform.SetPositionAndRotation(Vector3.Lerp(p0, p1, s), Quaternion.Slerp(r0, r1, sr));
            yield return null;
        }
        cam.transform.SetPositionAndRotation(p1, r1);
    }

    // If a shared script moves the player right after handoff, put them back
    IEnumerator AnchorGuard(Vector3 target)
    {
        float end = Time.time + 0.7f;
        while (Time.time < end)
        {
            yield return null;
            Vector3 f = FeetWorld();
            Vector3 flat = f - target; flat.y = 0f;
            if (flat.magnitude > 1.5f || f.y < target.y - 1f)
            {
                Debug.LogWarning("Player was moved to " + f + " right after standing. Putting back.");
                if (controller != null) controller.enabled = false;
                PlaceFeet(target, standYaw);
                Physics.SyncTransforms();
                if (controller != null) controller.enabled = true;
            }
        }
    }

    // ---------- freeze / sound / fade ----------
    void Freeze()
    {
        foreach (var m in frozen) if (m != null) m.enabled = false;
        if (controller != null) controller.enabled = false;
        if (rb != null) { rbWasKinematic = rb.isKinematic; rb.isKinematic = true; }
    }

    void Unfreeze()
    {
        Physics.SyncTransforms();
        if (controller != null) controller.enabled = true;
        if (rb != null) rb.isKinematic = rbWasKinematic;
        foreach (var m in frozen) if (m != null) m.enabled = true;
    }

    void PlayShatter()
    {
        if (shatterClip == null) { Debug.LogWarning("BedRoutine: no Shatter Clip assigned."); return; }
        var go = new GameObject("ShatterSFX");
        go.transform.position = soundPosition != null ? soundPosition.position : cam.transform.position;
        var src = go.AddComponent<AudioSource>();
        src.clip = shatterClip;
        src.volume = volume;
        src.spatialBlend = 1f;
        src.minDistance = 10f;
        src.maxDistance = 50f;
        src.Play();
        Destroy(go, shatterClip.length + 0.5f);
    }

    IEnumerator Fade(float target, float dur)
    {
        float start = blackAlpha;
        for (float t = 0f; t < 1f; t += Time.deltaTime / dur)
        {
            blackAlpha = Mathf.Lerp(start, target, t);
            yield return null;
        }
        blackAlpha = target;
    }

    // ---------- editor helpers ----------
    void OnDrawGizmos()
    {
        if (bedPoint != null) { Gizmos.color = Color.green; Gizmos.DrawWireSphere(bedPoint.position, 0.2f); }
        if (standPoint != null) { Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(standPoint.position, 0.2f); }
        if (lyingView != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lyingView.position, 0.15f);
            Gizmos.DrawRay(lyingView.position, lyingView.forward * 0.5f);
        }
        if (faceTarget != null) { Gizmos.color = Color.white; Gizmos.DrawWireSphere(faceTarget.position, 0.15f); }
    }

    void OnDrawGizmosSelected()
    {
        if (bedPoint == null) return;
        Gizmos.color = Color.cyan;
        Vector3 c = bedPoint.position;
        Vector3 prev = c + new Vector3(sleepRange, 0f, 0f);
        for (int i = 1; i <= 48; i++)
        {
            float a = i * Mathf.PI * 2f / 48f;
            Vector3 next = c + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * sleepRange;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    void OnGUI()
    {
        GUI.depth = -1000;
        if (blackAlpha > 0.001f)
        {
            GUI.color = new Color(0f, 0f, 0f, blackAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
        if (!string.IsNullOrEmpty(hintText) && blackAlpha < 0.5f)
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label)
                { alignment = TextAnchor.MiddleCenter, fontSize = 28, fontStyle = FontStyle.Bold };
                style.normal.textColor = Color.white;
            }
            GUI.color = Color.white;
            GUI.Label(new Rect(0, Screen.height - 120, Screen.width, 60), hintText, style);
        }
    }
}