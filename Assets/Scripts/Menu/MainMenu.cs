using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ZombiRunner
{
    public class MainMenu : MonoBehaviour
    {
        public GameObject dialog;
        public Text dialogTitle, dialogBody, soundLabel;
        public Button playButton;
        private bool loading;

        private void Start()
        {
            Time.timeScale = 1;
            AudioListener.volume = PlayerPrefs.GetInt("MenuMuted", 0) == 1 ? 0 : 1;
            UpdateSoundLabel();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) CloseDialog();
        }
        public void Play()
        {
            if (loading) return;
            loading = true;
            playButton.interactable = false;
            SceneManager.LoadSceneAsync("gameplay");
        }
        private void Show(string title, string body)
        {
            dialogTitle.text = title;
            dialogBody.text = body;
            dialog.SetActive(true);
        }
        public void Survivor() => Show("THE SURVIVOR", "One runner. No second chances.\n\nDodge the infected, collect supplies and keep moving.\nYour survivor is ready for the streets.");
        public void District() => Show("DEAD CITY", "The streets are never the same twice.\n\nRun through shifting city blocks, take the corners\nand survive ambushes along the way.\n\nENDLESS RUN  /  AVAILABLE");
        public void Armory() => Show("FIELD ARMORY", "BAT  /  PISTOL  /  RIFLE\n\nCollect weapon crates during your run to equip a weapon.\nTap to attack. Ammunition is limited.\n\nWeapons are found in the field, not purchased here.");
        public void Tutorial() => Show("STAY ALIVE", "SWIPE LEFT / RIGHT     Change lanes\nSWIPE UP                         Jump\nSWIPE DOWN                    Slide\nTAP                                     Attack\n\nMatch the arrows during close combat.\nIn the editor, drag with the mouse to swipe.");
        public void Settings() => Show("SETTINGS", "Use the SOUND button below to mute or enable audio.\n\nYour audio preference is saved between sessions.\nPress ESC or CLOSE to return to the menu.");
        public void ToggleSound()
        {
            AudioListener.volume = AudioListener.volume > 0 ? 0 : 1;
            PlayerPrefs.SetInt("MenuMuted", AudioListener.volume == 0 ? 1 : 0);
            PlayerPrefs.Save();
            UpdateSoundLabel();
        }
        private void UpdateSoundLabel() { soundLabel.text = AudioListener.volume > 0 ? "SOUND  ON" : "SOUND  OFF"; }
        public void CloseDialog() => dialog.SetActive(false);
    }
}
