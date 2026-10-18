---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Helpers/NameGenerator.cs"
ref: HEAD
---

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JassToCsMain
{
    public class NameGenerator
    {
        private IEnumerable<char> validCharacters = new char[]
        {
            //'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j','k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J','K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        };

        private Dictionary<int, IEnumerator<char>> enumeratorByPosition = new Dictionary<int, IEnumerator<char>>();

        private int count;

        private StringBuilder currentName = null;

        public NameGenerator(int count)
        {
            this.count = count;

            enumeratorByPosition = Enumerable.Range(1, count)
                .Select(i => new { Position = count - i, Enumerator = validCharacters.GetEnumerator() })
                .ToDictionary(item => item.Position, item => item.Enumerator);
        }

        public string GetNext()
        {
            if (currentName == null)
            {
                List<char> chars = new List<char>();
                foreach (var keyValue in enumeratorByPosition)
                {
                    keyValue.Value.MoveNext();
                    chars.Add(keyValue.Value.Current);
                }

                currentName = new StringBuilder(new string(chars.ToArray()));
            }
            else
            {
                List<char> chars = new List<char>();
                foreach (var keyValue in enumeratorByPosition)
                {
                    int position = keyValue.Key;
                    IEnumerator<char> enumerator = keyValue.Value;

                    if (enumerator.MoveNext())
                    {
                        currentName[position] = enumerator.Current;
                        break;
                    }
                    else
                    {
                        if (position == 0)
                        {
                            throw new Exception();
                        }

                        enumerator.Reset();
                        enumerator.MoveNext();
                        currentName[position] = enumerator.Current;
                    }
                }
            }

            return currentName.ToString();
        }

        private char GetNextChar(IEnumerator<char> enumerator)
        {
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
            else
            {
                enumerator.Reset();
                enumerator.MoveNext();
                return enumerator.Current;
            }
        }
    }
}
