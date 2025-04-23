using System.Collections.Generic;
using UnityEngine;

namespace Bray
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private Dictionary<int, List<Sprite>> playersWithCards = new Dictionary<int, List<Sprite>>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        public void AddPlayerCards(int playerId, List<Sprite> playerCards)
        {
            if (!playersWithCards.ContainsKey(playerId))
            {
                playersWithCards[playerId] = new List<Sprite>();
            }
            playersWithCards[playerId].AddRange(playerCards);
        }
        public Dictionary<int, List<Sprite>> GetAllPlayerCards()
        {
            return playersWithCards;
        }
        public List<Sprite> GetPlayerCards(int playerId)
        {
            if (playersWithCards.ContainsKey(playerId))
            {
                return playersWithCards[playerId];
            }
            return null;
        }
    }
}