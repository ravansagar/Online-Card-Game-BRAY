using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bray
{
    public class PassCard : MonoBehaviourPunCallbacks, IPointerClickHandler
    {
        public TMP_Text msg;
        private RectTransform cardRectTransform = new RectTransform();
        private float moveDistance;
        public GameObject cardPrefab;
        private static Dictionary<Sprite, bool> cardState = new();
        public GameLogicNetworkCall localPlayerCards;
        public GameObject localPlayer;
        public static PassCard Instance;
        public int track = 0;
        public List<Image> cardLocation = new List<Image>();
        private int localPlayerIndex;
        public List<int> players = new();
        public List<string> cards = new();
        public static bool first = true;
        public static int a = 1;

        private void Awake()
        {
            Instance = this;
            msg.text = "";    
        }
        private void Start()
        {
            localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            cardRectTransform = GetComponent<RectTransform>();
            track = 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!localPlayerCards.cardPassed)
            {
                cardPassPointer(eventData);
            }
            else
            {
                AttemptToThrowCard(eventData);
            }
        }

        public void cardPassPointer(PointerEventData eventData)
        {
            Image clickedCardImage = eventData.pointerPress.GetComponent<Image>();
            if (clickedCardImage != null)
            {
                Sprite clickedCardSprite = clickedCardImage.sprite;
                if (cardState.ContainsKey(clickedCardSprite))
                {
                    if (cardState[clickedCardSprite] == true)
                    {
                        MoveCardDown(clickedCardImage);
                    }
                    else
                    {
                        if (localPlayerCards.size < 5)
                        {
                            MoveCardUp(clickedCardImage);
                        }
                        else
                        {
                            msg.text = "Maximum number of cards is selected.";
                            StartCoroutine(clearMsg(5f));
                        }
                    }
                }
                else
                {
                    if (localPlayerCards.size < 5)
                    {
                        MoveCardUp(clickedCardImage);
                    }
                    else
                    {
                        msg.text = "Maximum number of cards is selected.";
                        StartCoroutine(clearMsg(5f));
                    }
                }
            }
        }
        public void AttemptToThrowCard(PointerEventData eventData)
        {
            if (IsMyTurn())
            {
#if UNITY_ANDROID || UNITY_IOS
    Handheld.Vibrate();
#endif
                bool check = cardThrowPointer(eventData);
                if (check)
                {
                    NextTurn();
                    return;
                }
                else
                {
                    Debug.Log("You must play a card of the correct suit.");
                }
            }
            else
            {
                Debug.Log("It's not your turn!");
            }
        }
        private bool IsMyTurn()
        {
            return PhotonNetwork.LocalPlayer.ActorNumber == localPlayerCards.currentTurn;
        }
        private void NextTurn()
        {
            localPlayerCards.photonView.RPC("SetTurn", RpcTarget.All);
        }
        public bool cardThrowPointer(PointerEventData eventData)
        {
            Image img = eventData.pointerPress.GetComponent<Image>();
            bool canThrow = true;
            bool hasToThrowBray = false;
            if(img == null)
            {
                return false;
            }
            if(a != 1) 
            {
                if (a == 4)
                {
                    a = 1;
                }
                localPlayerCards.photonView.RPC("CheckThrowCard", RpcTarget.All);
                string leadingSuit = localPlayerCards.cards[0].Split('_')[2];
                string selectedCardSuit = img.sprite.name.Split('_')[2];
                if (localPlayerCards.hasBray)
                {
                    hasToThrowBray = true;
                }
                else if (selectedCardSuit == leadingSuit)
                {
                    canThrow = true;  
                }
                else if (localPlayerCards.hasCard)
                {
                    canThrow = false;
                }
                else
                {
                    canThrow = true;
                }
             
            }
            if (hasToThrowBray)
            {
                if (img.sprite.name == "queen_of_spades")
                {
                    CardThrow(img);
                    return true;
                }
                else
                {
                    Debug.Log("You must throw the Queen of Spades!");
                    return false; 
                }
            }
            else if (!hasToThrowBray && canThrow)
            {
                CardThrow(img);
            }

            return canThrow;
        }
        public void SetA(int val)
        {
            a = val;
        }
        void CardThrow(Image card)
        {
            localPlayerCards.photonView.RPC("OnCardThrown", RpcTarget.All, localPlayerIndex, card.sprite.name);
        }

        IEnumerator clearMsg(float delay)
        {
            yield return new WaitForSeconds(delay);
            msg.text = "";
        }

        void MoveCardUp(Image cardImage)
        {
            if (cardState.Count == 5) return;
            moveDistance = cardRectTransform.rect.height * 0.2f;
            cardRectTransform.anchoredPosition += new Vector2(0, moveDistance);
            if (!cardState.ContainsKey(cardImage.sprite))
            {
                cardState.Add(cardImage.sprite, true);
                localPlayerCards.size++;
            }
        }

        void MoveCardDown(Image cardImage)
        {
            if (cardState.Count == 0) return;
            moveDistance = cardRectTransform.rect.height * 0.2f;
            cardRectTransform.anchoredPosition -= new Vector2(0, moveDistance);
            cardState[cardImage.sprite] = false;
            cardState.Remove(cardImage.sprite);
            localPlayerCards.size--;
        }

        void Update()
        {
            CheckCardSelection();
        }
        public void CheckCardSelection()
        {
            if (localPlayerCards.size == 5)
            {
                onCardSelected();
                localPlayerCards.size = 0;
            }
            if (localPlayerCards.size < 5)
            {
                msg.text = "";
            }
        }
        Player getNextPlayer(Player currentPlayer)
        {
            Player[] players = PhotonNetwork.PlayerList;
            int currentIndex = System.Array.IndexOf(players, currentPlayer);
            int nextIndex = (currentIndex + 1) % players.Length;
            return players[nextIndex];
        }

        public void sendCardsToNextPlayer()
        {
            localPlayerCards.cardPassed = true;
            clearLocalSelectedCards();
        }

        void clearLocalSelectedCards()
        {
            List<string> selectedCards = new List<string>();
            foreach (Sprite sprite in cardState.Keys)
            {
                if (cardState[sprite] == true)
                {
                    selectedCards.Add(sprite.name);
                }
            }
            if (selectedCards.Count == 5)
            {
                Player nextPlayer = getNextPlayer(PhotonNetwork.LocalPlayer);
                localPlayerCards.photonView.RPC("RecivePassedCards", nextPlayer, selectedCards[0], selectedCards[1], selectedCards[2], selectedCards[3], selectedCards[4]);
            }
            else
            {
                Debug.LogError($"Expected 5 selected cards, but found {selectedCards.Count}. Unable to pass cards.");
            }
            RemoveAfterPass();
            cardState.Clear();
            localPlayerCards.size = 0;
        }

        public void onCardSelected()
        {
            localPlayerCards.photonView.RPC("PlayerReady", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public void RemoveAfterPass()
        {
            foreach (Sprite sprite in cardState.Keys)
            {
                if (cardState[sprite] == true)
                {
                    if (localPlayerCards.playerHand.Contains(sprite))
                    {
                        localPlayerCards.playerHand.Remove(sprite);
                    }
                    else
                    {
                        Debug.LogWarning($"{sprite.name} not found in player hand");
                    }
                }
            }
            cardState.Clear();
            localPlayerCards.size = 0;
       }
    }
}