using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bray
{
    public class HandManager : MonoBehaviourPunCallbacks, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData) 
        {
            Image img  = eventData.pointerPress.GetComponent<Image>();
            if (img != null)
            {
                Debug.LogWarning($"Image name: {img.sprite.name}");
            }
            else
            {
                Debug.LogWarning("Image not found...");
            }
        }
        //public Image[] cardPositions;  
        //private int localPlayerIndex;  
        //public HorizontalLayoutGroup layoutGroup;  
        //private LocalPlayerCards localPlayerCards;

        //private void Awake()
        //{
        //    Debug.LogWarning("Hand Manager is working...");
        //}
        //private void Start()
        //{
        //    localPlayerCards = FindObjectOfType<LocalPlayerCards>();
        //    if (localPlayerCards != null)
        //        Debug.LogWarning("Component found");
        //    else
        //        Debug.LogError("LocalPlayerCards component not found");

        //    localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        //}

        //public void OnPointerClick(PointerEventData eventData)
        //{

        //    Image clickedCard = eventData.pointerPress.GetComponent<Image>();

        //    if (clickedCard != null && localPlayerCards.playerHand.Contains(clickedCard.sprite))
        //    {
        //        Debug.Log("Card clicked...");
        //        int cardIndex = localPlayerCards.playerHand.IndexOf(clickedCard.sprite);
        //        layoutGroup.enabled = false; 
        //        localPlayerCards.photonView.RPC("OnCardThrown", RpcTarget.All, localPlayerIndex, cardIndex, clickedCard);
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Clicked card not found in player’s hand.");
        //    }
        //}

        //[PunRPC]
        //public void OnCardThrown(int playerIndex, int cardIndex, Image card)
        //{
        //    Debug.Log("OnCardThrown called");
        //    // Calculate the position relative to the local player
        //    int relativePosition = (playerIndex - localPlayerIndex + 4) % 4;
        //    Image targetPosition = cardPositions[relativePosition];

        //    // Check if the target position exists and set sprite
        //    if (targetPosition != null && localPlayerCards != null)
        //    {
        //        Debug.Log($"Card name: {card.sprite.name}");
        //        targetPosition.sprite = localPlayerCards.playerHand[cardIndex];
        //        targetPosition.gameObject.SetActive(true);
        //        cardPositions[relativePosition].sprite = card.sprite;
        //        Debug.Log("Card seted");
        //        //StartCoroutine(AnimateCardToCenter(targetPosition));
        //    }
        //    else
        //    {
        //        Debug.LogError("Target position or local player hand not found.");
        //    }
        //}

        //private IEnumerator AnimateCardToCenter(Image targetCard)
        //{
        //    // Ensure layout group is disabled during animation
        //    layoutGroup.enabled = false;

        //    // Set up animation variables
        //    Vector3 originalPosition = targetCard.transform.position;
        //    Vector3 targetPosition = cardPositions[0].transform.position;  // Adjust to desired center position
        //    float duration = 0.5f;
        //    float elapsedTime = 0;

        //    while (elapsedTime < duration)
        //    {
        //        // Smoothly interpolate position
        //        targetCard.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
        //        elapsedTime += Time.deltaTime;
        //        yield return null;
        //    }

        //    // Ensure card is at target position after animation
        //    targetCard.transform.position = targetPosition;

        //    // Re-enable layout if needed
        //    layoutGroup.enabled = true;
        //}
    }
}



//using Photon.Pun;
//using Photon.Realtime;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//namespace Bray
//{
//    public class HandManager : MonoBehaviourPunCallbacks, IPointerClickHandler
//    {
//        public Image[] cardPositions;
//        private int localPlayerIndex;
//        public HorizontalLayoutGroup layoutGroup;
//        private LocalPlayerCards localPlayerCards;
//        //private List<Sprite> playerCards = new();
//        public List<string> cardNames;

//        private void Start()
//        {
//            localPlayerCards = FindObjectOfType<LocalPlayerCards>();
//            if (localPlayerCards != null) 
//            {
//                Debug.Log("Component found");
//            }
//            else
//            {
//                Debug.Log("Component Dosen't found");
//            }

//            localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
//        }
//        public void OnPointerClick(PointerEventData eventData)
//        {
//            Image clickedCard = eventData.pointerPress.GetComponent<Image>();
//            if (clickedCard != null && localPlayerCards.playerHand.Contains(clickedCard.sprite))
//            {
//                int cardIndex = localPlayerCards.playerHand.IndexOf(clickedCard.sprite);
//                layoutGroup.enabled = false;
//                photonView.RPC("OnCardThrown", RpcTarget.All, localPlayerIndex, cardIndex);
//            }
//        }

