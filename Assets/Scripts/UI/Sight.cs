using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Sight : MonoBehaviour
{
    public GameObject sightCenter;
    public Image _sightCenterImage;
    private Image _image;

    public Color shootColor = Color.red;
    public float shootDOShakeScaleDuration = 0.2f;
    public float shootDOColorDuration = 0.2f;

    private void Start()
    {
        _image = GetComponent<Image>();
        _sightCenterImage = sightCenter.GetComponent<Image>();
    }

    public void PlayerShootAnimation()
    {
        // sightCenter.transform.DOScaleX(2.5f, shootDOShakeScaleDuration,).SetLoops(2, LoopType.Yoyo);
        // sightCenter.transform.DOScaleY(2.5f, shootDOShakeScaleDuration).SetLoops(2, LoopType.Yoyo);
        sightCenter.transform.DOPunchScale(new Vector3(1, 1, 0), shootDOShakeScaleDuration, 3);
        // sightCenter.transform.DOShakeScale(2.5f, new Vector3(1, 1, 0), 10, 0f);
        // sightCenter.transform.DOShakeScale(2.5f, new Vector3(1, 1, 0), 10, 0f);
        _image.DOColor(shootColor, shootDOColorDuration)
            .OnComplete(() =>
            {
                _image.DOColor(Color.white, shootDOColorDuration);
            });
        // _sightCenterImage.DOColor(shootColor, shootDOColorDuration).SetLoops(2, LoopType.Yoyo);
    }
}