using System;
using System.Collections.Generic;
using System.Linq;

namespace ClueHelper.Models
{
    public class Category
    {
        public string Name { get; }
        public Card[] Cards { get; }

        public Category(string name, IEnumerable<string> cardNames)
        {
            Name = name;
            Cards = cardNames.Select(cardName => new Card(this, cardName)).ToArray();
        }

        public class Builder
        {
            private readonly string _name;
            private readonly IList<string> _cardNames = new List<string>();

            public Builder(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }
                _name = name;
            }

            public Builder AddCard(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }
                if (_cardNames.Contains(name))
                {
                    throw new ArgumentException($"Card already exists with name {name}.", nameof(name));
                }
                _cardNames.Add(name);
                return this;
            }

            public Category Build()
            {
                if (_cardNames.Count == 0)
                {
                    throw new InvalidOperationException("Can't build empty category.");
                }
                return new Category(_name, _cardNames);
            }
        }
    }
}
