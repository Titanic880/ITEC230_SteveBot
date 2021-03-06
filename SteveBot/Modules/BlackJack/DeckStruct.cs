﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteveBot.Modules.BlackJack
{
    class DeckStruct
    {
        private List<Card> Deck = new List<Card>();
        private int DeckLeft { get { return Deck.Count; } }

        public DeckStruct()
        {
            for (int i = 0; i < 13; i++)
            {
                Card hearts = new Card(i, Card.CardSuit.Hearts);
                Card spades = new Card(i, Card.CardSuit.Spades);
                Card clubs = new Card(i, Card.CardSuit.Clubs);
                Card diamonds = new Card(i, Card.CardSuit.Diamonds);

                Deck.Add(hearts);
                Deck.Add(spades);
                Deck.Add(clubs);
                Deck.Add(diamonds);
            }
            Shuffle();
        }

        //Deck Shuffler
        private void Shuffle()
        {
            int num1, num2;
            for (int i = 0; i < 10000; i++)
            {
                num1 = main.rand.Next(0, 52);
                num2 = main.rand.Next(0, 52);
                //Stashing the old card in memory
                Card tmp = Deck[num1];
                Deck[num1] = Deck[num2];
                Deck[num2] = tmp;
            }
        }

        //Takes the first card from the top of the deck
        public Card TopDeck()
        {
            Card outp = Deck[0];
            Deck.Remove(Deck[0]);
            return outp;
        }

    }
}
