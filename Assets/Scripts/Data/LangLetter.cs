using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class LangLetter
    {
        public LangLetter(char letter, float frequency, bool isVocal = false, float normalized = 0f, short difficultyGroup = 0)
        {
            Letter = letter;
            Frequency = frequency;
            IsVocal = isVocal;
            Normalized = normalized;
            DifficultyGroup = difficultyGroup;
        }
        public char Letter { get; set; }
        public float Frequency { get; set; }
        public bool IsVocal { get; set; }
        public float Normalized { get; set; }
        public short DifficultyGroup { get; set; }
        public float ObsNormalized { get; set; }

    }
}
