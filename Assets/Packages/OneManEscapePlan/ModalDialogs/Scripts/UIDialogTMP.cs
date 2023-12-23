/// ©2019 - 2021 Kevin Foley
/// For distribution only on the Unity Asset Store
/// Terms/EULA: https://unity3d.com/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace OneManEscapePlan.ModalDialogs.Scripts {

	[RequireComponent(typeof(Canvas))]
	public class UIDialogTMP : UIDialogBase {

		[SerializeField] protected TMP_Text titleText;
		[SerializeField] protected TMP_Text messageText;


		override protected void initialize() {
			base.initialize();

			Assert.IsNotNull(titleText);
			Assert.IsNotNull(messageText);
		}

		protected override void ApplyButtonSettings(ICollection<ButtonSettings> buttonSettings) {
			//Populate buttons with the given settings
			int i = 0;
			foreach (var setting in buttonSettings) {
				TMP_Text text = buttons[i].GetComponentInChildren<TMP_Text>();
				text.text = setting.Label;

				if (setting.FontSettings != null) ApplyFontSettings(text, setting.FontSettings);

				buttons[i].onClick.RemoveAllListeners();
				if (setting.Callback == null) {
					buttons[i].onClick.AddListener(Close);
				} else {
					buttons[i].onClick.AddListener(() =>
					{
						setting.Callback.Invoke();
						Close();
					});
				}

				i++;
			}
		}

		/// <summary>
		/// Set the font settings for the title
		/// </summary>
		/// <param name="settings"></param>
		/// <returns>This UIDialog instance (for method chaining)</returns>
		override public UIDialogBase SetTitleFontSettings(FontSettings settings) {
			ApplyFontSettings(titleText, settings);
			return this;
		}

		/// <summary>
		/// Set the font settings for the message
		/// </summary>
		/// <param name="settings"></param>
		/// <returns>This UIDialog instance (for method chaining)</returns>
		override public UIDialogBase SetMessageFontSettings(FontSettings settings) {
			ApplyFontSettings(messageText, settings);
			return this;
		}

		/// <summary>
		/// Apply the given font settings to the given UI Text
		/// </summary>
		/// <param name="text"></param>
		/// <param name="settings"></param>
		virtual protected void ApplyFontSettings(TMP_Text text, FontSettings settings) {
			Assert.IsNotNull(text);
			Assert.IsNotNull(settings);

			if (settings.TMPFont != null) text.font = settings.TMPFont;
			if (settings.Color != null) text.color = settings.Color.Value;
			text.fontSize = settings.FontSize;
			text.fontStyle = settings.TMPStyle;
		}

		/// <summary>
		/// Title displayed in the dialog header
		/// </summary>
		override public string Title {
			get {
				if (!isInitialized) initialize();
				return titleText.text;
			}
			set {
				if (!isInitialized) initialize();
				titleText.text = value;
			}
		}

		/// <summary>
		/// Message displayed in the dialog body
		/// </summary>
		override public string Message {
			get {
				if (!isInitialized) initialize();
				return messageText.text;
			}
			set {
				if (!isInitialized) initialize();
				messageText.text = value;
			}
		}

		/// <summary>
		/// Label displayed on the checkbox. If null or empty, the checkbox will be hidden
		/// </summary>
		override public string CheckboxLabel {
			get {
				if (!isInitialized) initialize();
				return checkbox.GetComponentInChildren<TMP_Text>().text;
			}
			set {
				if (!isInitialized) initialize();
				checkbox.GetComponentInChildren<TMP_Text>().text = value;
				checkboxGroup.gameObject.SetActive(!string.IsNullOrEmpty(value));
			}
		}
	}
}
