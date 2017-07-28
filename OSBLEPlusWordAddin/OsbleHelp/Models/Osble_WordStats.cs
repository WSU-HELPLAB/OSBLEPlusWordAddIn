using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;

namespace OSBLEPlus.Services.Models
{
    [Serializable]
    public class WordStats
    {
        string AuthToken { get; set; }

        int Characters { get; set; }
        int Words { get; set; }
        int Sentences { get; set; }
        int Paragraphs { get; set; }
        int Tables { get; set; }
        int Windows { get; set; }

        int SpellingErrors { get; set; }
        bool SpellingChecked { get; set; }

        int GrammaticalErrors { get; set; }
        bool GrammarChecked { get; set; }

        public WordStats(string authToken, Document doc)
        {
            AuthToken = authToken;

            Characters = doc.Characters.Count;
            Words = doc.Words.Count;
            Sentences = doc.Sentences.Count;
            Paragraphs = doc.Paragraphs.Count;
            Tables = doc.Tables.Count;
            Windows = doc.Windows.Count;

            SpellingErrors = doc.SpellingErrors.Count;
            SpellingChecked = doc.SpellingChecked;

            GrammaticalErrors = doc.GrammaticalErrors.Count;
            GrammarChecked = doc.GrammarChecked;
        }

        public WordStats(WordStats copy)
        {
            AuthToken = copy.AuthToken;

            Characters = copy.Characters;
            Words = copy.Words;
            Sentences = copy.Sentences;
            Paragraphs = copy.Paragraphs;
            Tables = copy.Tables;
            Windows = copy.Windows;

            SpellingErrors = copy.SpellingErrors;
            SpellingChecked = copy.SpellingChecked;

            GrammaticalErrors = copy.GrammaticalErrors;
            GrammarChecked = copy.GrammarChecked;
        }
        
        public WordStats()
        {

        }
    }
}

/*          
    var tmp = doc.GrammaticalErrors;
    var tmp2 = doc.SpellingErrors;
    var tmp3 = doc.Words;
    var tmp4 = doc.Windows;
    var tmp5 = doc.Versions;
    var tmp6 = doc.SpellingChecked;
    var tmp7 = doc.Sentences;
    var tmp8 = doc.Saved;
    var tmp9 = doc.Paragraphs;
    var tmp10 = doc.GrammarChecked;
    var tmp11 = doc.Comments;
    var tmp12 = doc.Characters;
*/
