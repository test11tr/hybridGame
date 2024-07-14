using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakeModule : MonoBehaviour
{
    public CinemachineVirtualCamera CinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin _cbmp;

    public void ShakeCamera(float shakeIntensity, float shakeFrequency, float shakeTime)
    {
        CinemachineBasicMultiChannelPerlin _cbmp = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmp.m_AmplitudeGain = shakeIntensity;
        _cbmp.m_FrequencyGain = shakeFrequency;
        DelayHelper.DelayAction(shakeTime, StopShake);
    }

    void StopShake()
    {
        CinemachineBasicMultiChannelPerlin _cbmp = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmp.m_AmplitudeGain = 0f;
        _cbmp.m_FrequencyGain = 0f;
    }
}