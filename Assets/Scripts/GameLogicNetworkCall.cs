using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.SocialPlatforms.Impl;

namespace Bray
{
    public class GameLogicNetworkCall : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private List<TMP_Text> playerName = new List<TMP_Text>();
        [SerializeField]
        private List<TMP_Text> showName = new List<TMP_Text>();
        [SerializeField]
        private Image bgImage;
        [SerializeField]
        private RectTransform pointsPanel;
        public TMP_Text winnerMsg;
        public PassCard passCard;
        public GameObject cardPrefab;
        public HorizontalLayoutGroup layoutGroup;
        public RectTransform layoutWidth;
        private List<string> cardFamily = new() { "hearts", "spades", "clubs", "diamonds" };
        private List<string> cardName = new() { "2", "3", "4", "5", "6", "7", "8", "9", "10", "ace", "jack", "queen", "king" };
        public List<Sprite> deck = new();
        public List<Sprite> playerHand = new();
        public new PhotonView photonView;
        private Dictionary<int, bool> playerReadyStatus = new Dictionary<int, bool>();
        private int readyPlayers = 0;
        private int localPlayerIndex;
        public List<Image> cardLocation = new List<Image>();
        public List<int> players = new();
        public List<string> cards = new();
        public List<TMP_Text> points = new();
        private static bool firstRound = true; 
        public bool cardPassed = false;
        public int handOwner;
        public int brayOwner;
        public int noCardThrown = 0;
        public int currentTurn = 1;
        public int round = 0;
        public int size = 0;
        public bool hasCard = false;
        public bool hasBray = false;
        public TMP_Text roundText;
        public RectTransform roundTransform;
        public float screenEdgeOffset = 50f; 
        public float slideSpeed = 500f; 
        public float visibleDuration = 2f;
        private Vector2 offScreenRight;
        private Vector2 onScreenCenter;
        private Vector2 offScreenLeft;
        private static int roundNum = 1;
        public static Dictionary<string, int> suitOrder = new()
        {
            { "clubs", 1 }, { "diamonds", 2 }, { "hearts", 3 }, { "spades", 4 }
        };

        public static Dictionary<string, int> valueOrder = new()
        {
            { "ace", 14 }, { "2", 2 }, { "3", 3 }, { "4", 4 }, { "5", 5 },
            { "6", 6 }, { "7", 7 }, { "8", 8 }, { "9", 9 }, { "10", 10 },
            { "jack", 11 }, { "queen", 12 }, { "king", 13 }
        };

