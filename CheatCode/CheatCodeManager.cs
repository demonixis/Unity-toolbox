using System;
using UnityEngine;
using UnityEngine.UI;

public class CheatCodeManager : MonoBehaviour
{
	private GameObject _cheatCodeNode;
    public Text cheatCodeText;
    public AudioClip cheatCodeSound;

    void Awake()
    {
        if (cheatCodeText == null)
        {
            Debug.LogException(new Exception("You must attach a Text element to this component to display the messages"));
            Destroy(this);
        }
    }

    void Start()
    {
		_cheatCodeNode = new GameObject("CheatCodeNode");
		_cheatCodeNode.SetActive(false);
		_cheatCodeNode.transform.parent = transform;

        AddCheatCode("GODMODE", "God Mode", false, () =>
        {
            // Toggle God Mode.
        });

        AddCheatCode("GIVEMEALLWEAPONS", "You now have all weapons!", true,  () =>
        {
            // Give all weapons to the player.
        });

        AddCheatCode("KILLALL", "All enemies are dead", true, () =>
        {
			// Kill all ennemies
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
            _cheatCodeNode.SetActive(!_cheatCodeNode.activeSelf);
    }
	
	private void AddCheatCode(string sequence, string description, bool destroyAfterUnlock, Action callback)
	{
        var codeSeq = new string[sequence.Length];
        for (int i = 0, l = sequence.Length; i < l; i++)
            codeSeq[i] = sequence[i].ToString();

		var code = (CheatCode)_cheatCodeNode.AddComponent(typeof(CheatCode));
        code.Code = codeSeq;
		code.Description = description;
        code.DestroyAfterUnlock = destroyAfterUnlock;
		code.SuccessCallback = () => 
        {
			ShowCheatCodeUI(code.Description);
			callback();
		};
	}
	
	private void ShowCheatCodeUI(string description)
	{
		cheatCodeText.text = description;
		cheatCodeText.gameObject.SetActive(true);
		Invoke("HideCheatCodeUI", 2.5f);
		AudioSource.PlayClipAtPoint(cheatCodeSound, GameObject.FindWithTag("Player").transform.position);
	}
	
	private void HideCheatCodeUI()
	{
		cheatCodeText.gameObject.SetActive(false);
	}
}