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
        public bool fade;
        public float delay;

        public AudioJob(AudioAction _action, AudioType _type, bool _fade, float _delay)
        {
            action = _action;
            type = _type;
            fade = _fade;
            delay = _delay;
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

    public void PlayAudio(AudioType _type, bool _fade = false, float _delay = 0.0f)
    {
        AddJob(new AudioJob(AudioAction.START, _type, _fade, _delay));
    }

    public void StopAudio(AudioType _type, bool _fade = false, float _delay = 0.0f)
    {
        AddJob(new AudioJob(AudioAction.STOP, _type, _fade, _delay));
    }

    public void RestartAudio(AudioType _type, bool _fade = false, float _delay = 0.0f)
    {
        AddJob(new AudioJob(AudioAction.RESTART, _type, _fade, _delay));
    }

    // Private Methods

    private void AddJob(AudioJob _job)
    {
        RemoveConflictingJobs(_job.type);

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

    private AudioChannel GetAudioChannel(AudioType _type)
    {
        if (!audioTable.ContainsKey(_type))
        {
            return null;
        }

        return (AudioChannel) audioTable[_type];
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

    private void RemoveJob(AudioType _type)
    {
        // Return if you are trying to stop a job that isn't running
        if (!jobTable.ContainsKey(_type))
        {
            return;
        }

        IEnumerator _runningJob = (IEnumerator)jobTable[_type];
        StopCoroutine(_runningJob);
        jobTable.Remove(_type);
    }

    private void RemoveConflictingJobs(AudioType _type)
    {
        // cancel the job if it already exists in the jobTable
        if (jobTable.ContainsKey(_type))
        {
            RemoveJob(_type);
        }


        // cancel the jobs that are running on the same channel
        AudioType _conflictAudio = AudioType.None;
        foreach (DictionaryEntry _entry in jobTable)
        {
            AudioType _audioType = (AudioType)_entry.Key;
            AudioChannel _audioChannelInUse = GetAudioChannel(_audioType);
            AudioChannel _audioChannelNeeded = GetAudioChannel(_type);
            if (_audioChannelInUse.source == _audioChannelNeeded.source)
            {
                _conflictAudio = _audioType;
            }
        }

        if (_conflictAudio != AudioType.None)
        {
            RemoveJob(_conflictAudio);
        }
    }

    private IEnumerator RunAudioJob(AudioJob _job)
    {
        yield return new WaitForSeconds(_job.delay);

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

        // handle volume fades
        if (_job.fade)
        {
            float _initial = _job.action == AudioAction.START || _job.action == AudioAction.RESTART ? 0 : 1;
            float _target = _initial == 0 ? 1 : 0;
            float _duration = 1.0f;
            float _timer = 0.0f;

            while (_timer < _duration)
            {
                _channel.source.volume = Mathf.Lerp(_initial, _target, _timer / _duration);
                _timer += Time.deltaTime;
                yield return null;
            }

            if (_job.action == AudioAction.STOP)
            {
                _channel.source.Stop();
            }
        }

        jobTable.Remove(_job.type);
    }
}
