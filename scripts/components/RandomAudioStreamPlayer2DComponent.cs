using System;
using Godot;
using Godot.Collections;

namespace Game.Components;

public partial class RandomAudioStreamPlayer2DComponent : AudioStreamPlayer2D
{
    [Export] private Array<AudioStream> _audioStreams;

    public void PlayRandom()
    {
        if (_audioStreams.Count == 0)
            return;

        var randomIndex = new Random().Next(0, _audioStreams.Count);
        Stream = _audioStreams[randomIndex];
        Play();
    }
}
