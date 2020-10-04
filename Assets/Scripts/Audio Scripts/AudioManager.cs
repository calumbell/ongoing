using System.Collections;
using UnityEngine;

// Adapted from https://github.com/coderDarren/UnityCore

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioChannel[] channels;

    private Hashtable audioTable; // relationship btwn AudioTypes (key) & channels (value)
    private Hashtable jobTable;

    private enum AudioAction
    {
        START,
        STOP,
        RESTART
    }

    [System.Serializable]
    public class AudioObject
    {
        public AudioType type;
        public AudioClip clip;
    }

    [System.Serializable]
    public class AudioChannel
    {
        public AudioSource source;
        public AudioObject[] audio;
    }

    private class AudioJob
    {
        public AudioAction action;
        public AudioType type;

        public AudioJob(AudioAction _action, AudioType _type)
        {
            action = _action;
            type = _type;
        }
    }

    // Unity Functions

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            audioTable = new Hashtable();
            jobTable = new Hashtable();
            GenerateAudioTable();
            DontDestroyOnLoad(gameObject);
        }
    }

    // Public Methods

    public void PlayAudio(AudioType _type)
    {
        AddJob(new AudioJob(AudioAction.START, _type));
    }

    public void StopAudio(AudioType _type)
    {
        AddJob(new AudioJob(AudioAction.STOP, _type));
    }

    public void RestartAudio(AudioType _type)
    {
        AddJob(new AudioJob(AudioAction.RESTART, _type));
    }

    // Private Methods

    private void AddJob(AudioJob _job)
    {
        IEnumerator _jobRunner = RunAudioJob(_job);
        jobTable.Add(_job.type, _jobRunner);
        StartCoroutine(_jobRunner);
    }

    private void GenerateAudioTable()
    {
        foreach (AudioChannel _channel in channels)
        {
            foreach(AudioObject _obj in _channel.audio)
            {
                if (!audioTable.ContainsKey(_obj.type))
                    audioTable.Add(_obj.type, _channel);
            }
        }
    }

    private  AudioClip GetAudioClipFromChannel(AudioType _type, AudioChannel _channel)
    {
        foreach (AudioObject _obj in _channel.audio)
        {
            if (_obj.type == _type)
            {
                return _obj.clip;
            }
        }

        return null;
    }

    private IEnumerator RunAudioJob(AudioJob _job)
    {
        yield return new WaitForSeconds(0.0f);

        // Which channel does this audio type play on? Update that channel's clip
        AudioChannel _channel = (AudioChannel)audioTable[_job.type]; 
        _channel.source.clip = GetAudioClipFromChannel(_job.type, _channel);


        switch (_job.action)
        {
            case AudioAction.START:
                _channel.source.Play();
                break;

            case AudioAction.STOP:
                _channel.source.Stop();
                break;

            case AudioAction.RESTART:
                _channel.source.Stop();
                _channel.source.Play();
                break;
        }

        jobTable.Remove(_job.type);
    }
}
