using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Game_Message : UIBehaviour
{
    [SerializeField]
    Text text;

    protected override void Start()
    {
        base.Start();
        GetComponent<Button>().onClick.AddListener(Onc);
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// メッセージを表示するコルーチン
    /// </summary>
    /// <param name="massege"></param>
    /// <returns></returns>
    public IEnumerator Show(string massege)
    {
        text.text = massege;
        gameObject.SetActive(true);
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
    }
    /// <summary>
    /// メッセージクリック字の処理
    /// </summar
    void Onc()
    {
        Game_SoundManeger.Instance.put.Play();
        gameObject.SetActive(true);
    }
}
