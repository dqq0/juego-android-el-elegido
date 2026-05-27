using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VNCreator
{
    public class VNCreator_DisplayUI : DisplayBase
    {
        [Header("Text")]
        public Text characterNameTxt;
        public Text dialogueTxt;
        [Header("Visuals")]
        public Image characterImg;
        public Image backgroundImg;
        [Header("Audio")]
        public AudioSource musicSource;
        public AudioSource soundEffectSource;
        [Header("Buttons")]
        public Button nextBtn;
        public Button previousBtn;
        public Button saveBtn;
        public Button menuButton;
        public Button restartBtn;
        [Header("Choices")]
        public Button choiceBtn1;
        public Button choiceBtn2;
        public Button choiceBtn3;
        [Header("End")]
        public GameObject endScreen;
        [Header("Main menu")]
        [Scene]
        public string mainMenu;

        private Coroutine visualCoroutine;
        private Vector2 originalCharPos;

        public AudioClip textBleepSound;
        private AudioSource bleepSource;
        private Coroutine displayCoroutine;

        void Start()
        {
            bleepSource = gameObject.AddComponent<AudioSource>();
            bleepSource.loop = true;
            bleepSource.volume = GameOptions.sfxVolume * 0.3f; // Un poco mas bajito para no molestar

            if (characterImg != null)
                originalCharPos = characterImg.rectTransform.anchoredPosition;

            nextBtn.onClick.AddListener(delegate { NextNode(0); });
            if(previousBtn != null)
                previousBtn.onClick.AddListener(Previous);
            if(saveBtn != null)
                saveBtn.onClick.AddListener(Save);
            if (menuButton != null)
                menuButton.onClick.AddListener(ExitGame);
            if (restartBtn != null)
                restartBtn.onClick.AddListener(RestartGame);

            if(choiceBtn1 != null)
                choiceBtn1.onClick.AddListener(delegate { NextNode(0); });
            if(choiceBtn2 != null)
                choiceBtn2.onClick.AddListener(delegate { NextNode(1); });
            if(choiceBtn3 != null)
                choiceBtn3.onClick.AddListener(delegate { NextNode(2); });

            endScreen.SetActive(false);

            StartDisplayCoroutine();
        }

        private void StartDisplayCoroutine()
        {
            if (displayCoroutine != null)
                StopCoroutine(displayCoroutine);
            
            if (bleepSource != null && bleepSource.isPlaying)
                bleepSource.Stop();

            displayCoroutine = StartCoroutine(DisplayCurrentNode());
        }

        protected override void NextNode(int _choiceId)
        {
            if (lastNode)
            {
                endScreen.SetActive(true);
                return;
            }

            base.NextNode(_choiceId);
            StartDisplayCoroutine();
        }

        IEnumerator DisplayCurrentNode()
        {
            bool changedCharacter = false;
            bool changedBackground = false;

            characterNameTxt.text = currentNode.characterName;
            if (currentNode.characterSpr != null)
            {
                if (characterImg.sprite != currentNode.characterSpr || characterImg.color.a == 0) 
                    changedCharacter = true;
                characterImg.sprite = currentNode.characterSpr;
            }
            else
            {
                characterImg.color = new Color(1, 1, 1, 0);
            }
            
            if(currentNode.backgroundSpr != null)
            {
                if (backgroundImg.sprite != currentNode.backgroundSpr) 
                    changedBackground = true;
                backgroundImg.sprite = currentNode.backgroundSpr;
            }
            else
            {
                Sprite prevBg = null;
                for (int i = loadList.Count - 1; i >= 0; i--)
                {
                    NodeData prevNode = story.GetCurrentNode(loadList[i]);
                    if (prevNode != null && prevNode.backgroundSpr != null)
                    {
                        prevBg = prevNode.backgroundSpr;
                        break;
                    }
                }

                if (prevBg != null && backgroundImg.sprite != prevBg)
                {
                    changedBackground = true;
                    backgroundImg.sprite = prevBg;
                }
            }

            if (changedCharacter || changedBackground)
            {
                if (visualCoroutine != null) StopCoroutine(visualCoroutine);
                visualCoroutine = StartCoroutine(AnimateVisuals(changedCharacter, changedBackground));
            }

            if (previousBtn != null)
                previousBtn.gameObject.SetActive(loadList.Count != 1);

            if (currentNode.choices <= 1) 
            {
                nextBtn.gameObject.SetActive(true);

                choiceBtn1.gameObject.SetActive(false);
                choiceBtn2.gameObject.SetActive(false);
                choiceBtn3.gameObject.SetActive(false);
            }
            else
            {
                nextBtn.gameObject.SetActive(false);

                bool isFinalChoice = currentNode.characterName == "SISTEMA_FINAL";
                int finalDecision = -1;
                
                if (isFinalChoice)
                {
                    int aptitud = PlayerStats.instance != null ? PlayerStats.instance.aptitud : 0;
                    int miedo = PlayerStats.instance != null ? PlayerStats.instance.miedo : 0;
                    
                    if (aptitud >= miedo + 5) finalDecision = 0; // Final 1 (Mucha aptitud)
                    else if (miedo >= aptitud + 5) finalDecision = 1; // Final 2 (Mucho miedo)
                    else finalDecision = 2; // Final 3 (Equilibrado / Neutral)
                }

                choiceBtn1.gameObject.SetActive(true);
                choiceBtn1.interactable = !isFinalChoice || finalDecision == 0;
                choiceBtn1.transform.GetChild(0).GetComponent<Text>().text = currentNode.choiceOptions[0];

                choiceBtn2.gameObject.SetActive(true);
                choiceBtn2.interactable = !isFinalChoice || finalDecision == 1;
                choiceBtn2.transform.GetChild(0).GetComponent<Text>().text = currentNode.choiceOptions[1];

                if (currentNode.choices == 3)
                {
                    choiceBtn3.gameObject.SetActive(true);
                    choiceBtn3.interactable = !isFinalChoice || finalDecision == 2;
                    choiceBtn3.transform.GetChild(0).GetComponent<Text>().text = currentNode.choiceOptions[2];
                }
                else
                {
                    choiceBtn3.gameObject.SetActive(false);
                }
            }

            if (currentNode.backgroundMusic != null)
                VNCreator_MusicSource.instance.Play(currentNode.backgroundMusic);
            if (currentNode.soundEffect != null)
                VNCreator_SfxSource.instance.Play(currentNode.soundEffect);

            dialogueTxt.text = string.Empty;
            if (GameOptions.isInstantText)
            {
                dialogueTxt.text = currentNode.dialogueText;
            }
            else
            {
                if (textBleepSound != null)
                {
                    bleepSource.clip = textBleepSound;
                    bleepSource.Play();
                }

                char[] _chars = currentNode.dialogueText.ToCharArray();
                string fullString = string.Empty;
                for (int i = 0; i < _chars.Length; i++)
                {
                    fullString += _chars[i];
                    dialogueTxt.text = fullString;
                    yield return new WaitForSeconds(0.01f/ GameOptions.readSpeed);
                }

                if (textBleepSound != null)
                {
                    bleepSource.Stop();
                }
            }
        }

        protected override void Previous()
        {
            base.Previous();
            StartDisplayCoroutine();
        }

        void ExitGame()
        {
            SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
        }

        void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        IEnumerator AnimateVisuals(bool animateChar, bool animateBg)
        {
            float duration = 0.5f;
            float elapsed = 0f;

            Color charColor = characterImg.color;
            Color bgColor = backgroundImg.color;
            Vector2 startCharPos = originalCharPos + new Vector2(0, -50f); // Empieza un poco mas abajo

            if (animateChar)
            {
                charColor.a = 0f;
                characterImg.color = charColor;
                characterImg.rectTransform.anchoredPosition = startCharPos;
            }
            if (animateBg)
            {
                bgColor.a = 0f;
                backgroundImg.color = bgColor;
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                if (animateChar)
                {
                    charColor.a = Mathf.Lerp(0f, 1f, t);
                    characterImg.color = charColor;
                    characterImg.rectTransform.anchoredPosition = Vector2.Lerp(startCharPos, originalCharPos, t);
                }

                if (animateBg)
                {
                    bgColor.a = Mathf.Lerp(0f, 1f, t);
                    backgroundImg.color = bgColor;
                }

                yield return null;
            }

            if (animateChar)
            {
                charColor.a = 1f;
                characterImg.color = charColor;
                characterImg.rectTransform.anchoredPosition = originalCharPos;
            }
            if (animateBg)
            {
                bgColor.a = 1f;
                backgroundImg.color = bgColor;
            }
        }
    }
}