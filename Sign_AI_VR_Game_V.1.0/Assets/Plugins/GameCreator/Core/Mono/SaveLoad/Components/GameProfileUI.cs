namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using UnityEngine;
    using UnityEngine.UI;
    using GameCreator.Variables;

    [AddComponentMenu("Game Creator/UI/Game Profile", 100)]
    public class GameProfileUI : MonoBehaviour
    {
        public NumberProperty profile = new NumberProperty(1);
        
        public Text textProfile;
        public string formatProfile = "{0:00}";

        public Text textDate;
        public string formatDate = "dd/MM/yyyy HH:mm";

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Start()
        {
            this.UpdateUI();
            SaveLoadManager.Instance.onSave += this.UpdateUI;
        }

        private void OnDestroy()
        {
            SaveLoadManager.Instance.onSave -= this.UpdateUI;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void UpdateUI(int updateProfile = -1)
        {
            int profileNumber = Mathf.RoundToInt(this.profile.GetValue(gameObject));

            if (updateProfile != -1 && profileNumber != updateProfile) return;
            SavesData.Profile data = SaveLoadManager.Instance.GetProfileInfo(profileNumber);

            if (this.textProfile != null)
            {
                this.textProfile.text = string.Format(
                    this.formatProfile, 
                    profileNumber
                );
            }

            if (this.textDate != null && data != null)
            {
                DateTime date = DateTime.Parse(data.date);
                this.textDate.text = date.ToString(this.formatDate);
            }
        }
    }
}