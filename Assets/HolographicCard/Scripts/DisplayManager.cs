using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DisplayManager : MonoBehaviour {
    [SerializeField] Transform cameraTransform;
    [SerializeField] float animationDuration;
    [SerializeField] float baseDistanceFromCard;
    [SerializeField] float pressedDistanceFromCard;
    float currentDistanceFromCard;
    Coroutine rotationCoroutine;
    //
    [SerializeField] Material sparkleMaterial;
    [SerializeField] float endSparkleVal;
    //
    [SerializeField] Transform galaxySphere;
    //
    bool image0Mode = true;
    bool pressed = false;
    [SerializeField] List<Transform> imageList0;
    float[] baseOffsets0;
    [SerializeField] List<Transform> imageList1;
    float[] baseOffsets1;
    //
    Tween distanceChangeTween;
    Tween transformMoveTween;
    const float StartRotation = Mathf.PI / 3;
    const float EndRotation = Mathf.PI - StartRotation;
    static readonly int _MinusVal = Shader.PropertyToID("_MinusVal");
    [RuntimeInitializeOnLoadMethod]
    public static void InitializeTween() => DOTween.Init();
    void Awake() {
        currentDistanceFromCard = baseDistanceFromCard;
        rotationCoroutine = StartCoroutine(ContinuousRotation());
        //
        baseOffsets0 = new float[imageList0.Count];
        for (int i = 0; i < imageList0.Count; i++) baseOffsets0[i] = imageList0[i].transform.position.y;
        //
        baseOffsets1 = new float[imageList1.Count];
        for (int i = 0; i < imageList1.Count; i++) baseOffsets1[i] = imageList1[i].transform.position.y;

        SetAllPositionState(0, true);
        SetAllPositionState(1, false);
    }
    IEnumerator ContinuousRotation() {
        while (true) {
            yield return DOTween.To(() => 0f, value => sparkleMaterial.SetFloat(_MinusVal, value), endSparkleVal, animationDuration).SetEase(Ease.InOutQuad);
            yield return DOTween.To(() => StartRotation, SetCameraRotation, EndRotation, animationDuration).SetEase(Ease.InOutQuad).WaitForKill();
            yield return null;
            yield return DOTween.To(() => endSparkleVal, value => sparkleMaterial.SetFloat(_MinusVal, value), 0, animationDuration).SetEase(Ease.InOutQuad);
            yield return DOTween.To(() => EndRotation, SetCameraRotation, StartRotation, animationDuration).SetEase(Ease.InOutQuad).WaitForKill();
            yield return null;
        }
    }
    void OnDisable() {
        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
        rotationCoroutine = null;
    }
    void Update() {
        galaxySphere.Rotate(Vector3.forward, 10 * Time.deltaTime);
        if (Input.GetMouseButton(0)) {
            if (!pressed) {
                pressed = true;
                //
                distanceChangeTween?.Kill();
                distanceChangeTween = DOTween.To(() => currentDistanceFromCard, value => currentDistanceFromCard = value, pressedDistanceFromCard, 0.3f).SetEase(Ease.OutQuint)
                    .OnKill(delegate { distanceChangeTween = null; });
                //
                int mode = image0Mode ? 0 : 1;
                transformMoveTween?.Kill();
                transformMoveTween = SetPositionAnimation(mode, false).OnKill(delegate {
                    transformMoveTween = null;
                    SetAllPositionState(mode, false);
                });
            }
        } else {
            if (pressed) {
                pressed = false;
                //
                distanceChangeTween?.Kill();
                distanceChangeTween = DOTween.To(() => currentDistanceFromCard, value => currentDistanceFromCard = value, baseDistanceFromCard, 0.3f).SetEase(Ease.OutBack)
                    .OnKill(delegate { distanceChangeTween = null; });
                //
                image0Mode = !image0Mode;
                transformMoveTween?.Kill();
                int mode = image0Mode ? 0 : 1;
                transformMoveTween = SetPositionAnimation(mode, true).OnKill(delegate {
                    transformMoveTween = null;
                    SetAllPositionState(mode, true);
                });
            }
        }
    }
    const float OffOffset = 2.5f;
    Sequence SetPositionAnimation(int imageIndex, bool state) {
        List<Transform> transformList = imageIndex == 0 ? imageList0 : imageList1;
        float[] offsets = imageIndex == 0 ? baseOffsets0 : baseOffsets1;
        float additionalOffset = state ? 0 : OffOffset;
        Ease ease = state ? Ease.OutBack : Ease.InBack;
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < transformList.Count; i++) {
            int index = i;
            sequence.Insert(0.04f * i,
                DOTween.To(() => transformList[index].position.y, value => transformList[index].SetPosY(value), offsets[index] - additionalOffset, 0.2f).SetEase(ease));
        }

        return sequence;
    }
    void SetAllPositionState(int imageIndex, bool state) {
        if (imageIndex == 0) {
            for (int i = 0; i < imageList0.Count; i++) {
                Vector3 pos = imageList0[i].position;
                imageList0[i].position = new Vector3(pos.x, baseOffsets0[i] - (state ? 0 : OffOffset), pos.z);
            }
        } else if (imageIndex == 1) {
            for (int i = 0; i < imageList1.Count; i++) {
                Vector3 pos = imageList1[i].position;
                imageList1[i].position = new Vector3(pos.x, baseOffsets1[i] - (state ? 0 : OffOffset), pos.z);
            }
        }
    }
    void SetCameraRotation(float degrees) {
        cameraTransform.position = new Vector3(Mathf.Cos(degrees), 0, Mathf.Sin(degrees)) * -currentDistanceFromCard;
        cameraTransform.LookAt(Vector3.zero);
    }
}
public static class Extensions {
    public static void SetPosY(this Transform t, float y) {
        Vector3 position = t.position;
        t.position = new Vector3(position.x, y, position.z);
    }
}