        private void Awake()
        {
            Screen.SetResolution(Screen.width, Screen.height, false);
            photonView = PhotonView.Get(this);
            foreach (Image card in cardLocation)
            {
                card.color = Color.clear;
            }
            if (photonView == null)
            {
                Debug.LogError("PhotonView is not attached to this GameObject.");
                return;
            }
            foreach (string card in cardFamily)
            {
                foreach (string name in cardName)
                {
                    string path = $"cards/{name}_of_{card}";
                    Sprite cardSprite = Resources.Load<Sprite>(path);
                    if (cardSprite != null)
                    {   
                        deck.Add(cardSprite);
                    }
                    else
                    {
                        Debug.LogError($"Card sprite not found at path: {path}");
                    }
                }
            }
            for(int i = 1; i < 5; i++)
            {
                PlayerPrefs.SetInt($"Player{i}", 0);
                PlayerPrefs.Save();
            }
            //if (PlayerPrefs.HasKey("NickName"))
            //{
            //    playerName[PhotonNetwork.LocalPlayer.ActorNumber - 1].text = PlayerPrefs.GetString("NickName");
            //}
            //else
            //{
            //    playerName[PhotonNetwork.LocalPlayer.ActorNumber - 1].text = $"Player {PhotonNetwork.LocalPlayer.ActorNumber}";
            //}
            
        }
        private void Start()
        {
            setScreenResponsiveness(Screen.width, Screen.height);
            SetPlayerNames();
            SetNames();
            if (PhotonNetwork.IsMasterClient)
            {
                Shuffle(deck);
                DealCards();
            }
            RectTransform rectTransform = layoutGroup.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Screen.width - ((Screen.width * 10)/100), Screen.height - ((Screen.height * 80)/100));
            foreach (Transform child in rectTransform)
            {
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                {
                    childRect.sizeDelta = new Vector2(((Screen.width - ((Screen.width * 10)/13)) / 100) , Screen.height - ((Screen.height * 80) / 100));
                }
            }
            layoutGroup.spacing = 10f;
            localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            if (firstRound)
            {
                firstRound = false;
                foreach (TMP_Text text in points)
                {
                    text.text = "0";
                }
            }
            else
            {
                points[0].text = PlayerPrefs.GetString("Player1Score", "0");
                points[1].text = PlayerPrefs.GetString("Player2Score", "0");
                points[2].text = PlayerPrefs.GetString("Player3Score", "0");
                points[3].text = PlayerPrefs.GetString("Player4Score", "0");
            }
            roundText.text = $"Round {roundNum}";
            float screenWidth = Screen.width;
            offScreenRight = new Vector2(screenWidth + screenEdgeOffset, roundTransform.anchoredPosition.y);
            onScreenCenter = new Vector2(0, roundTransform.anchoredPosition.y);
            offScreenLeft = new Vector2(-screenWidth - screenEdgeOffset, roundTransform.anchoredPosition.y);
            StartCoroutine(SlideTextAnimation());
        }
        [PunRPC]
        private void SetPlayerNames(int localActorNumber)
        {
            List<string> orderedNames = new List<string>();
            int totalPlayers = PhotonNetwork.PlayerList.Length;
            for (int i = 1; i <= totalPlayers; i++)
            {
                int targetActorNumber = (localActorNumber + i - 1) % totalPlayers + 1;
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player.ActorNumber == targetActorNumber)
                    {
                        orderedNames.Add(player.NickName);
                        break;
                    }
                }
            }
            for (int i = 0; i < showName.Count && i < orderedNames.Count; i++)
            {
                showName[i].text = orderedNames[i];
                Debug.Log($"Position {i + 1}: {orderedNames[i]}");
            }
        }
        public void SetNames()
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("SetPlayerNames", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        private void setScreenResponsiveness(int width, int height)
        {
            Screen.SetResolution(width, height, false);
            if (bgImage != null)
            {
                bgImage.rectTransform.sizeDelta = new Vector2(width, height);
            }
            Debug.LogWarning($"{width}, {height}");
            pointsPanel.anchoredPosition = new Vector2(width - ( width * 20 )/ 100, height - ( height * 10 )/ 100);
        }
        private void Update()
        {
            
            setScreenResponsiveness(Screen.width, Screen.height);
            foreach (TMP_Text text in points)
            {
                text.ForceMeshUpdate();
            }
        }
        private static void Shuffle<T>(List<T> deck)
        {
            System.Random rnd = new System.Random();
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }
        private void SetPlayerNames()
        {
            foreach(var player in PhotonNetwork.PlayerList)
            {
                int actorNumber = player.ActorNumber;
                if (player.CustomProperties.ContainsKey("NickName"))
                {
                    string playerNickname = (string)player.CustomProperties["NickName"]; 
                    if (actorNumber - 1 < playerName.Count)
                    {
                        playerName[actorNumber - 1].text = playerNickname;
                        player.NickName = playerNickname;
                    }
                    else
                    {
                        Debug.LogWarning("playerName list does not have enough elements for all players.");
                    }
                }
                else
                {
                    playerName[actorNumber - 1].text = PhotonNetwork.LocalPlayer.NickName;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "NickName", PlayerPrefs.GetString("NickName") } });
                }
            }
        }
        private void DealCards()
        {
            int totalPlayers = PhotonNetwork.PlayerList.Length;
    
            if (totalPlayers != 4)
            {
                Debug.LogError("Exactly 4 players are required to deal cards.");
                return;
            }

            if (deck.Count < 52)
            {
                Debug.LogError("Not enough cards in the deck. A total of 52 cards are required.");
                return;
            }

            for (int i = 0; i < totalPlayers; i++)
            {
                List<Sprite> hand = deck.GetRange(i * 13, 13);
                List<string> handNames = hand.Select(sprite => sprite.name).ToList();

                if (i == 0)
                {
                    DistributeCards(handNames.ToArray());
                }
                else
                {
                    photonView.RPC("DistributeCards", PhotonNetwork.PlayerList[i], handNames.ToArray());
                }
            }
        }

        [PunRPC]
        private void DistributeCards(string[] handNames)
        {
            List<Sprite> hand = handNames.Select(name => Resources.Load<Sprite>("cards/" + name)).ToList();
            playerHand = hand;
            photonView.RPC("DisplayCardsRPC", RpcTarget.All);
        }

        [PunRPC]
        private void DisplayCardsRPC()
        {
            DisplayCards();
        }
        //public void DisplayCards()
        //{
        //    foreach (Transform child in layoutGroup.transform)
        //    {
        //        Destroy(child.gameObject);
        //    }

        //    List<Image> cards = new List<Image>();

        //    for (int i = 0; i < playerHand.Count; i++)
        //    {
        //        GameObject cardInstance = Instantiate(cardPrefab, layoutGroup.transform);
        //        Image cardImage = cardInstance.GetComponent<Image>();
        //        cardImage.sprite = playerHand[i];
        //        RectTransform rectTransform = cardInstance.GetComponent<RectTransform>();
        //        rectTransform.sizeDelta = new Vector2(120, 200);
        //        cards.Add(cardImage);
        //    }
        //    layoutWidth.sizeDelta = new Vector2(playerHand.Count * 13, 200);
        //    CompareCardsAndArrange(cards);
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        //}
        public void DisplayCards()
        {
            foreach (Transform child in layoutGroup.transform)
            {
                Destroy(child.gameObject);
            }
            List<Image> cards = new List<Image>();
            float availableWidth = Screen.width - (Screen.width * 0.10f); 
            float cardWidth = availableWidth / 13f;
            float andjustedWidth = cardWidth * playerHand.Count;
            for (int i = 0; i < playerHand.Count; i++)
            {
                GameObject cardInstance = Instantiate(cardPrefab, layoutGroup.transform);
                Image cardImage = cardInstance.GetComponent<Image>();
                cardImage.sprite = playerHand[i];
                RectTransform rectTransform = cardInstance.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(cardWidth, 200); 
                cards.Add(cardImage);
            }
            layoutWidth.sizeDelta = new Vector2(andjustedWidth, 200); 
            CompareCardsAndArrange(cards);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }

        public void CompareCardsAndArrange(List<Image> cards)
        {
            cards.Sort((card1, card2) =>
            {
                string[] card1Data = card1.sprite.name.Split('_');
                string[] card2Data = card2.sprite.name.Split('_');

                string card1Value = card1Data[0];
                string card1Suit = card1Data[2];
                string card2Value = card2Data[0];
                string card2Suit = card2Data[2];

                int suitComparison = suitOrder[card1Suit].CompareTo(suitOrder[card2Suit]);
                if (suitComparison != 0)
                {
                    return suitComparison;
                }

                return valueOrder[card1Value].CompareTo(valueOrder[card2Value]);
            });

            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].transform.SetSiblingIndex(i);
            }
        }
        public Sprite getCardSprite(string cardName)
        {
            string path = $"cards/{cardName}";
            Sprite cardSprite = Resources.Load<Sprite>(path);
            if (cardSprite == null)
            {
                Debug.LogError($"Card sprite not found at path: {path}");
            }
            return cardSprite;
        }

        public void addRecivedCard(Sprite cardSprite)
        {
            playerHand.Add(cardSprite);
        }
        public void RemovePassedCards(Sprite cardSprite)
        {
            playerHand.Remove(cardSprite);
        }
        public void DestroySprite(Sprite sprite)
        {
            foreach (Transform child in layoutGroup.transform)
            {
                Image image = child.GetComponent<Image>();
                if (image.sprite == sprite)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }

        [PunRPC]
        public void PlayerReady(int playerId)
        {
            playerReadyStatus[playerId] = true;
            readyPlayers++;
            if (readyPlayers == 4)
            {
                photonView.RPC("enableConfirmButton", RpcTarget.All);
            }
        }
        [PunRPC]
        public void enableConfirmButton()
        {
            PassCard.Instance.sendCardsToNextPlayer();
        }

        [PunRPC]
        public void RecivePassedCards(string card1, string card2, string card3, string card4, string card5)
        {
            playerHand.Add(Resources.Load<Sprite>("cards/" + card1));
            playerHand.Add(Resources.Load<Sprite>("cards/" + card2));
            playerHand.Add(Resources.Load<Sprite>("cards/" + card3));
            playerHand.Add(Resources.Load<Sprite>("cards/" + card4));
            playerHand.Add(Resources.Load<Sprite>("cards/" + card5));
            DisplayCards();
        }
        [PunRPC]
        public void OnCardThrown(int playerIndex, string spriteName)
        {
            if (cardLocation == null || cardLocation.Count < 4)
            {
                Debug.LogError("cardLocation list is not properly initialized or has fewer than 4 elements.");
                return;
            }
            if(playerHand.Count < 0)
            {
                return;
            }

            int relativePosition = (playerIndex - localPlayerIndex + 4) % 4;
            if (relativePosition < 0 || relativePosition >= cardLocation.Count)
            {
                Debug.LogError($"Invalid relativePosition: {relativePosition}. It must be between 0 and {cardLocation.Count - 1}.");
                return;
            }

            Image targetPosition = cardLocation[relativePosition];
            Sprite cardSprite = null;

            if (targetPosition != null)
            {
                cardSprite = Resources.Load<Sprite>($"cards/{spriteName}");

                if (cardSprite != null)
                {
                    targetPosition.color = Color.white;
                    targetPosition.sprite = cardSprite;
                }
                else
                {
                    Debug.LogError($"Sprite not found: {spriteName}");
                }
            }
            else
            {
                Debug.LogError("Target position is null.");
            }
            playerHand.Remove(cardSprite);
            DisplayCards();
            SetCard(spriteName, playerIndex);
            passCard.SetA(cards.Count+1);
        }

        public void SetCard(string spriteName, int playerIndex)
        {
            if (spriteName != null)
            {
                cards.Add(spriteName);
                players.Add(playerIndex);
            }

            if (players.Count == 4 && cards.Count == 4)
            {
                photonView.RPC("HandOwner", RpcTarget.All);
            }
        }
        [PunRPC]
        public void HandOwner()
        {
            Dictionary<int, string> cardThrown = new Dictionary<int, string>();
            string[] cardFamily = new string[cards.Count];
            string[] cardVal = new string[cards.Count];
            int currentPoints = 0;
            int earnedPoints = 0;

           

            if (cards.Count == 4 && players.Count == 4) 
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    cardThrown[players[i]] = cards[i];
                    cardFamily[i] = cards[i].Split('_')[2];
                    cardVal[i] = cards[i].Split('_')[0];
                }

                string leadingFamily = cardFamily[0];
                int maxVal = valueOrder[cardVal[0]];
                handOwner = players[0];
                
                for (int i = 1; i < cards.Count; i++)
                {
                    if (cardFamily[i] == leadingFamily && valueOrder[cardVal[i]] > maxVal)
                    {
                        maxVal = valueOrder[cardVal[i]];
                        handOwner = players[i];
                        PlayerPrefs.SetInt($"Player{handOwner+1}", 1);
                        PlayerPrefs.Save();
                    }
                }
                if (!string.IsNullOrEmpty(points[handOwner].text))
                {
                    currentPoints =  int.Parse(points[handOwner].text);
                }
                for (int i = 0; i < cardFamily.Length; i++)
                {
                    if (cardVal[i] == "queen" && cardFamily[i] == "spades")
                    {
                        if (!string.IsNullOrEmpty(points[handOwner].text))
                        {
                            if (int.Parse(points[handOwner].text) >= 75)
                            {
                                earnedPoints += 0;
                            }
                            else
                            {
                                earnedPoints += 12;
                            }
                            brayOwner = handOwner;
                        }
                    }
                    if (cardFamily[i] == "hearts") earnedPoints++;
                }
                
                int point = currentPoints + earnedPoints;
                AddPointsRpc(handOwner, point);
                StartCoroutine(WaitAndRemove());
                currentTurn = handOwner;
                round++;
                if (round >= 13)
                {
                    round = 0;
                    roundNum++;
                    reShuffleAndDealCards();
                }
                noCardThrown++;
                cards.Clear();
                players.Clear();
                playerReadyStatus.Clear();
                passCard.SetA(cards.Count + 1);
            }
        }
        private void checkPenalities()
        {
            int penaltyPoints = -5;
            for (int i = 1; i < 5; i++)
            {
                if (PlayerPrefs.GetInt($"Player{i}") == 0)
                {
                    int currentPoints = PlayerPrefs.HasKey($"Player{i}Score") ? int.Parse(PlayerPrefs.GetString($"Player{i}Score")) : 0;
                    PlayerPrefs.SetString($"Player{i}Score", (currentPoints+penaltyPoints).ToString());
                    PlayerPrefs.Save();
                }
            }
            if (roundNum >= 3)
            {
                for (int i = 1; i < 5; i++)
                {
                    List<int> roundPoints = new List<int>();
                    for (int j = 1; j <= roundNum; j++)
                    {
                        roundPoints.Add(int.Parse(PlayerPrefs.GetString($"Player{i + 1}Round{j}")));
                    }
                    if (roundPoints.Count >= 3)
                    {
                        for (int k = 0; k < roundPoints.Count; k++)
                        {
                            if (roundPoints[k] == roundPoints[k + 1] && roundPoints[k + 1] == roundPoints[k + 2])
                            {
                                int currentPoints = PlayerPrefs.HasKey($"Player{k + 1}Score") ? int.Parse(PlayerPrefs.GetString($"Player{k + 1}Score")) : 0;
                                Debug.Log($"Player1Score: {PlayerPrefs.GetString("Player1Score", "0")}");
                                currentPoints -= penaltyPoints;
                                Debug.LogWarning($"Current Point = {currentPoints} \n PointWithPenalty = {currentPoints -= penaltyPoints}");
                                PlayerPrefs.SetString($"Player{k + 1}Score", currentPoints.ToString());
                                PlayerPrefs.Save();
                            }
                        }
                    }
                }
            }
        }
        private void reShuffleAndDealCards()
        {
            StartCoroutine(ReshuffleAndDealRoutine());
        }
        private IEnumerator ReshuffleAndDealRoutine()
        {
            yield return new WaitForSeconds(2f);
            for(int i = 0; i < points.Count; i++)
            {
                if (!string.IsNullOrEmpty(points[i].text))
                {
                    PlayerPrefs.SetString($"Player{i+1}Score", points[i].text);
                    PlayerPrefs.SetString($"Player{i + 1}Round{roundNum}", points[i].text);
                    PlayerPrefs.Save();
                }
                else
                {
                    PlayerPrefs.SetString($"Player{i+1}Score", $"0");
                    PlayerPrefs.Save();
                }
            }
            checkPenalities();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        private IEnumerator WaitAndRemove()
        {
            yield return new WaitForSeconds(1f);
            photonView.RPC("RemoveCards", RpcTarget.All);
        }
        private void AddPointsRpc(int handOwner, int point)
        {
            photonView.RPC("AddPoints", RpcTarget.All, handOwner, point);
        }
        [PunRPC]
        public void RemoveCards()
        {
            foreach (Image card in cardLocation)
            {
                card.color = Color.clear;
                card.sprite = null;
            }
        }
        [PunRPC]
        public void AddPoints(int handOwner, int point) 
        {
            switch (handOwner)
            {
                case 0:
                    points[0].text = $"{point}";
                    break;
                case 1:
                    points[1].text = $"{point}";
                    break;
                case 2:
                    points[2].text = $"{point}";
                    break;
                case 3:
                    points[3].text = $"{point}";
                    break;
            }
        }
        [PunRPC]
        public void SetTurn()
        {
            currentTurn = (currentTurn % 4) + 1;
        }
        [PunRPC]
        public void CheckThrowCard()
        {
            string leadingSuit = cards[0].Split('_')[2];
            bool aceOrKingThrown = checkForAceOrKing(leadingSuit);
            checkAvailabilityAndBray(leadingSuit, aceOrKingThrown);
        }
        private bool checkForAceOrKing(string leadingSuit)
        {
            if (leadingSuit == "spades")
            {
                foreach (string card in cards)
                {
                    string cardValue = card.Split('_')[0];
                    if (cardValue == "ace" || cardValue == "king")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void checkAvailabilityAndBray(string leadingSuit, bool aceOrKingThrown)
        {
            hasCard = false;
            hasBray = false;

            foreach (Sprite pH in playerHand)
            {
                string cardSuit = pH.name.Split('_')[2];
                if (cardSuit == leadingSuit)
                {
                    hasCard = true;
                    break;
                }
            }
            if (aceOrKingThrown && leadingSuit == "spades")
            {
                foreach (Sprite pH in playerHand)
                {
                    string cardValue = pH.name.Split('_')[0];
                    string cardSuit = pH.name.Split('_')[2];

                    if (cardSuit == "spades" && cardValue == "queen")
                    {
                        hasBray = true;
                        break;
                    }
                }
            }
            else if (!hasCard)
            {
                foreach(Sprite pH in playerHand)
                {
                    if(pH.name == "queen_of_spades")
                    {
                        hasBray = true;
                    }
                }
            }
        }           
        IEnumerator SlideTextAnimation()
        {
            yield return StartCoroutine(SlideToPosition(offScreenRight, onScreenCenter));
            yield return new WaitForSeconds(visibleDuration);
            yield return StartCoroutine(SlideToPosition(onScreenCenter, offScreenLeft));
        }

        IEnumerator SlideToPosition(Vector2 start, Vector2 end)
        {
            roundTransform.anchoredPosition = start;
            float elapsedTime = 0;
            float totalTime = Mathf.Abs((end - start).x) / slideSpeed;

            while (elapsedTime < totalTime)
            {
                roundTransform.anchoredPosition = Vector2.Lerp(start, end, elapsedTime / totalTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            roundTransform.anchoredPosition = end;
        }
    }
}