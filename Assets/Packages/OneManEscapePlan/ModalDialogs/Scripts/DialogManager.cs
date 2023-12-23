/// ©2019 - 2023 Kevin Foley
/// For distribution only on the Unity Asset Store
/// Terms/EULA: https://unity3d.com/legal/as_terms

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace OneManEscapePlan.ModalDialogs.Scripts {
	public class DialogManager : MonoBehaviour {

		#region STATIC
		protected static DialogManager instance;
		public static DialogManager Instance {
			get {
#if UNITY_2021_3_OR_NEWER
				//NOTE: THIS LINE MAY NOT WORK IN EARLY VERSIONS OF 2021.3. TO FIX, UPGRADE TO LATEST VERSION OF
				//UNITY 2021.3, OR REPLACE FindAnyObjectByType WITH FindObjectOfType
				if (instance == null) instance = GameObject.FindAnyObjectByType<DialogManager>();
#else
				if (instance == null) instance = GameObject.FindObjectOfType<DialogManager>();
#endif
				if (instance == null) throw new System.Exception("No DialogManager instance exists");
				return instance;
			}
		}

#endregion

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[SerializeField] protected UIDialogBase dialogPrefab;
		[SerializeField] protected bool dontDestroyOnLoad;

		virtual protected void Awake() {
			Assert.IsNotNull(dialogPrefab, "You forgot to select a UIDialog prefab");

			if (dontDestroyOnLoad) {
				if (instance != null && instance != this) {
					Destroy(gameObject);
					return;
				}
				DontDestroyOnLoad(gameObject);
			}

			instance = this;
		}
		
		/// <summary>
		/// Show a dialog with the given title and message
		/// </summary>
		/// <param name="title">Title to display in the dialog</param>
		/// <param name="message">Message to display in the dialog</param>
		/// <param name="modal">Whether the dialog is modal (blocks application underneath)</param>
		/// <returns>The new dialog</returns>
		public UIDialogBase ShowDialog(string title, string message, bool modal = true) {
			return Instantiate(dialogPrefab).SetText(title, message).SetModal(modal);
		}

		/// <summary>
		/// Show a dialog with the given title, message, and checkbox label
		/// </summary>
		/// <param name="title">Title to display in the dialog</param>
		/// <param name="message">Message to display in the dialog</param>
		/// <param name="checkboxLabel">Label to display on the checkbox (checkbox will be hidden if null or empty)</param>
		/// <param name="modal">Whether the dialog is modal (blocks application underneath)</param>
		/// <returns>The new dialog</returns>
		public UIDialogBase ShowDialog(string title, string message, string checkboxLabel, bool modal = true) {
			return ShowDialog(title, message, modal).SetCheckboxLabel(checkboxLabel);
		}

		/// <summary>
		/// Show a dialog with the given title, message, and button settings
		/// </summary>
		/// <param name="title">Title to display in the dialog</param>
		/// <param name="message">Message to display in the dialog</param>
		/// <param name="buttonSettings">Settings for the buttons to display</param>
		/// <param name="modal">Whether the dialog is modal (blocks application underneath)</param>
		/// <returns>The new dialog</returns>
		public UIDialogBase ShowDialog(string title, string message, ICollection<ButtonSettings> buttonSettings, bool modal = true) {
			return ShowDialog(title, message, modal).SetButtons(buttonSettings);
		}

		/// <summary>
		/// Show a dialog with the given title, message, checkbox label, and button settings
		/// </summary>
		/// <param name="title">Title to display in the dialog</param>
		/// <param name="message">Message to display in the dialog</param>
		/// <param name="checkboxLabel">Label to display on the checkbox (checkbox will be hidden if null or empty)</param>
		/// <param name="buttonSettings">Settings for the buttons to display</param>
		/// <param name="modal">Whether the dialog is modal (blocks application underneath)</param>
		/// <returns>The new dialog</returns>
		public UIDialogBase ShowDialog(string title, string message, string checkboxLabel, ICollection<ButtonSettings> buttonSettings, bool modal = true) {
			return ShowDialog(title, message, modal).SetCheckboxLabel(checkboxLabel).SetButtons(buttonSettings);
		}

		/// <summary>
		/// Show a confirmation dialog with the given title, message, and confirm/cancel buttons
		/// </summary>
		/// <param name="title">Title to display in the dialog</param>
		/// <param name="message">Message to display in the dialog</param>
		/// <param name="confirmButtonLabel">Text shown on the confirm button</param>
		/// <param name="confirmButtonCallback">Callback triggered when the confirm button is activated</param>
		/// <param name="cancelButtonLabel">Text shown on the cancel button</param>
		/// <param name="cancelButtonCallback">Callback triggered when the cancel button is activated</param>
		/// <param name="modal">Whether the dialog is modal (blocks application underneath)</param>
		/// <returns>The new dialog</returns>
		public UIDialogBase ShowDialog(string title, string message, string confirmButtonLabel, UnityAction confirmButtonCallback, string cancelButtonLabel = "Cancel", UnityAction cancelButtonCallback = null, bool modal = true) {
			ButtonSettings[] buttonSettings = new ButtonSettings[2];
			buttonSettings[0] = new ButtonSettings(confirmButtonLabel, confirmButtonCallback);
			buttonSettings[1] = new ButtonSettings(cancelButtonLabel, cancelButtonCallback);
			return ShowDialog(title, message, buttonSettings, modal);
		}

		/// <summary>
		/// Show a confirmation dialog with the given title, message, checkbox label, and confirm/cancel buttons
		/// </summary>
		/// <param name="title">Title to display in the dialog</param>
		/// <param name="message">Message to display in the dialog</param>
		/// <param name="checkboxLabel">Label to display on the checkbox (checkbox will be hidden if null or empty)</param>
		/// <param name="confirmButtonLabel">Text shown on the confirm button</param>
		/// <param name="confirmButtonCallback">Callback triggered when the confirm button is activated</param>
		/// <param name="cancelButtonLabel">Text shown on the cancel button</param>
		/// <param name="cancelButtonCallback">Callback triggered when the cancel button is activated</param>
		/// <param name="modal">Whether the dialog is modal (blocks application underneath)</param>
		/// <returns>The new dialog</returns>
		public UIDialogBase ShowDialog(string title, string message, string checkboxLabel, string confirmButtonLabel, UnityAction confirmButtonCallback, string cancelButtonLabel = "Cancel", UnityAction cancelButtonCallback = null, bool modal = true) {
			return ShowDialog(title, message, confirmButtonLabel, confirmButtonCallback, cancelButtonLabel, cancelButtonCallback, modal).SetCheckboxLabel(checkboxLabel);
		}
	}
}
