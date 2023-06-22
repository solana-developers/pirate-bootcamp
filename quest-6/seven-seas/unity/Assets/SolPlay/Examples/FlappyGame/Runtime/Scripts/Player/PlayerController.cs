using System;
using System.Collections;
using Solana.Unity.SDK.Nft;
#if GLTFAST
using GLTFast;
#endif
using SolPlay.Scripts;
using UnityEngine;

namespace SolPlay.FlappyGame.Runtime.Scripts.Player
{
    [RequireComponent(typeof(PlayerAudio))]
    [RequireComponent(typeof(PlayerInputs))]
    public class PlayerController : MonoBehaviour
    {
        public enum NftType
        {
            Water,
            Default
        }
    
        [SerializeField] PlayerInputs _input;
        [SerializeField] PlayerAudio _audio;
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] ParticleSystem _fireParticles;
        [SerializeField] ParticleSystem _waterTrail;
        [SerializeField] ParticleSystem _waterSplash;
        [SerializeField] GameObject _tradeGraphParticles;
        public PlayerParameters MovementParameters;
        public NftType Type = NftType.Default;
        // 3D character
#if GLTFAST
        public GltfAsset GltfAsset;
#endif
        public BoxCollider2D TargetBounds;
    
        Vector3 _velocity;
        float _speedIncrease;
        Vector3 _rotation;

        private bool _isDead;
        private ParticleSystem.MinMaxCurve _initialParticleEmission;
        private Coroutine _fireDisableCoroutine;
        public Vector3 Velocity { get => _velocity; }

        public async void SetSpriteFromNft(Nft nft)
        {
            if (!string.IsNullOrEmpty(nft.metaplexData.data.offchainData.animation_url))
            {
                _spriteRenderer.gameObject.SetActive(false);
#if GLTFAST
                foreach (Transform trans in GltfAsset.transform)
                {
                    Destroy(trans.gameObject);
                }
                GltfAsset.gameObject.SetActive(true);

                var loaded = await GltfAsset.Load(nft.MetaplexData.data.json.animation_url);
                if (loaded)
                {
                    StartCoroutine(ScaleLoadedAssetToCorrectSize());   
                }
#endif
            }
            else
            {
                _spriteRenderer.gameObject.SetActive(true);
#if GLTFAST
                GltfAsset.gameObject.SetActive(false);
#endif
                var rect = new Rect(0, 0, nft.metaplexData.nftImage.file.width, nft.metaplexData.nftImage.file.height);
                _spriteRenderer.sprite = Sprite.Create(nft.metaplexData.nftImage.file, rect, new Vector2(0.5f, 0.5f));
            }
        }

#if GLTFAST
        private IEnumerator ScaleLoadedAssetToCorrectSize()
        {
            yield return new WaitForEndOfFrame();
            if (GltfAsset.transform.childCount == 0)
            {
                yield break;
            }

            var gltfScene = GltfAsset.transform.GetChild(0);
            var currentBounds = UnityUtils.GetBoundsWithChildren(gltfScene.gameObject);
            bool stillTooBigOrSmall = true;
            var sizeY = TargetBounds.bounds.size.y / currentBounds.size.y;
            gltfScene.gameObject.transform.localScale = new Vector3(sizeY, sizeY, sizeY);
        }
#endif

        public void Die()
        {
            _isDead = true;
            _velocity = Vector3.zero;
            _audio.OnDie();
            CameraShake.Shake(0.1f, 0.2f);
            _tradeGraphParticles.gameObject.SetActive(false);
            _waterTrail.gameObject.SetActive(false);
            _speedIncrease = 0;
        }

        public void Reset()
        {
            transform.position = Vector3.zero;
            _isDead = false;
            transform.rotation = Quaternion.identity;
            switch(Type)
            {
                case NftType.Water:
                    _fireParticles.gameObject.SetActive(false);
                    _waterTrail.gameObject.SetActive(true);
                    _tradeGraphParticles.gameObject.SetActive(false);
                    break;
                case NftType.Default:
                    _fireParticles.gameObject.SetActive(true);
                    _waterTrail.gameObject.SetActive(false);
                    _tradeGraphParticles.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Awake()
        {
            _velocity = new Vector3();
            _rotation = new Vector3();
            var fireParticlesEmission = _fireParticles.emission;
            _initialParticleEmission = fireParticlesEmission.rateOverDistance;
            fireParticlesEmission.rateOverDistance = 0;
        }

        private void Update()
        {
            float delta = Time.deltaTime;

            ApplyGravity(in delta);
            TryApplyFallRotation(in delta);

            if(!_isDead)
            {
                _speedIncrease += MovementParameters.ForwardSpeedIncrease * delta;
                MoveForward();
                ProcessInput();
            }

            transform.position += _velocity * delta;
            transform.rotation = Quaternion.Euler(_rotation);
        }

        private void ProcessInput() 
        {
            if(_input.TapUp())
            {
                Flap();
            }
        }

        public void OnHitGround()
        {
            _audio.OnHitGround();
            enabled = false;
        }

        public void Flap()
        {
            _velocity.y = MovementParameters.FlapSpeed;
            _rotation.z = MovementParameters.FlapRotation;
            _audio.OnFlap();

            /*if (!_tradeGraphParticles.gameObject.activeInHierarchy)
        {
            _tradeGraphParticles.gameObject.SetActive(true);   
        }*/

            switch (Type)
            {
                case NftType.Water:
                    _waterSplash.gameObject.SetActive(false);
                    _waterSplash.gameObject.SetActive(true);
                    break;
                case NftType.Default:
                    var fireParticlesEmission = _fireParticles.emission;
                    fireParticlesEmission.rateOverDistance = _initialParticleEmission;
                    if (_fireDisableCoroutine != null)
                    {
                        StopCoroutine(_fireDisableCoroutine);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            CameraShake.Shake(0.1f, 0.05f);
            _fireDisableCoroutine = StartCoroutine(SprayFire());
        }

        private IEnumerator SprayFire()
        {
            yield return new WaitForSeconds(0.2f);
            var fireParticlesEmission = _fireParticles.emission;
            fireParticlesEmission.rateOverDistance = 0;
        }

        private void MoveForward()
        {
            _velocity.x = MovementParameters.ForwardSpeed + _speedIncrease;
        }

        private void ApplyGravity(in float delta)
        {
            _velocity.y -= MovementParameters.Gravity * delta;
        }

        private void TryApplyFallRotation(in float delta)
        {
            if(_velocity.y <= 0)
            {
                _rotation.z -= delta * MovementParameters.FallingRotationSpeed;
                _rotation.z = Mathf.Clamp(
                    _rotation.z, 
                    MovementParameters.FallingRotationAngle,
                    MovementParameters.FlapRotation
                );
            }
        }
    }
}
