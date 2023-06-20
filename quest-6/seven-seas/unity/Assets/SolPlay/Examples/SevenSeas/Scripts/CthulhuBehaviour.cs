using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Frictionless;
using SolPlay.FlappyGame.Runtime.Scripts;
using SolPlay.Scripts.Ui;
using UnityEngine;

public class CthulhuBehaviour : MonoBehaviour
{
    public GameObject AttackBullet;
    public GameObject BulletStartPoint;

    public float SpeedMultiplier = 1;
    public float JumpPower = 3;
    public Ease BulletEase = Ease.InExpo;
    
    void Start()
    {
       MessageRouter.AddHandler<SevenSeasService.CthulhuAttackedMessage>(OnCthuluhMessage); 
    }

    private void OnCthuluhMessage(SevenSeasService.CthulhuAttackedMessage message)
    {
        var attackBullet = Instantiate(AttackBullet);
        attackBullet.transform.position = BulletStartPoint.transform.position;
        float distance = (attackBullet.transform.position - message.Ship.transform.position).magnitude;
        attackBullet.transform.DOJump(message.Ship.transform.position, JumpPower, 1, SpeedMultiplier * distance).SetEase(BulletEase)
            .OnComplete(() =>
            {
                CameraShake.Shake(0.1f, 0.5f);
                MessageRouter.RaiseMessage(new BlimpSystem.Show3DBlimpMessage("-"+message.Damage, message.Ship.transform.position));
                Destroy(attackBullet);
            });
    }
}
