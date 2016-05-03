using System;
using System.Collections.Generic;

namespace wwwplatform.Extensions
{
    public class String
    {
        private static Random _randomizer;
        private static Random randomizer { get { return _randomizer ?? GetRandomizer(); } }

        private static Random GetRandomizer()
        {
            _randomizer = new Random();
            return _randomizer;
        }

        public static string Random(int length = 32, int? seed = null)
        {
            return RandomString(length, seed);
        }

        private static string RandomString(int length = 32, int? seed = null)
        {
            string rndchars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
            return GenerateRandom(length, seed, rndchars);
        }

        private static string GenerateRandom(int length, int? seed, string rndchars)
        {
            Random r = seed.HasValue ? new Random(seed.Value) : randomizer;
            string s = "";
            while (s.Length < length)
            {
                s += rndchars.Substring(r.Next(0, rndchars.Length), 1);
            }
            return s;
        }
    }
}
