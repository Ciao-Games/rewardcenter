using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CiaoGames.RewardCenter.UI
{
    /// <summary>
    /// Pure uGUI Canvas-based confetti particle system for UI popups.
    /// Operates natively inside any Canvas (Screen Space Overlay, Screen Space Camera, or World Space).
    /// Fully cross-platform and pipeline-agnostic (works identically in Built-in Render Pipeline, URP, HDRP, Unity 2022/2023/Unity 6+).
    /// Requires zero special shaders or particle renderers.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class ConfettiParticleUI : MaskableGraphic
    {
        [System.Serializable]
        public struct ShapeUV
        {
            public Vector2 uvMin;
            public Vector2 uvMax;
        }

        private struct Particle
        {
            public Vector2 position;
            public Vector2 velocity;
            public float rotation;
            public float angularVelocity;
            public Vector2 size;
            public Color color;
            public float lifeTime;
            public float maxLifeTime;
            public int shapeIndex;
            public float flipProgress;
            public float flipSpeed;
        }

        [Header("Confetti Configuration")]
        [SerializeField] private int particleCount = 100;
        [SerializeField] private float duration = 3.5f;
        [SerializeField] private Vector2 particleSizeMin = new Vector2(16f, 24f);
        [SerializeField] private Vector2 particleSizeMax = new Vector2(24f, 36f);
        [SerializeField] private float gravity = 850f;
        [SerializeField] private float airResistance = 0.985f;
        [SerializeField] private bool autoPlayOnEnable = true;

        [Header("Colors")]
        [SerializeField] private Color[] vibrantColors = new Color[]
        {
            new Color(1f, 0.84f, 0f, 1f),       // Vivid Gold
            new Color(1f, 0.25f, 0.55f, 1f),    // Hot Pink
            new Color(0f, 0.82f, 1f, 1f),       // Bright Cyan
            new Color(0.3f, 0.95f, 0.35f, 1f),  // Vibrant Lime Green
            new Color(1f, 0.55f, 0f, 1f),       // Warm Orange
            new Color(0.7f, 0.3f, 1f, 1f),      // Royal Purple
            new Color(1f, 0.95f, 0.3f, 1f)      // Sunshine Yellow
        };

        private List<Particle> particles = new List<Particle>();
        private bool isPlaying = false;
        private float playTimer = 0f;

        [SerializeField] private ShapeUV[] shapeUVs = new ShapeUV[]
        {
            // Atlas 2x2:
            // 0: Top-Left (Paper Rectangle)
            new ShapeUV { uvMin = new Vector2(0f, 0.5f), uvMax = new Vector2(0.5f, 1f) },
            // 1: Top-Right (Circle / Dot)
            new ShapeUV { uvMin = new Vector2(0.5f, 0.5f), uvMax = new Vector2(1f, 1f) },
            // 2: Bottom-Left (Ribbon / Strip)
            new ShapeUV { uvMin = new Vector2(0f, 0f), uvMax = new Vector2(0.5f, 0.5f) },
            // 3: Bottom-Right (5-Point Star)
            new ShapeUV { uvMin = new Vector2(0.5f, 0f), uvMax = new Vector2(1f, 0.5f) }
        };

        [SerializeField] private Sprite confettiAtlasSprite;

        public override Texture mainTexture
        {
            get
            {
                if (confettiAtlasSprite != null && confettiAtlasSprite.texture != null)
                {
                    return confettiAtlasSprite.texture;
                }
                return s_WhiteTexture;
            }
        }

        public Sprite ConfettiAtlasSprite
        {
            get => confettiAtlasSprite;
            set
            {
                if (confettiAtlasSprite != value)
                {
                    confettiAtlasSprite = value;
                    SetMaterialDirty();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            raycastTarget = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (autoPlayOnEnable)
            {
                Play();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Stop();
        }

        /// <summary>
        /// Starts the confetti celebration burst.
        /// </summary>
        public void Play()
        {
            gameObject.SetActive(true);
            isPlaying = true;
            playTimer = 0f;
            SpawnParticles();
            SetVerticesDirty();
        }

        /// <summary>
        /// Stops and clears the confetti.
        /// </summary>
        public void Stop()
        {
            isPlaying = false;
            particles.Clear();
            SetVerticesDirty();
        }

        private void SpawnParticles()
        {
            particles.Clear();
            Rect r = rectTransform.rect;
            float w = r.width;
            float h = r.height;

            if (w <= 0 || h <= 0)
            {
                w = 800f;
                h = 1000f;
            }
            
            Vector2 topCenterOrigin = new Vector2(0f, 0f);

            int perOrigin = particleCount / 3;
            
            int remainder = particleCount - (perOrigin * 2);
            for (int i = 0; i < remainder; i++)
            {
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float speed = Random.Range(200f, 750f);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                AddSingleParticle(topCenterOrigin + new Vector2(Random.Range(-50f, 50f), Random.Range(-30f, 30f)), vel);
            }
        }

        private void AddSingleParticle(Vector2 pos, Vector2 vel)
        {
            Particle p = new Particle();
            p.position = pos;
            p.velocity = vel;
            p.rotation = Random.Range(0f, 360f);
            p.angularVelocity = Random.Range(-360f, 360f);
            p.size = new Vector2(
                Random.Range(particleSizeMin.x, particleSizeMax.x),
                Random.Range(particleSizeMin.y, particleSizeMax.y)
            );

            Color baseColor = (vibrantColors != null && vibrantColors.Length > 0)
                ? vibrantColors[Random.Range(0, vibrantColors.Length)]
                : Color.white;
            p.color = baseColor;

            p.maxLifeTime = Random.Range(duration * 0.75f, duration * 1.25f);
            p.lifeTime = 0f;
            p.shapeIndex = (shapeUVs != null && shapeUVs.Length > 0) ? Random.Range(0, shapeUVs.Length) : 0;
            p.flipProgress = Random.Range(0f, Mathf.PI * 2f);
            p.flipSpeed = Random.Range(4f, 10f);

            particles.Add(p);
        }

        private void Update()
        {
            if (!isPlaying || particles.Count == 0) return;

            float dt = Time.unscaledDeltaTime;
            playTimer += dt;

            bool anyAlive = false;

            for (int i = 0; i < particles.Count; i++)
            {
                Particle p = particles[i];
                p.lifeTime += dt;

                if (p.lifeTime < p.maxLifeTime)
                {
                    anyAlive = true;

                    p.velocity.y -= gravity * dt;
                    p.velocity *= Mathf.Pow(airResistance, dt * 60f);

                    // Flutter effect: horizontal wobble
                    float wobble = Mathf.Sin(p.lifeTime * 8f + i) * 60f * dt;
                    p.velocity.x += wobble;

                    p.position += p.velocity * dt;
                    p.rotation += p.angularVelocity * dt;
                    p.flipProgress += p.flipSpeed * dt;

                    particles[i] = p;
                }
            }

            SetVerticesDirty();

            if (!anyAlive && playTimer >= duration)
            {
                isPlaying = false;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (!isPlaying || particles.Count == 0) return;

            for (int i = 0; i < particles.Count; i++)
            {
                Particle p = particles[i];
                if (p.lifeTime >= p.maxLifeTime) continue;

                float lifeRatio = p.lifeTime / p.maxLifeTime;
                float alpha = 1f;
                if (lifeRatio > 0.7f)
                {
                    alpha = Mathf.Clamp01(1f - ((lifeRatio - 0.7f) / 0.3f));
                }

                Color c = p.color;
                c.a *= alpha;

                // 3D paper tumbling scale on X
                float flipScale = Mathf.Cos(p.flipProgress);
                float rad = p.rotation * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                float halfW = (p.size.x * 0.5f) * flipScale;
                float halfH = p.size.y * 0.5f;

                Vector2 cornerTL = new Vector2(-halfW, halfH);
                Vector2 cornerTR = new Vector2(halfW, halfH);
                Vector2 cornerBR = new Vector2(halfW, -halfH);
                Vector2 cornerBL = new Vector2(-halfW, -halfH);

                Vector2 Rotate(Vector2 v)
                {
                    return new Vector2(
                        v.x * cos - v.y * sin,
                        v.x * sin + v.y * cos
                    ) + p.position;
                }

                Vector2 v0 = Rotate(cornerBL);
                Vector2 v1 = Rotate(cornerTL);
                Vector2 v2 = Rotate(cornerTR);
                Vector2 v3 = Rotate(cornerBR);

                Vector2 uv0 = Vector2.zero;
                Vector2 uv1 = new Vector2(0f, 1f);
                Vector2 uv2 = Vector2.one;
                Vector2 uv3 = new Vector2(1f, 0f);

                if (shapeUVs != null && shapeUVs.Length > 0 && p.shapeIndex >= 0 && p.shapeIndex < shapeUVs.Length)
                {
                    ShapeUV suv = shapeUVs[p.shapeIndex];
                    uv0 = new Vector2(suv.uvMin.x, suv.uvMin.y);
                    uv1 = new Vector2(suv.uvMin.x, suv.uvMax.y);
                    uv2 = new Vector2(suv.uvMax.x, suv.uvMax.y);
                    uv3 = new Vector2(suv.uvMax.x, suv.uvMin.y);
                }

                int baseIndex = vh.currentVertCount;
                vh.AddVert(new Vector3(v0.x, v0.y, 0f), c, uv0);
                vh.AddVert(new Vector3(v1.x, v1.y, 0f), c, uv1);
                vh.AddVert(new Vector3(v2.x, v2.y, 0f), c, uv2);
                vh.AddVert(new Vector3(v3.x, v3.y, 0f), c, uv3);

                vh.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
                vh.AddTriangle(baseIndex + 2, baseIndex + 3, baseIndex);
            }
        }
    }
}
