/// ©2019 - 2021 Kevin Foley
/// For distribution only on the Unity Asset Store
/// Terms/EULA: https://unity3d.com/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OneManEscapePlan.ModalDialogs.Scripts {

	[System.Serializable] public class UIDialogEvent : UnityEvent<UIDialogBase> { }

	/// <summary>
	/// Defines settings for a button
	/// </summary>
	[System.Serializable] public class ButtonSettings {
		public string Label { get; private set; }
		public FontSettings FontSettings { get; private set; }
		public UnityAction Callback { get; private set; }

		/// <summary>
		/// Create a new instance of ButtonSettings with the given values
		/// </summary>
		/// <param name="label">The label to display on the button</param>
		/// <param name="callback">A function that will be called when the button is pressed (null allowed)</param>
		/// <param name="fontSettings">[Optional] The font settings that will be used for the button label</param>
		public ButtonSettings(string label, UnityAction callback, FontSettings fontSettings = null) {
			Label = label;
			Callback = callback;
			FontSettings = fontSettings;
		}
	}

	public class FontSettings {
		public Font Font { get; private set; }
		public TMP_FontAsset TMPFont { get; private set; }
		public int FontSize { get; private set; }
		public FontStyle Style { get; private set; }
		public FontStyles TMPStyle { get; private set; }
		public Color? Color { get; private set; }

		/// <summary>
		/// Create a new instance of FontSettings configured for legacy text with the given values
		/// </summary>
		/// <param name="fontSize">The size of the font, in points</param>
		/// <param name="style">The font style</param>
		/// <param name="color">[Optional] The font color</param>
		/// <param name="font">[Optional] The specific font to use</param>
		public FontSettings(int fontSize, FontStyle style = FontStyle.Normal, Color? color = null, Font font = null) {
			Font = font;
			FontSize = fontSize;
			Color = color;
			Style = style;
		}
		
		/// <summary>
		/// Create a new instance of FontSettings configured for TextMeshPro with the given values
		/// </summary>
		/// <param name="fontSize">The size of the font, in points</param>
		/// <param name="style">The font style</param>
		/// <param name="color">[Optional] The font color</param>
		/// <param name="font">[Optional] The specific font to use</param>
		public FontSettings(int fontSize, FontStyles style = FontStyles.Normal, Color? color = null, TMP_FontAsset font = null) {
			TMPFont = font;
			FontSize = fontSize;
			Color = color;
			TMPStyle = style;
		}
	}

	[RequireComponent(typeof(Canvas))]
	abstract public class UIDialogBase : MonoBehaviour {

		[SerializeField] protected Image modalBackground;
		[SerializeField] protected Image leftIcon;
		[SerializeField] protected Image rightIcon;
		[SerializeField] protected LayoutGroup checkboxGroup;
		[SerializeField] protected Toggle checkbox;
		[SerializeField] protected LayoutGroup buttonGroup;
		[SerializeField] protected Button button;

		[SerializeField] protected UIDialogEvent closedEvent = new UIDialogEvent();
		public UIDialogEvent ClosedEvent => closedEvent;

		protected List<Button> buttons = new List<Button>();
		protected bool isInitialized = false;

		virtual protected void Awake() {
			initialize();
		}

		virtual protected void initialize() {
			if (isInitialized) return;
			isInitialized = true;

			Assert.IsNotNull(modalBackground);
			Assert.IsNotNull(leftIcon);
			Assert.IsNotNull(rightIcon);
			Assert.IsNotNull(checkboxGroup);
			Assert.IsNotNull(checkbox);
			Assert.IsNotNull(buttonGroup);
			Assert.IsNotNull(button);

			leftIcon.gameObject.SetActive(false);
			rightIcon.gameObject.SetActive(false);
			checkboxGroup.gameObject.SetActive(false);

			buttons.Add(button);
			button.onClick.AddListener(Close);
		}

		/// <summary>
		/// Set the labels and callbacks of the buttons to display at the bottom of the dialog
		/// </summary>
		/// <param name="buttonSettings"></param>
		/// <returns></returns>
		virtual public UIDialogBase SetButtons(ICollection<ButtonSettings> buttonSettings) {
			Assert.IsNotNull(buttonSettings, "You must specify at least one button");
			Assert.IsTrue(buttonSettings.Count > 0, "You must specify at least one button");

			//add buttons if we don't have enough
			while (buttons.Count < buttonSettings.Count) {
				Button newButton = Instantiate(button, buttonGroup.transform);
				buttons.Add(newButton);
			}
			//remove buttons if we have too many
			while (buttons.Count > buttonSettings.Count) {
				Button oldButton = buttons[buttons.Count - 1];
				buttons.RemoveAt(buttons.Count - 1);
				Destroy(oldButton);
			}

			ApplyButtonSettings(buttonSettings);

			return this;
		}

		abstract protected void ApplyButtonSettings(ICollection<ButtonSettings> buttonSettings);

		/// <summary>
		/// Set the title and message
		/// </summary>
		/// <param name="title">Title displayed in the dialog header</param>
		/// <param name="message">Message displayed in the dialog body</param>
		/// <returns>This UIDialog instance (for method chaining)</returns>
		public UIDialogBase SetText(string title, string message) {
			Title = title;
			Message = message;
			return this;
		}

		/// <summary>
		/// Set the label displayed on the checkbox. If null or empty, the checkbox will be hidden
		/// </summary>
		/// <param name="text">The checkbox label</param>
		/// <returns>This UIDialog instance (for method chaining)</returns>
		public UIDialogBase SetCheckboxLabel(string text) {
			CheckboxLabel = text;
			return this;
		}

		/// <summary>
		/// Set the icon displayed to the left of the message. If null, the icon will be hidden.
		/// </summary>
		/// <param name="icon">Icon to display</param>
		/// <returns>This UIDialog instance (for method chaining)</returns>
		public UIDialogBase SetLeftIcon(Sprite icon) {
			LeftIcon = icon;
			return this;
		}

		/// <summary>
		/// Set the icon displayed to the right of the message. If null, the icon will be hidden.
		/// </summary>
		/// <param name="icon">Icon to display</param>
		/// <returns>This UIDialog instance (for method chaining)</returns>
		public UIDialogBase SetRightIcon(Sprite icon) {
			RightIcon = icon;
			return this;
		}

		/// <summary>
		/// Set whether this dialog is modal (blocks interaction with the application underneath)
		/// </summary>
		/// <param name="value"></param>
		/// <returns>This UIDialog instance (for method chaining)</returns>
		public UIDialogBase SetModal(bool value) {
			IsModal = value;
			return this;
		}

		/// <summary>
		/// Set the font settings for the title
		/// </summary>
		/// <param name="settings"></param>
		/// <returns>This UIDialog instance (for method chaining)</returns>
		abstract public UIDialogBase SetTitleFontSettings(FontSettings settings);

		/// <summary>
		/// Set the font settings for the message
		/// </summary>
		/// <param name="settings"></param>
		/// <returns>This UIDialog instance (for method chaining)</returns>
		abstract public UIDialogBase SetMessageFontSettings(FontSettings settings);

		/// <summary>
		/// Close this dialog
		/// </summary>
		virtual public void Close() {
			closedEvent.Invoke(this);
			Destroy(gameObject);
		}

		/// <summary>
		/// Title displayed in the dialog header
		/// </summary>
		abstract public string Title {
			get; set;
		}

		/// <summary>
		/// Message displayed in the dialog body
		/// </summary>
		abstract public string Message {
			get; set;
		}

		/// <summary>
		/// Icon displayed to the left of the message. If null, the icon will be hidden.
		/// </summary>
		virtual public Sprite LeftIcon {
			get {
				if (!isInitialized) initialize();
				return leftIcon.sprite;
			}
			set {
				if (!isInitialized) initialize();
				leftIcon.sprite = value;
				leftIcon.gameObject.SetActive(value != null);
			}
		}

		/// <summary>
		/// Icon displayed to the right of the message. If null, the icon will be hidden.
		/// </summary>
		virtual public Sprite RightIcon {
			get {
				if (!isInitialized) initialize();
				return rightIcon.sprite;
			}
			set {
				if (!isInitialized) initialize();
				rightIcon.sprite = value;
				rightIcon.gameObject.SetActive(value != null);
			}
		}

		/// <summary>
		/// Label displayed on the checkbox. If null or empty, the checkbox will be hidden
		/// </summary>
		abstract public string CheckboxLabel {
			get; set;
		}

		/// <summary>
		/// Whether the checkbox is selected
		/// </summary>
		virtual public bool CheckboxSelected {
			get {
				if (!isInitialized) initialize();
#if UNITY_EDITOR
				if (!checkbox.gameObject.activeInHierarchy) {
					Debug.LogWarning("You are accessing the 'CheckboxSelected' property while the checkbox is hidden");
				}
#endif
				return checkbox.isOn;
			}
			set {
				if (!isInitialized) initialize();
				checkbox.isOn = value;
			}
		}

		/// <summary>
		/// Whether this dialog is modal (blocks interaction with the application underneath)
		/// </summary>
		virtual public bool IsModal {
			get {
				if (!isInitialized) initialize();
				return modalBackground.gameObject.activeInHierarchy;
			}
			set {
				if (!isInitialized) initialize();
				modalBackground.gameObject.SetActive(value);
			}
		}
	}
}
