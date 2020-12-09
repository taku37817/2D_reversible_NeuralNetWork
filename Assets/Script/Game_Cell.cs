using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;    //UIBehaviour使うため
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マスクラス
/// </summary>
[RequireComponent(typeof(Button))]
public class Game_Cell : UIBehaviour
{
    [SerializeField]
    Image frame;
    [SerializeField]
    Image stone;

    int x;
    int y;
    Button button;
    public bool IsPuttable
    {
        set
        {
            frame.color = value ? Color.red : Color.black;
        }
        get
        {
            return frame.color == Color.red;
        }
    }
    /// <summary>
    /// クリック可能確認
    /// </summary>
    /// <returns></returns>
    public bool IsClicKable
    {
        set
        {
            button.enabled = value;
        }
    }
    /// <summary>
    /// マスX座標
    /// </summary>
    public int X { get { return x; } }

    /// <summary>
    /// マスY座標
    /// </summary>
    public int Y { get { return y; } }

    public Game_Fild.StoneColor stoneColor
    {
        set 
        {
            switch (value)
            {
                case Game_Fild.StoneColor.NONE:
                    stone.gameObject.SetActive(false);
                    break;
                case Game_Fild.StoneColor.BLACK:
                    stone.gameObject.SetActive(true);
                    stone.color = Color.black;
                    break;
                case Game_Fild.StoneColor.WHITE:
                    stone.gameObject.SetActive(true);
                    stone.color = Color.white;
                    break;
                default:
                    break;
            }
        }
        get
        {
            if (!stone.gameObject.activeSelf)
            {
                return Game_Fild.StoneColor.NONE;
            }
            return stone.color == Color.black ? Game_Fild.StoneColor.BLACK : Game_Fild.StoneColor.WHITE;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        button = GetComponent<Button>();
    }

    protected override void Start()
    {
        base.Start();
        button.onClick.AddListener(Onclik);
    }
    /// <summary>
    /// マスをイニシャライズ
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Initilize(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    void Onclik()
    {
        Game_SceneController.Instance.OnCellClick(this);
    }


}

