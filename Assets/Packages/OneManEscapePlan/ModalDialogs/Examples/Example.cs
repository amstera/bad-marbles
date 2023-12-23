/// ©2019 - 2021 Kevin Foley
/// For distribution only on the Unity Asset Store
/// Terms/EULA: https://unity3d.com/legal/as_terms

using OneManEscapePlan.ModalDialogs.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OneManEscapePlan.ModalDialogs.Examples {
	public class Example : MonoBehaviour {

		[SerializeField] private Text statusText;
		[SerializeField] private Sprite leftIcon;
		[SerializeField] private Sprite rightIcon;

		private UIDialogBase nonModalDialog;

		private void Start() {
			Assert.IsNotNull(statusText);
			Assert.IsNotNull(leftIcon);
			Assert.IsNotNull(rightIcon);
		}

		public void ShowSimpleDialog() {
			statusText.text = "";
			DialogManager.Instance.ShowDialog("Alert", "The dialog is automatically sized to fit the length of the text, so it will look right no matter what size message you put in it.");
		}

		public void ShowBoldDialog() {
			statusText.text = "";

			FontSettings fontSettings = new FontSettings(15, FontStyle.Bold);

			List<ButtonSettings> buttonSettings = new List<ButtonSettings>();
			buttonSettings.Add(new ButtonSettings("Cool", null, fontSettings));

			DialogManager.Instance.ShowDialog("Alert", "You can supply custom font settings to customize the appearance of a particular dialog.", buttonSettings)
				.SetTitleFontSettings(fontSettings)
				.SetMessageFontSettings(fontSettings);
		}

		public void ShowNonModalDialog() {
			if (nonModalDialog != null) return;
			nonModalDialog = DialogManager.Instance.ShowDialog("Alert", "You can display the dialog as a modal dialog (which blocks the application underneath) or non-modal dialog. This example is non-modal.", false);
		}

		public void ShowDialogWithCheckbox() {
			statusText.text = "";
			DialogManager.Instance.ShowDialog("Checkbox Alert", "This dialog has a checkbox.", "I understand").ClosedEvent.AddListener((UIDialogBase dialog) =>
			{
				statusText.text = "Checkbox selected: " + dialog.CheckboxSelected;
			});
		}

		public void ShowConfirmationDialog() {
			statusText.text = "";
			DialogManager.Instance.ShowDialog("Confirmation Dialog", "This dialog lets the user confirm or cancel", "OK", OnPressedOK, "Cancel", OnPressedCancel);
		}

		public void ShowThreeButtonDialog() {
			statusText.text = "";

			List<ButtonSettings> settings = new List<ButtonSettings>();
			settings.Add(new ButtonSettings("OK", OnPressedOK));
			settings.Add(new ButtonSettings("Cancel", OnPressedCancel));
			settings.Add(new ButtonSettings("Later", OnPressedLater));

			DialogManager.Instance.ShowDialog("Three-button Dialog", "The dialog supports a variable number of buttons with customizable labels. This example demonstrates three buttons.", settings);
		}

		public void ShowLeftIconDialog() {
			statusText.text = "";
			DialogManager.Instance.ShowDialog("Alert", "This is a dialog with an icon on the left side.").SetLeftIcon(leftIcon);
		}

		public void ShowRightIconDialog() {
			statusText.text = "";
			DialogManager.Instance.ShowDialog("Alert", "This is a dialog with an icon on the right side.").SetRightIcon(rightIcon);
		}

		public void ShowEverythingDialog() {
			statusText.text = "";
			FontSettings titleSettings = new FontSettings(15, FontStyle.Bold, Color.white);
			FontSettings messageSettings = new FontSettings(15, FontStyle.Bold, Color.red);
				
			DialogManager.Instance.ShowDialog("Fancy Dialog", "The various dialog features can be combined in whatever manner you wish.", "I understand", "OK", OnPressedOK, "Cancel", OnPressedCancel)
				.SetLeftIcon(leftIcon)
				.SetRightIcon(rightIcon)
				.SetTitleFontSettings(titleSettings)
				.SetMessageFontSettings(messageSettings)
				.ClosedEvent.AddListener((UIDialogBase dialog) =>
				{
					statusText.text += ". Checkbox selected: " + dialog.CheckboxSelected;
				});
		}

		private void OnPressedOK() {
			statusText.text = "Pressed OK";
		}

		private void OnPressedCancel() {
			statusText.text = "Pressed Cancel";
		}

		private void OnPressedLater() {
			statusText.text = "Pressed Later";
		}
	}
}