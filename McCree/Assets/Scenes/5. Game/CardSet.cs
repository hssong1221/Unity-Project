using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.ThreeCS.McCree
{
    public class CardSet : MonoBehaviour  // 게임에 사용될 총 카드셋  (탁자 위 가운데 카드뭉치) 
    {
        public List<Card> cardList = new List<Card>();

        private void Start()
        {
            Card card = this.gameObject.AddComponent<Card>();
            card.ability = Card.cType.Bang;
            for (int i = 0; i < 30; i++) // 뱅 30개
            {
                cardList.Add(card);
            }
            Card card2 = this.gameObject.AddComponent<Card>();
            card2.ability = Card.cType.Avoid;
            for (int i = 0; i < 10; i++) // 회피 10개
            {
                cardList.Add(card2);
            }
            Card card3 = this.gameObject.AddComponent<Card>();
            card3.ability = Card.cType.Heal;
            for (int i = 0; i < 10; i++) // 힐 10개
            {
                cardList.Add(card3);
            }
            cardList = CommonFunction.ShuffleList(cardList);
        }
    }
}
