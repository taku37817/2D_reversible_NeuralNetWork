using UnityEngine;
using System.Collections;

public class Game_SoundManeger : MonoBehaviour
{
    public static Game_SoundManeger Instance
    {
        get { return instance; }
    }

    static Game_SoundManeger instance;

    public AudioSource put;
    public AudioSource turn;

    void Awake()
    {
        instance = this;
    }
}