//        [PunRPC]
//        public void OnCardThrown(int playerIndex, int cardIndex)
//        {
//            int relativePosition = (playerIndex - localPlayerIndex + 4) % 4;
//            Image targetPosition = cardPositions[relativePosition];
//            targetPosition.sprite = localPlayerCards.playerHand[cardIndex];
//            targetPosition.gameObject.SetActive(true);
//            StartCoroutine(AnimateCardToCenter(targetPosition));
//        }

//        private IEnumerator AnimateCardToCenter(Image targetCard)
//        {
//            Vector3 originalPosition = targetCard.transform.position;
//            Vector3 targetPosition = new Vector3(0, 0, 0);
//            float duration = 0.5f;
//            float elapsedTime = 0;
//            while (elapsedTime < duration)
//            {
//                targetCard.transform.position = Vector3.Lerp(originalPosition, targetPosition, (elapsedTime / duration));
//                elapsedTime += Time.deltaTime;
//                yield return null;
//            }
//            targetCard.transform.position = targetPosition;
//        }
//public void OnPointerClick(PointerEventData eventData) 
//{
//    Image clickedCard = eventData.pointerPress.GetComponent<Image>();
//    if(clickedCard != null)
//    {
//        Sprite cardSprite = clickedCard.sprite;
//        card1.sprite = cardSprite;
//    }
//}
//private void cardAssignment()
//{
//    cardNames.Clear();
//    if (card1 == null || card2 == null || card3 == null || card4 == null)
//    {
//        Debug.LogError("One or more Image component not found.");
//        return;
//    }
//    Sprite[] sprites = { card1.sprite, card2.sprite, card3.sprite, card4.sprite };
//    foreach (Sprite sp in sprites)
//    {
//        if (sp != null)
//        {
//            cardNames.Add(sp.name);
//        }
//        else
//        {
//            Debug.LogError("One or more sprite is not assigned.");
//            return;
//        }
//    }
//    foreach (Sprite sp in sprites)
//    {
//        if (sp != null)
//        {
//            cardNames.Add(sp.name);
//        }
//        else
//        {
//            Debug.LogError("One or more sprite is not assigned.");
//            return;
//        }
//    }
//if (card1 != null && card2 != null && card3 != null && card4 != null)
//{
//    Sprite sp1 = card1.sprite;
//    Sprite sp2 = card2.sprite;
//    Sprite sp3 = card3.sprite;
//    Sprite sp4 = card4.sprite;
//    if (sp1 != null && sp2 != null && sp3 != null && sp4 != null)
//    {
//        cardNames.Add(sp1.name);
//        cardNames.Add(sp2.name);
//        cardNames.Add(sp3.name);
//        cardNames.Add(sp4.name);
//    }
//    else
//    {
//        Debug.LogError("One or more sprite is not assigned");
//    }
//}
//else
//{
//    Debug.LogError("One or more image component not found");
//}
//}
//private void winnerCard(List<string> cardNames)
//{
//    List<int> cardValues = new List<int>();
//    string suit = cardNames[0].Split('_')[2];
//    bool sameSuit = cardNames.All(card => card.Split('_')[2] == suit);
//    if (sameSuit)
//    {
//        foreach (string card in cardNames)
//        {
//            string rank = card.Split('_')[0];
//            if (LocalPlayerCards.valueOrder.TryGetValue(rank, out int value))
//            {
//                cardValues.Add(value);
//            }
//            else
//            {
//                Debug.LogError($"Rank '{rank}' not found in valueOrder dictionary.");
//                return;
//            }
//        }
//        int maxValue = cardValues.Max();
//        Debug.Log($"Highest card value: {maxValue}");
//    }
//    else
//    {
//        List<int> sameSuitValues = new List<int>();
//        foreach (string card in cardNames)
//        {
//            string cardSuit = card.Split('_')[2];
//            if (cardSuit == suit)
//            {
//                string rank = card.Split('_')[0];
//                if (LocalPlayerCards.valueOrder.TryGetValue(rank, out int value))
//                {
//                    sameSuitValues.Add(value);
//                }
//                else
//                {
//                    Debug.LogError($"Rank '{rank}' not found in valueOrder dictionary.");
//                    return;
//                }
//            }
//        }
//        if (sameSuitValues.Count > 0)
//        {
//            int maxSameSuitValue = sameSuitValues.Max();
//            Debug.Log($"Highest card is {maxSameSuitValue}");
//        }
//        else
//        {
//            Debug.Log("Highets card is card1....");
//        }
//    }
//private void winnerCard()
//{
//    string[] values = new string[4];
//    string[] families = new string[4];
//    int i = 0;
//    foreach (string cards in cardNames)
//    {
//        string[] value_with_name = cards.Split("_");
//        string value = value_with_name[0];
//        string family = value_with_name[2];
//        values[i] = value;
//        families[i] = family;
//        i++;
//    }
//    if (families[0] == families[1] && families[1] == families[2] && families[2] == families[3])
//    {

//    }
//}
//}
//}

