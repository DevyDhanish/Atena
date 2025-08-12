using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace atena
{
    public class SimpleParser
    {
        private List<string>? buffer;
        private string? parseRegex;
        private string? parseOutput;

        public SimpleParser()
        {
            buffer = new List<string>();
        }

        private void addToBuffer(string literal)
        {
            buffer?.Add(literal);
        }

        public void AddRegex(string regex)
        {
            parseRegex = regex;
        }

        public void AddOutput(string output)
        {
            parseOutput = output;
        }

        public string? Parse()
        {
            // this should never happen, if did something is wrong
            if (buffer == null)
            {
                Log.Err("Buffer is empty");
                return null;
            }

            if(parseRegex == null)
            {
                Log.Err("Regex is empty use AddRegex to add one");
                return null;
            }

            if(parseOutput == null)
            {
                Log.Err("Output is empty use AddOutput to add one");
                return null;
            }

            foreach(string literal in buffer)
            {
                Match result = Regex.Match(literal, parseRegex); 
                
                // found one
                if(result.Success)
                {
                    parseOutput += literal;
                    return parseOutput;
                }
            }

            return null;
        }

        public void AddLiteral(string literal)
        {
            int literalLength = new StringInfo(literal).LengthInTextElements;

            // we don't have anything in the buffer or the literal is not a single char so ignore
            if(buffer?.Count == 0 || literalLength != 1)
            {
                // fire the OnParsedText();
            }

            // since we only care about, words that are encapsulated in '*' or '**' 
            switch(literal)
            {
                case "*":
                    addToBuffer(literal);
                    Parse();
                    break;
                default:
                    break;
            }

        }
    }
}
