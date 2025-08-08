using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class scr_manButton : MonoBehaviour
{
	public GameObject[] buttons;//resume button
	public Vector2 posOffsetSel = Vector2.zero;
	public GameObject buttonMan;
	public GameObject buttonPrevMan;
	public static scr_manButton s;
	public GameObject iconSelect;//cappy icon
	public EventSystem ev;

	private int currentButton = 0; //number of current button
	private bool buttonPressed = false; //if it was pressed once, it has to be 0 to be able to be pressed again, so it wont just go party with the buttons.
	private Vector3 iconSelectPos; // set everytime new button selected, this is cuz he moves left and right.

	public void SetMenu(GameObject[] lbuttons, GameObject lbuttonMan, byte firstSelectPos, Vector2 lposOffsetSel)
    {
		if(buttonMan != null) buttonPrevMan = buttonMan;
		buttons = lbuttons;
		buttonMan = lbuttonMan;
		posOffsetSel = lposOffsetSel;
		currentButton = firstSelectPos;
		iconSelect.SetActive(true);
		setPosition(buttons[currentButton].transform.position);
		ev.firstSelectedGameObject = buttons [currentButton];
	}
	public void Active(bool b)
    {
        if (b) setPosition(buttons[currentButton].transform.position);
		iconSelect.SetActive(b);
	}
	public void SwitchPrevMenu()
    {
		if(buttonPrevMan == null) buttonPrevMan.SendMessage("OnMenu");
		else Active(false);
	}
	public void SetButton(int index)
	{
		currentButton = index;
		setPosition(buttons[currentButton].transform.position);
	}

	void setPosition(Vector3 position)
	{
		iconSelect.transform.position = new Vector3(posOffsetSel.x + position.x - buttons[currentButton].GetComponent<RectTransform>().rect.width/2, posOffsetSel.y + position.y, -300);
		if(EventSystem.current != null)
			EventSystem.current.SetSelectedGameObject(buttons[currentButton]);
	}

	void Awake()
	{
		iconSelect = transform.GetChild(3).gameObject;
		ev = GetComponent<EventSystem> ();
		s = this;
		iconSelect.SetActive(false);
	}

	void FixedUpdate()
	{
		if ((Time.timeScale > 0) && (buttons.Length > 0))
		{
#if UNITY_EDITOR
			float h = Input.GetAxisRaw("Vertical");
			bool buttonOK = Input.GetKey(KeyCode.Return);
#else
			float h = UnityEngine.N3DS.GamePad.CirclePad.y + Input.GetAxisRaw("Vertical");
			bool buttonOK = UnityEngine.N3DS.GamePad.GetButtonHold(N3dsButton.A);
#endif
			if (!buttonPressed && h != 0)
			{
				buttonPressed = true;
				if (h > 0)
					if (currentButton <= 0)
						currentButton = buttons.Length - 1;
					else
						currentButton--;
				if (h < 0)
					if (currentButton >= buttons.Length - 1)
						currentButton = 0;
					else
						currentButton++;
				setPosition(buttons[currentButton].transform.position);
			}
			else if (h == 0)
				buttonPressed = false;

			if (buttonOK)
			{
				buttonMan.SendMessage("OnButtonPress", currentButton);
				Active(false);
			}
		}
	}
}
