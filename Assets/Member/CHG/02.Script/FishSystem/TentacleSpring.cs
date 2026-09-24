using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    [DisallowMultipleComponent]
    public class TentacleSpring : MonoBehaviour
    {
        [Header("Bones (뿌리 -> 끝 순서)")]
        [SerializeField] Transform[] bones;

        [Header("스프링")]
        [Tooltip("기본 포즈로 돌아가려는 힘. 낮을수록 흐물흐물합니다.")]
        [Range(0f, 1f)] public float stiffness = 0.14f;

        [Tooltip("에너지 감쇠. 높을수록 빨리 멎습니다.")]
        [Range(0f, 1f)] public float damping = 0.22f;

        [Tooltip("1이면 본체 이동을 온전히 관성으로 느낍니다. 0이면 딱 붙어서 따라갑니다.")]
        [Range(0f, 1f)] public float inertia = 1f;

        [Tooltip("끝으로 갈수록 얼마나 더 흐물거릴지. 이게 채찍 같은 파동을 만듭니다.")]
        [Range(0f, 1f)] public float tipLooseness = 0.6f;

        [Header("외력")]
        [Tooltip("수중이면 -0.3 ~ -0.6 정도가 자연스럽습니다.")]
        public Vector3 gravity = new Vector3(0f, -0.4f, 0f);

        [Tooltip("물살. 월드 공간 방향입니다.")]
        public Vector3 currentFlow = Vector3.zero;

        [Tooltip("잔잔한 불규칙 흔들림. 0이면 꺼집니다.")]
        public float noiseAmount = 0.12f;
        public float noiseSpeed = 0.6f;

        [Header("Curl (기본 포즈 구동)")]
        [Tooltip("양수면 말리고 음수면 펴집니다. 애니메이션하거나 코드로 바꾸세요.")]
        public float curlDegrees = 0f;

        [Tooltip("본의 로컬 축. 말리는 방향이 이상하면 (0,0,1) / (0,1,0)으로 바꿔보세요.")]
        public Vector3 curlAxis = Vector3.right;

        [Tooltip("1에 가까울수록 끝쪽만 말립니다.")]
        [Range(0f, 1f)] public float curlTipBias = 0.7f;

        [Header("안전장치")]
        [Tooltip("한 프레임에 이 거리 이상 이동하면 텔레포트로 보고 시뮬레이션을 리셋합니다.")]
        public float teleportThreshold = 2f;

        /// <summary>
        /// 본별 추가 각도(도). TentacleWhip 같은 외부 컴포넌트가 여기에 값을 써 넣으면
        /// Curl Degrees 위에 더해집니다. 길이는 Bones 배열과 같아야 합니다.
        /// </summary>
        [System.NonSerialized] public float[] externalDrive;

        public int BoneCount => bones != null ? bones.Length : 0;
        public Vector3 CurlAxis => curlAxis;

        Quaternion[] baseLocal;
        Vector3[] tipPos, prevTipPos, localTipDir;
        float[] lengths;
        Vector3 prevOwnerPos;
        float noiseSeed;
        bool ready;

        void Awake() { Initialize(); }

        void OnEnable() { if (ready) Snap(); }

        void Initialize()
        {
            int n = bones != null ? bones.Length : 0;
            if (n < 2) { enabled = false; return; }

            baseLocal = new Quaternion[n];
            tipPos = new Vector3[n];
            prevTipPos = new Vector3[n];
            localTipDir = new Vector3[n];
            lengths = new float[n];

            for (int i = 0; i < n; i++)
            {
                if (!bones[i]) { enabled = false; return; }
                baseLocal[i] = bones[i].localRotation;
            }

            for (int i = 0; i < n; i++)
            {
                Vector3 tip;
                if (i < n - 1)
                {
                    tip = bones[i + 1].position;
                }
                else
                {
                    // 마지막 본은 자식이 없으므로 직전 구간의 방향과 길이를 이어서 씁니다.
                    Vector3 dir = (bones[i].position - bones[i - 1].position).normalized;
                    tip = bones[i].position + dir * lengths[i - 1];
                }

                lengths[i] = Vector3.Distance(bones[i].position, tip);
                if (lengths[i] < 1e-5f) lengths[i] = 0.01f;

                localTipDir[i] = bones[i].InverseTransformDirection((tip - bones[i].position).normalized);
                tipPos[i] = prevTipPos[i] = tip;
            }

            prevOwnerPos = transform.position;
            noiseSeed = Random.value * 100f;
            ready = true;
        }

        void LateUpdate()
        {
            if (!ready) return;

            int n = bones.Length;
            float dt = Mathf.Min(Time.deltaTime, 1f / 30f);
            if (dt <= 0f) return;

            Vector3 ownerDelta = transform.position - prevOwnerPos;
            prevOwnerPos = transform.position;

            if (ownerDelta.magnitude > teleportThreshold) { Snap(); return; }

            // (1) Curl + 외부 드라이브로 기본 포즈를 세웁니다.
            bool hasDrive = externalDrive != null && externalDrive.Length == n;
            for (int i = 0; i < n; i++)
            {
                float k = n > 1 ? i / (float)(n - 1) : 0f;
                float w = Mathf.Lerp(1f - curlTipBias, 1f, k);
                float angle = curlDegrees * w + (hasDrive ? externalDrive[i] : 0f);
                bones[i].localRotation = baseLocal[i] * Quaternion.AngleAxis(angle, curlAxis);
            }

            // (2) 그 위에 스프링을 얹습니다. 반드시 뿌리 -> 끝 순서여야 합니다.
            float t = Time.time * noiseSpeed;
            float follow = 1f - inertia;

            for (int i = 0; i < n; i++)
            {
                Transform b = bones[i];

                tipPos[i] += ownerDelta * follow;
                prevTipPos[i] += ownerDelta * follow;

                float k = n > 1 ? i / (float)(n - 1) : 0f;
                float local = stiffness * Mathf.Lerp(1f, 1f - tipLooseness, k);

                Vector3 restDir = b.TransformDirection(localTipDir[i]);
                Vector3 restTip = b.position + restDir * lengths[i];

                Vector3 vel = (tipPos[i] - prevTipPos[i]) * (1f - damping);

                Vector3 force = gravity + currentFlow;
                if (noiseAmount > 0f)
                {
                    float nx = Mathf.PerlinNoise(noiseSeed + t, i * 0.37f) - 0.5f;
                    float ny = Mathf.PerlinNoise(noiseSeed + t + 31.7f, i * 0.37f) - 0.5f;
                    float nz = Mathf.PerlinNoise(noiseSeed + t + 71.3f, i * 0.37f) - 0.5f;
                    force += new Vector3(nx, ny, nz) * noiseAmount * 2f;
                }

                prevTipPos[i] = tipPos[i];

                Vector3 next = tipPos[i] + vel + force * (dt * dt * 60f);

                // 기본 포즈로 당기기
                next = Vector3.Lerp(next, restTip, local);

                // 본 길이 유지
                Vector3 d = next - b.position;
                if (d.sqrMagnitude < 1e-10f) d = restDir;
                next = b.position + d.normalized * lengths[i];

                // 본을 파티클 쪽으로 겨눕니다. 자식 본들은 이 회전을 따라 함께 움직입니다.
                b.rotation = Quaternion.FromToRotation(restDir, (next - b.position).normalized) * b.rotation;

                tipPos[i] = next;
            }
        }

        /// <summary>시뮬레이션을 현재 포즈로 즉시 정착시킵니다. 텔레포트 직후에 호출하세요.</summary>
        public void Snap()
        {
            if (!ready) return;

            int n = bones.Length;
            bool hasDrive = externalDrive != null && externalDrive.Length == n;
            for (int i = 0; i < n; i++)
            {
                float k = n > 1 ? i / (float)(n - 1) : 0f;
                float w = Mathf.Lerp(1f - curlTipBias, 1f, k);
                float angle = curlDegrees * w + (hasDrive ? externalDrive[i] : 0f);
                bones[i].localRotation = baseLocal[i] * Quaternion.AngleAxis(angle, curlAxis);
            }
            for (int i = 0; i < n; i++)
            {
                Vector3 tip = bones[i].position + bones[i].TransformDirection(localTipDir[i]) * lengths[i];
                tipPos[i] = prevTipPos[i] = tip;
            }
            prevOwnerPos = transform.position;
        }

        void OnDrawGizmosSelected()
        {
            if (bones == null) return;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < bones.Length - 1; i++)
                if (bones[i] && bones[i + 1])
                    Gizmos.DrawLine(bones[i].position, bones[i + 1].position);
        }
    }
}