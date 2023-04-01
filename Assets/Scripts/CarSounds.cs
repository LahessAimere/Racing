using UnityEngine;

public class CarSounds:MonoBehaviour
{
    public AudioSource _source;
        
    [Space(5)]
    public float idleRpm = 600.0f;
    public float minPitch = 0.25f;
    public float idleVolume = 0.4f;
    [Space(5)]
    public float maxRpm = 6000.0f;
    public float maxPitch = 1.5f;
    public float maxVolume = 0.6f;
        
        
    public float volumeChangeRateUp = 48.0f;
    public float volumeChangeRateDown = 16.0f;
        
    public void ProcessContinuousAudioPitch (float ratio)
    {

        _source.pitch = Mathf.Lerp(minPitch, maxPitch, ratio);

        if (!_source.isPlaying && _source.isActiveAndEnabled) 
            _source.Play();
            
        _source.loop = true;
        ProcessVolume(ratio,volumeChangeRateUp,volumeChangeRateDown);
    }
        
    public void ProcessVolume (float ratio, float changeRateUp, float changeRateDown)
    {
        float volume = Mathf.Lerp(idleVolume, maxVolume, ratio);
        float changeRate = volume > _source.volume? changeRateUp : changeRateDown;
        _source.volume = Mathf.Lerp(_source.volume, volume, Time.deltaTime * changeRate);
    }
}